using Authenticator.API.Core.Application.Interfaces.AccessControl.AccessGroup;
using Authenticator.API.Core.Application.Interfaces.AccessControl.Module;
using Authenticator.API.Core.Application.Interfaces.AccessControl.RoleAccessGroups;
using Authenticator.API.Core.Application.Interfaces.AccessControl.RolePermissions;
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
        IAccessGroupRepository accessGroupRepository,
        IRolePermissionRepository rolePermissionRepository,
        IRoleAccessGroupRepository roleAccessGroupRepository,
        IModuleRepository moduleRepository,
        IUserContext userContext,
        IMapper mapper,
        ILogger<RolePainelService> logger
    ) : IRolePainelService
    {
        private readonly IRoleRepository _roleRepository = roleRepository;
        private readonly IAccessGroupRepository _accessGroupRepository = accessGroupRepository;
        private readonly IRolePermissionRepository _rolePermissionRepository = rolePermissionRepository;
        private readonly IRoleAccessGroupRepository _roleAccessGroupRepository = roleAccessGroupRepository;
        private readonly IModuleRepository _moduleRepository = moduleRepository;
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

        public async Task<ResponseDTO<RoleDTO>> AddRoleAsync(RoleCreateDTO dto)
        {
            try
            {
                var tenantId = _userContext.CurrentUser?.TenantId;
                if (!tenantId.HasValue)
                    return StaticResponseBuilder<RoleDTO>.BuildError("TenantId não encontrado no contexto do usuário.");

                var entity = _mapper.Map<RoleEntity>(dto);
                entity.CreatedAt = DateTime.Now;
                entity.TenantId = tenantId.Value;

                var created = await _roleRepository.AddAsync(entity);

                if (dto.Permissions != null && dto.Permissions.Any())
                    await AssignPermissionsInternalAsync(created.Id, dto.Permissions);

                var full = await _roleRepository.GetByIdAsync(created.Id, include: r => r
                    .Include(x => x.RolePermissions));

                var resultDto = _mapper.Map<RoleDTO>(full);
                return StaticResponseBuilder<RoleDTO>.BuildCreated(resultDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar role (Painel)");
                return StaticResponseBuilder<RoleDTO>.BuildErrorResponse(ex);
            }
        }

        public async Task<ResponseDTO<RoleDTO>> UpdateRoleAsync(Guid id, RoleUpdateDTO dto)
        {
            try
            {
                var tenantId = _userContext.CurrentUser?.TenantId;
                var existing = await _roleRepository.GetByIdAsync(id, include: r => r.Include(x => x.RolePermissions));

                if (existing == null || (tenantId.HasValue && existing.TenantId != tenantId.Value))
                    return StaticResponseBuilder<RoleDTO>.BuildError("Role não encontrada ou não pertence ao seu tenant.");

                _mapper.Map(dto, existing);
                existing.UpdatedAt = DateTime.Now;
                await _roleRepository.UpdateAsync(existing);

                if (dto.Permissions != null)
                    await SyncRolePermissionsInternalAsync(id, dto.Permissions);

                var resultDto = _mapper.Map<RoleDTO>(existing);
                return StaticResponseBuilder<RoleDTO>.BuildOk(resultDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar role (Painel)");
                return StaticResponseBuilder<RoleDTO>.BuildErrorResponse(ex);
            }
        }

        public async Task<ResponseDTO<bool>> DeleteRoleAsync(Guid id)
        {
            try
            {
                var tenantId = _userContext.CurrentUser?.TenantId;
                var existing = await _roleRepository.GetByIdAsync(id);

                if (existing == null || (tenantId.HasValue && existing.TenantId != tenantId.Value))
                    return StaticResponseBuilder<bool>.BuildError("Role não encontrada ou não pertence ao seu tenant.");

                // Não permitir excluir roles do sistema (se houver) se o usuário quiser proteger isso
                if (existing.IsSystem)
                    return StaticResponseBuilder<bool>.BuildError("Roles de sistema não podem ser excluídas.");

                await _rolePermissionRepository.RemoveAllByRoleIdAsync(id);
                await _roleAccessGroupRepository.RemoveAllByRoleIdAsync(id);
                await _roleRepository.DeleteAsync(existing);

                return StaticResponseBuilder<bool>.BuildOk(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir role (Painel)");
                return StaticResponseBuilder<bool>.BuildErrorResponse(ex);
            }
        }

        public async Task<ResponseDTO<IEnumerable<SimplifiedModuleDTO>>> GetAvailableModulesAsync()
        {
            try
            {
                // Por enquanto, todos os módulos ativos. No futuro, filtrar por aplicação ou plano.
                var modules = await _moduleRepository.FindAsync(m => m.IsActive);
                var dtos = modules.Select(m => new SimplifiedModuleDTO
                {
                    Key = m.Key ?? m.Name,
                    Name = m.Name,
                    Description = m.Description,
                    AvailableActions = new List<string> { "CREATE", "READ", "UPDATE", "DELETE" }
                });

                return StaticResponseBuilder<IEnumerable<SimplifiedModuleDTO>>.BuildOk(dtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar módulos disponíveis");
                return StaticResponseBuilder<IEnumerable<SimplifiedModuleDTO>>.BuildErrorResponse(ex);
            }
        }

        private async Task<bool> AssignPermissionsInternalAsync(Guid roleId, List<SimplifiedPermissionDTO> permissions)
        {
            var existing = await _rolePermissionRepository.GetAllRolePermissionsByRoleIdAsync(roleId);

            foreach (var perm in permissions)
            {
                var rel = existing.FirstOrDefault(rp => rp.ModuleKey == perm.Module);
                if (rel != null)
                {
                    rel.Actions = perm.Actions;
                    rel.IsActive = true;
                    rel.UpdatedAt = DateTime.Now;
                    await _rolePermissionRepository.UpdateAsync(rel);
                }
                else
                {
                    await _rolePermissionRepository.AddAsync(new RolePermissionEntity
                    {
                        Id = Guid.NewGuid(),
                        RoleId = roleId,
                        ModuleKey = perm.Module,
                        Actions = perm.Actions,
                        IsActive = true,
                        CreatedAt = DateTime.Now
                    });
                }
            }
            return true;
        }

        private async Task SyncRolePermissionsInternalAsync(Guid roleId, List<SimplifiedPermissionDTO> permissions)
        {
            var existing = await _rolePermissionRepository.GetAllRolePermissionsByRoleIdAsync(roleId);
            var newModuleKeys = permissions.Select(p => p.Module).ToList();
            var toRemove = existing.Where(rp => !newModuleKeys.Contains(rp.ModuleKey)).Select(rp => rp.ModuleKey).ToList();

            if (toRemove.Count != 0)
                await _rolePermissionRepository.RemoveByRoleAndModulesAsync(roleId, toRemove);

            await AssignPermissionsInternalAsync(roleId, permissions);
        }
    }
}
