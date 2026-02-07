using Authenticator.API.Core.Application.Interfaces.AccessControl.AccountAccessGroups;
using Authenticator.API.Core.Application.Interfaces.AccessControl.AccessGroup;
using Authenticator.API.Core.Application.Interfaces;
using Authenticator.API.Core.Application.Interfaces.Auth;
using Authenticator.API.Core.Domain.AccessControl.AccessGroup.DTOs;
using Authenticator.API.Core.Domain.AccessControl.AccountAccessGroups.DTOs;
using OpaMenu.Infrastructure.Shared.Entities.AccessControl;
using Authenticator.API.Core.Domain.Api;
using AutoMapper;
using System.Linq.Expressions;

namespace Authenticator.API.Core.Application.Implementation.AccessControl.AccountAccessGroups
{
    /// <summary>
    /// Serviço para gerenciamento de vínculos de grupos de acesso com usuários
    /// </summary>
    public class AccountAccessGroupService(
        IAccountAccessGroupRepository accountAccessGroupRepository,
        IUserAccountsRepository userRepository,
        IAccessGroupRepository accessGroupRepository,
        IUserContext userContext,
        IMapper mapper,
        ILogger<AccountAccessGroupService> logger
    ) : IAccountAccessGroupService
    {
        private readonly IAccountAccessGroupRepository _accountAccessGroupRepository = accountAccessGroupRepository;
        private readonly IUserAccountsRepository _userRepository = userRepository;
        private readonly IAccessGroupRepository _accessGroupRepository = accessGroupRepository;
        private readonly IUserContext _userContext = userContext;
        private readonly IMapper _mapper = mapper;
        private readonly ILogger<AccountAccessGroupService> _logger = logger;

        public async Task<ResponseDTO<IEnumerable<AccessGroupDTO>>> GetUserAccessGroupsAsync(Guid userId)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(userId);

                var links = await _accountAccessGroupRepository.GetByUserAsync(userId);
                var groups = links
                    .Where(aag => aag.AccessGroup != null && aag.AccessGroup.IsActive)
                    .Select(aag => aag.AccessGroup)
                    .ToList();

                var dtos = _mapper.Map<IEnumerable<AccessGroupDTO>>(groups);
                return ResponseBuilder<IEnumerable<AccessGroupDTO>>.Ok(dtos).Build();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao listar grupos do usuário");
                return ResponseBuilder<IEnumerable<AccessGroupDTO>>
                    .Fail(new ErrorDTO { Message = ex.Message })
                    .WithException(ex)
                    .WithCode(500)
                    .Build();
            }
        }

        public async Task<ResponseDTO<bool>> AssignAccessGroupsAsync(Guid userId, AssignUserAccessGroupsDTO request)
        {
            try
            {
                var tenantId = _userContext.CurrentUser?.TenantId;
                var isSuperAdmin = _userContext.CurrentUser?.Roles?.Contains("SUPER_ADMIN") == true;

                if (!tenantId.HasValue && !isSuperAdmin)
                {
                    return ResponseBuilder<bool>
                        .Fail(new ErrorDTO { Message = "Tenant não identificado" })
                        .WithCode(400)
                        .Build();
                }

                if (request.AccessGroupIds == null || request.AccessGroupIds.Count == 0)
                {
                    return ResponseBuilder<bool>
                        .Fail(new ErrorDTO { Message = "Informe pelo menos um grupo" })
                        .WithCode(400)
                        .Build();
                }

                var user = await _userRepository.GetByIdAsync(userId);
                
                // Se for SUPER_ADMIN, ignora validação de tenant do usuário
                if (!isSuperAdmin && (user == null || user.TenantId != tenantId))
                {
                    return ResponseBuilder<bool>
                        .Fail(new ErrorDTO { Message = "usuário não encontrado no tenant" })
                        .WithCode(404)
                        .Build();
                }

                // Validar grupos pertencentes ao tenant e ativos
                var targetIds = request.AccessGroupIds.Distinct().ToList();
                
                // Se for SUPER_ADMIN, busca grupos globais ou de sistema, senão busca por tenant
                IEnumerable<AccessGroupEntity> validGroups;
                
                if (isSuperAdmin)
                    validGroups = await _accessGroupRepository.FindAsync(g => targetIds.Contains(g.Id) && g.IsActive);
                else
                    validGroups = await _accessGroupRepository.FindAsync(g => g.TenantId == tenantId.Value && targetIds.Contains(g.Id) && g.IsActive);

                var validIds = validGroups.Select(g => g.Id).ToHashSet();
                var invalidIds = targetIds.Where(id => !validIds.Contains(id)).ToList();
                if (invalidIds.Count > 0)
                {
                    return ResponseBuilder<bool>
                        .Fail(new ErrorDTO { Message = $"Grupos inválido ou de outro tenant: {string.Join(", ", invalidIds)}" })
                        .WithCode(400)
                        .Build();
                }

                await _accountAccessGroupRepository.AssignGroupsAsync(userId, targetIds, null, request.ExpiresAt);

                return ResponseBuilder<bool>.Ok(true).Build();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atribuir grupos ao usuário");
                return ResponseBuilder<bool>
                    .Fail(new ErrorDTO { Message = ex.Message })
                    .WithException(ex)
                    .WithCode(500)
                    .Build();
            }
        }

        public async Task<ResponseDTO<bool>> RevokeAccessGroupAsync(Guid userId, Guid accessGroupId)
        {
            try
            {
                var tenantId = _userContext.CurrentUser?.TenantId;
                if (!tenantId.HasValue)
                {
                    return ResponseBuilder<bool>
                        .Fail(new ErrorDTO { Message = "Tenant nÃ£o identificado" })
                        .WithCode(400)
                        .Build();
                }

                var user = await _userRepository.GetByIdAsync(userId);
                if (user == null || user.TenantId != tenantId)
                {
                    return ResponseBuilder<bool>
                        .Fail(new ErrorDTO { Message = "usuário nÃ£o encontrado no tenant" })
                        .WithCode(404)
                        .Build();
                }

                var group = await _accessGroupRepository.FirstOrDefaultAsync(ag => ag.Id == accessGroupId && ag.TenantId == tenantId);
                if (group == null)
                {
                    return ResponseBuilder<bool>
                        .Fail(new ErrorDTO { Message = "Grupo nÃ£o encontrado no tenant" })
                        .WithCode(404)
                        .Build();
                }

                var ok = await _accountAccessGroupRepository.RevokeAsync(userId, accessGroupId);
                if (!ok)
                {
                    return ResponseBuilder<bool>
                        .Fail(new ErrorDTO { Message = "VÃ­nculo nÃ£o encontrado ou jÃ¡ revogado" })
                        .WithCode(404)
                        .Build();
                }

                return ResponseBuilder<bool>.Ok(true).Build();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao revogar grupo do usuário");
                return ResponseBuilder<bool>
                    .Fail(new ErrorDTO { Message = ex.Message })
                    .WithException(ex)
                    .WithCode(500)
                    .Build();
            }
        }
    }
}
