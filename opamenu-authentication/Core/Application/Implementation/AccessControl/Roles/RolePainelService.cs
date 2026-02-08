using Authenticator.API.Core.Application.Interfaces.AccessControl.Roles;
using Authenticator.API.Core.Application.Interfaces.Auth;
using Authenticator.API.Core.Domain.AccessControl.Roles.DTOs;
using Authenticator.API.Core.Domain.Api;
using Authenticator.API.Core.Domain.Api.Commons;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using OpaMenu.Infrastructure.Shared.Entities.AccessControl;
using System.Linq.Expressions;

namespace Authenticator.API.Core.Application.Implementation.AccessControl.Roles
{
    public class RolePainelService(
        IRoleRepository roleRepository,
        IUserContext userContext,
        IMapper mapper,
        ILogger<RolePainelService> logger
    ) : IRolePainelService
    {
        private readonly IRoleRepository _roleRepository = roleRepository;
        private readonly IUserContext _userContext = userContext;
        private readonly IMapper _mapper = mapper;
        private readonly ILogger<RolePainelService> _logger = logger;

        public async Task<ResponseDTO<PagedResponseDTO<RoleDTO>>> GetAllRolesPagedAsync(int page, int limit, string? name = null)
        {
            try
            {
                var tenantId = _userContext.CurrentUser?.TenantId;
                if (!tenantId.HasValue)
                    return StaticResponseBuilder<PagedResponseDTO<RoleDTO>>.BuildError("TenantId não encontrado no contexto do usuário.");

                Expression<Func<RoleEntity, bool>> predicate = r => r.TenantId == tenantId.Value && (string.IsNullOrEmpty(name) || r.Name.Contains(name));
                var total = await _roleRepository.CountAsync(predicate);

                var entities = await _roleRepository.GetPagedAsync(
                    predicate: predicate,
                    pageNumber: page,
                    pageSize: limit,
                    include: r => r
                        .Include(x => x.RolePermissions)
                        .Include(x => x.RoleAccessGroups)!.ThenInclude(rag => rag.AccessGroup));

                if (total == 0)
                {
                    return StaticResponseBuilder<PagedResponseDTO<RoleDTO>>.BuildOk(new PagedResponseDTO<RoleDTO>
                    {
                        Items = Enumerable.Empty<RoleDTO>(),
                        Page = page,
                        Limit = limit,
                        Total = 0,
                        TotalPages = 0
                    });
                }

                var dtos = _mapper.Map<IEnumerable<RoleDTO>>(entities);
                var paged = new PagedResponseDTO<RoleDTO>
                {
                    Items = dtos,
                    Page = page,
                    Limit = limit,
                    Total = total,
                    TotalPages = (int)Math.Ceiling((double)total / limit)
                };
                return StaticResponseBuilder<PagedResponseDTO<RoleDTO>>.BuildOk(paged);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao listar roles por tenant (Painel)");
                return StaticResponseBuilder<PagedResponseDTO<RoleDTO>>.BuildErrorResponse(ex);
            }
        }

        public async Task<ResponseDTO<RoleDTO>> GetRoleByIdAsync(Guid id)
        {
            try
            {
                var tenantId = _userContext.CurrentUser?.TenantId;
                var entity = await _roleRepository.GetByIdAsync(id, include: r => r
                    .Include(x => x.RolePermissions)
                    .Include(x => x.RoleAccessGroups)!.ThenInclude(rag => rag.AccessGroup));

                if (entity == null || (tenantId.HasValue && tenantId.Value != Guid.Empty && entity.TenantId != tenantId.Value))
                    return StaticResponseBuilder<RoleDTO>.BuildError("Role não encontrada ou não pertence ao seu tenant.");

                var dto = _mapper.Map<RoleDTO>(entity);
                return StaticResponseBuilder<RoleDTO>.BuildOk(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar role por ID (Painel)");
                return StaticResponseBuilder<RoleDTO>.BuildErrorResponse(ex);
            }
        }
    }
}
