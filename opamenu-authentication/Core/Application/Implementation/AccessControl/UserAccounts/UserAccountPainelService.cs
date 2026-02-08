using Authenticator.API.Core.Application.Interfaces;
using Authenticator.API.Core.Application.Interfaces.AccessControl.UserAccounts;
using Authenticator.API.Core.Application.Interfaces.Auth;
using Authenticator.API.Core.Domain.AccessControl.UserAccounts.DTOs;
using Authenticator.API.Core.Domain.Api;
using Authenticator.API.Core.Domain.Api.Commons;
using AutoMapper;
using OpaMenu.Infrastructure.Shared.Entities.AccessControl.UserAccounts;
using OpaMenu.Infrastructure.Shared.Entities.AccessControl.UserAccounts.Enum;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace Authenticator.API.Core.Application.Implementation.AccessControl.UserAccounts
{
    public class UserAccountPainelService(
        IUserAccountsRepository userRepository,
        IMapper mapper,
        IUserContext userContext,
        ILogger<UserAccountPainelService> logger
    ) : IUserAccountPainelService
    {
        private readonly IUserAccountsRepository _userRepository = userRepository;
        private readonly IMapper _mapper = mapper;
        private readonly IUserContext _userContext = userContext;
        private readonly ILogger<UserAccountPainelService> _logger = logger;

        public async Task<ResponseDTO<PagedResponseDTO<UserAccountDTO>>> GetAllEmployeePagedAsync(int page, int limit, string? search = null)
        {
            try
            {
                var tenantId = _userContext.CurrentUser?.TenantId;
                if (!tenantId.HasValue)
                    return StaticResponseBuilder<PagedResponseDTO<UserAccountDTO>>.BuildError("TenantId não encontrado no contexto do usuário.");

                if (page < 1) page = 1;
                if (limit < 1) limit = 10;
                if (limit > 100) limit = 100;

                Expression<Func<UserAccountEntity, bool>> predicate = u => u.TenantId == tenantId.Value && 
                    (string.IsNullOrEmpty(search) || 
                     u.Username.Contains(search) || 
                     u.Email.Contains(search) || 
                     u.FirstName.Contains(search) || 
                     u.LastName.Contains(search));

                var total = await _userRepository.CountAsync(predicate);

                var userAccountEntities = await _userRepository.GetPagedAsync(
                    predicate: predicate,
                    pageNumber: page,
                    pageSize: limit,
                    include: query => query.Include(u => u.Role));

                var items = _mapper.Map<IEnumerable<UserAccountDTO>>(userAccountEntities);
                var totalPages = total == 0 ? 0 : (int)Math.Ceiling(total / (double)limit);
                
                var pagedResult = new PagedResponseDTO<UserAccountDTO>
                {
                    Items = items,
                    Page = page,
                    Limit = limit,
                    Total = total,
                    TotalPages = totalPages
                };
                
                return StaticResponseBuilder<PagedResponseDTO<UserAccountDTO>>.BuildOk(pagedResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao listar colaboradores por tenant");
                return StaticResponseBuilder<PagedResponseDTO<UserAccountDTO>>.BuildErrorResponse(ex);
            }
        }

        public async Task<ResponseDTO<UserAccountDTO>> GetEmployeeByIdAsync(Guid id)
        {
            try
            {
                var tenantId = _userContext.CurrentUser?.TenantId;
                var user = await _userRepository.GetByIdAsync(id);

                if (user == null || (tenantId.HasValue && user.TenantId != tenantId))
                    return StaticResponseBuilder<UserAccountDTO>.BuildError("Colaborador não encontrado ou não pertence ao seu tenant.");

                var dto = _mapper.Map<UserAccountDTO>(user);
                return StaticResponseBuilder<UserAccountDTO>.BuildOk(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar colaborador por ID");
                return StaticResponseBuilder<UserAccountDTO>.BuildErrorResponse(ex);
            }
        }

        public async Task<ResponseDTO<UserAccountDTO>> CreateEmployeeAsync(UserAccountCreateDTO dto)
        {
            try
            {
                var tenantId = _userContext.CurrentUser?.TenantId;
                if (!tenantId.HasValue)
                    return StaticResponseBuilder<UserAccountDTO>.BuildError("TenantId não encontrado no contexto do usuário.");

                if (await _userRepository.EmailExistsAsync(dto.Email))
                    return StaticResponseBuilder<UserAccountDTO>.BuildError("Email já está em uso.");

                var userName = dto.Email.Split('@')[0];
                if (await _userRepository.UsernameExistsAsync(userName))
                {
                    userName = $"{userName}{new Random().Next(1000, 9999)}";
                }

                var entity = _mapper.Map<UserAccountEntity>(dto);
                entity.Username = userName;
                entity.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
                entity.TenantId = tenantId.Value; // Força o tenant do admin logado
                entity.Status = EUserAccountStatus.Ativo; // Colaboradores criados pelo painel são ativos por padrão

                var created = await _userRepository.AddAsync(entity);
                var createdDto = _mapper.Map<UserAccountDTO>(created);
                return StaticResponseBuilder<UserAccountDTO>.BuildCreated(createdDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar colaborador");
                return StaticResponseBuilder<UserAccountDTO>.BuildErrorResponse(ex);
            }
        }

        public async Task<ResponseDTO<UserAccountDTO>> UpdateEmployeeAsync(Guid id, UserAccountUpdateDto dto)
        {
            try
            {
                var tenantId = _userContext.CurrentUser?.TenantId;
                var existingUser = await _userRepository.GetByIdAsync(id);

                if (existingUser == null || (tenantId.HasValue && existingUser.TenantId != tenantId))
                    return StaticResponseBuilder<UserAccountDTO>.BuildError("Colaborador não encontrado ou não pertence ao seu tenant.");

                if (!string.IsNullOrWhiteSpace(dto.Email))
                {
                    if (await _userRepository.EmailExistsAsync(dto.Email, id))
                        return StaticResponseBuilder<UserAccountDTO>.BuildError("Email já está em uso.");
                }

                _mapper.Map(dto, existingUser);
                existingUser.UpdatedAt = DateTime.UtcNow;

                await _userRepository.UpdateAsync(existingUser);
                var updatedDto = _mapper.Map<UserAccountDTO>(existingUser);
                return StaticResponseBuilder<UserAccountDTO>.BuildOk(updatedDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar colaborador");
                return StaticResponseBuilder<UserAccountDTO>.BuildErrorResponse(ex);
            }
        }

        public async Task<ResponseDTO<UserAccountDTO>> ToggleEmployeeStatusAsync(Guid id)
        {
            try
            {
                var tenantId = _userContext.CurrentUser?.TenantId;
                var existingUser = await _userRepository.GetByIdAsync(id);

                if (existingUser == null || (tenantId.HasValue && existingUser.TenantId != tenantId))
                    return StaticResponseBuilder<UserAccountDTO>.BuildError("Colaborador não encontrado ou não pertence ao seu tenant.");

                existingUser.Status = existingUser.Status == EUserAccountStatus.Ativo 
                    ? EUserAccountStatus.Inativo 
                    : EUserAccountStatus.Ativo;
                
                existingUser.UpdatedAt = DateTime.UtcNow;

                await _userRepository.UpdateAsync(existingUser);
                var updatedDto = _mapper.Map<UserAccountDTO>(existingUser);
                return StaticResponseBuilder<UserAccountDTO>.BuildOk(updatedDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao alterar status do colaborador");
                return StaticResponseBuilder<UserAccountDTO>.BuildErrorResponse(ex);
            }
        }

        public async Task<ResponseDTO<bool>> DeleteEmployeeAsync(Guid id)
        {
            try
            {
                var tenantId = _userContext.CurrentUser?.TenantId;
                var existing = await _userRepository.GetByIdAsync(id);

                if (existing == null || (tenantId.HasValue && existing.TenantId != tenantId))
                    return StaticResponseBuilder<bool>.BuildError("Colaborador não encontrado ou não pertence ao seu tenant.");

                await _userRepository.DeleteAsync(existing);
                return StaticResponseBuilder<bool>.BuildOk(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao deletar colaborador");
                return StaticResponseBuilder<bool>.BuildErrorResponse(ex);
            }
        }
    }
}
