using Authenticator.API.Core.Application.Interfaces.AccessControl.Roles;
using Authenticator.API.Core.Application.Interfaces.AccessControl.RolePermissions;
using Authenticator.API.Core.Application.Interfaces.AccessControl.RoleAccessGroups;
using Authenticator.API.Core.Application.Interfaces.AccessControl.AccessGroup;
using Authenticator.API.Core.Application.Interfaces.Auth;
using OpaMenu.Infrastructure.Shared.Entities.AccessControl;
using Authenticator.API.Core.Domain.AccessControl.Roles.DTOs;
using Authenticator.API.Core.Domain.AccessControl.AccessGroup.DTOs;
using Authenticator.API.Core.Domain.Api;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Authenticator.API.Core.Domain.Api.Commons;

namespace Authenticator.API.Core.Application.Implementation.AccessControl.Roles
{
    /// <summary>
    /// Serviço para gerenciamento de Roles
    /// </summary>
    public class RoleService(
        IRoleRepository roleRepository,
        IAccessGroupRepository accessGroupRepository,
        IRolePermissionRepository rolePermissionRepository,
        IRoleAccessGroupRepository roleAccessGroupRepository,
        IUserContext userContext,
        IMapper mapper
    ) : IRoleService
    {
        private readonly IRoleRepository _roleRepository = roleRepository;
        private readonly IAccessGroupRepository _accessGroupRepository = accessGroupRepository;
        private readonly IRolePermissionRepository _rolePermissionRepository = rolePermissionRepository;
        private readonly IRoleAccessGroupRepository _roleAccessGroupRepository = roleAccessGroupRepository;
        private readonly IUserContext _userContext = userContext;
        private readonly IMapper _mapper = mapper;

        public async Task<ResponseDTO<IEnumerable<RoleDTO>>> GetAllRolesAsync()
        {
            try
            {
                var entities = await _roleRepository.GetAllAsync(include: r => r
                    .Include(x => x.RolePermissions)
                    .Include(x => x.RoleAccessGroups)!.ThenInclude(rag => rag.AccessGroup));

                if (!entities.Any())
                    return StaticResponseBuilder<IEnumerable<RoleDTO>>.BuildOk([]);

                var dtos = _mapper.Map<IEnumerable<RoleDTO>>(entities);
                return StaticResponseBuilder<IEnumerable<RoleDTO>>.BuildOk(dtos);

            }
            catch (Exception ex)
            {
                return StaticResponseBuilder<IEnumerable<RoleDTO>>.BuildErrorResponse(ex);
            }
        }

        public async Task<ResponseDTO<PagedResponseDTO<RoleDTO>>> GetAllRolesPagedAsync(int page, int limit)
        {
            try
            {
                var entities = await _roleRepository.GetPagedAsync(
                    page: page,
                    pageSize: limit,
                    include: r => r
                        .Include(x => x.RolePermissions)
                        .Include(x => x.RoleAccessGroups)!.ThenInclude(rag => rag.AccessGroup));

                if (!entities.Any())
                    return StaticResponseBuilder<PagedResponseDTO<RoleDTO>>.BuildOk(null!);

                var dtos = _mapper.Map<IEnumerable<RoleDTO>>(entities);
                var paged = new PagedResponseDTO<RoleDTO>
                {
                    Items = dtos,
                    Page = page,
                    Limit = limit,
                    Total = entities.Count(),
                    TotalPages = (int)Math.Ceiling((double)entities.Count() / limit)
                };
                return StaticResponseBuilder<PagedResponseDTO<RoleDTO>>.BuildOk(paged);
            }
            catch (Exception ex)
            {
                return StaticResponseBuilder<PagedResponseDTO<RoleDTO>>.BuildErrorResponse(ex);
            }
        }

        public async Task<ResponseDTO<RoleDTO>> GetRoleByIdAsync(Guid id)
        {
            try
            {
                var entity = await _roleRepository.GetByIdAsync(id, include: r => r
                    .Include(x => x.RolePermissions)
                    .Include(x => x.RoleAccessGroups)!.ThenInclude(rag => rag.AccessGroup));

                if (entity == null)
                    return StaticResponseBuilder<RoleDTO>.BuildOk(null!);

                var currentTenantId = _userContext.CurrentUser?.TenantId;
                if (currentTenantId.HasValue && currentTenantId.Value != Guid.Empty && entity.TenantId != currentTenantId.Value)
                    return StaticResponseBuilder<RoleDTO>.BuildError("Role não encontrada");

                var dto = _mapper.Map<RoleDTO>(entity);
                return StaticResponseBuilder<RoleDTO>.BuildOk(dto);
            }
            catch (Exception ex)
            {
                return StaticResponseBuilder<RoleDTO>.BuildErrorResponse(ex);
            }
        }

        public async Task<ResponseDTO<IEnumerable<RoleDTO>>> GetRolesByTenantAsync(Guid tenantId)
        {
            try
            {
                var currentTenantId = _userContext.CurrentUser?.TenantId;
                if (currentTenantId.HasValue && currentTenantId.Value != Guid.Empty && currentTenantId.Value != tenantId)
                    return StaticResponseBuilder<IEnumerable<RoleDTO>>.BuildOk([]);

                var entities = await _roleRepository.GetAllByTenantAsync(tenantId);

                if (!entities.Any())
                    return StaticResponseBuilder<IEnumerable<RoleDTO>>.BuildOk([]);

                var entitiesWithIncludes = new List<RoleEntity>();
                foreach (var role in entities)
                {
                    var full = await _roleRepository.GetByIdAsync(role.Id, include: r => r
                        .Include(x => x.RolePermissions)
                        .Include(x => x.RoleAccessGroups)!.ThenInclude(rag => rag.AccessGroup));
                    if (full != null)
                        entitiesWithIncludes.Add(full);
                }

                var dtos = _mapper.Map<IEnumerable<RoleDTO>>(entitiesWithIncludes);
                return StaticResponseBuilder<IEnumerable<RoleDTO>>.BuildOk(dtos);
            }
            catch (Exception ex)
            {
                return StaticResponseBuilder<IEnumerable<RoleDTO>>.BuildErrorResponse(ex);
            }
        }

        public async Task<ResponseDTO<RoleDTO>> AddRoleAsync(RoleCreateDTO dto)
        {
            try
            {
                var entity = _mapper.Map<RoleEntity>(dto);
                entity.CreatedAt = DateTime.Now;

                var currentTenantId = _userContext.CurrentUser?.TenantId;
                if (currentTenantId.HasValue && currentTenantId.Value != Guid.Empty)
                    entity.TenantId = currentTenantId.Value;

                var created = await _roleRepository.AddAsync(entity);

                if (dto.Permissions != null && dto.Permissions.Any())
                    await AssignPermissionsInternalAsync(created.Id, dto.Permissions);

                // Associar grupos de acesso
                if (dto.AccessGroupIds != null && dto.AccessGroupIds.Any())
                    await AssignAccessGroupsInternalAsync(created.Id, dto.AccessGroupIds);

                var full = await _roleRepository.GetByIdAsync(created.Id, include: r => r
                    .Include(x => x.RolePermissions)
                    .Include(x => x.RoleAccessGroups)!.ThenInclude(rag => rag.AccessGroup));

                var resultDto = _mapper.Map<RoleDTO>(full);
                return StaticResponseBuilder<RoleDTO>.BuildOk(resultDto);
            }
            catch (Exception ex)
            {
                return StaticResponseBuilder<RoleDTO>.BuildErrorResponse(ex);
            }
        }

        public async Task<ResponseDTO<RoleDTO>> UpdateRoleAsync(Guid id, RoleUpdateDTO dto)
        {
            try
            {
                var existing = await _roleRepository.GetByIdAsync(id, include: r => r
                    .Include(x => x.RolePermissions)
                    .Include(x => x.RoleAccessGroups)!.ThenInclude(rag => rag.AccessGroup));

                if (existing == null)
                    return StaticResponseBuilder<RoleDTO>.BuildError("Role não encontrada");

                _mapper.Map(dto, existing);
                existing.UpdatedAt = DateTime.Now;
                await _roleRepository.UpdateAsync(existing);

                if (dto.Permissions != null)
                    await SyncRolePermissionsInternalAsync(id, dto.Permissions);

                if (dto.AccessGroupIds != null)
                    await SyncRoleAccessGroupsInternalAsync(id, dto.AccessGroupIds);

                var resultDto = _mapper.Map<RoleDTO>(existing);
                return StaticResponseBuilder<RoleDTO>.BuildOk(resultDto);
            }
            catch (Exception ex)
            {
                return StaticResponseBuilder<RoleDTO>.BuildErrorResponse(ex);
            }
        }

        public async Task<ResponseDTO<bool>> DeleteRoleAsync(Guid id)
        {
            try
            {
                var existing = await _roleRepository.GetByIdAsync(id);
                if (existing == null)
                    return StaticResponseBuilder<bool>.BuildError("Role não encontrada");

                var currentTenantId = _userContext.CurrentUser?.TenantId;
                if (currentTenantId.HasValue && currentTenantId.Value != Guid.Empty && existing.TenantId != currentTenantId.Value)
                    return StaticResponseBuilder<bool>.BuildError("Role não encontrada");

                await _rolePermissionRepository.RemoveAllByRoleIdAsync(id);
                await _roleAccessGroupRepository.RemoveAllByRoleIdAsync(id);

                await _roleRepository.DeleteAsync(existing);
                return StaticResponseBuilder<bool>.BuildOk(true);
            }
            catch (Exception ex)
            {
                return StaticResponseBuilder<bool>.BuildErrorResponse(ex);
            }
        }

        public async Task<ResponseDTO<IEnumerable<SimplifiedPermissionDTO>>> GetPermissionsByRoleAsync(Guid roleId)
        {
            try
            {
                var role = await _roleRepository.GetByIdAsync(roleId, include: r => r.Include(x => x.RolePermissions));
                if (role == null)
                    return StaticResponseBuilder<IEnumerable<SimplifiedPermissionDTO>>.BuildOk([]);

                var dtos = role.RolePermissions
                    .Where(rp => rp.IsActive)
                    .Select(rp => new SimplifiedPermissionDTO
                    {
                        Module = rp.ModuleKey,
                        Actions = rp.Actions
                    });

                return StaticResponseBuilder<IEnumerable<SimplifiedPermissionDTO>>.BuildOk(dtos);
            }
            catch (Exception ex)
            {
                return StaticResponseBuilder<IEnumerable<SimplifiedPermissionDTO>>.BuildErrorResponse(ex);
            }
        }

        public async Task<ResponseDTO<bool>> AssignPermissionsToRoleAsync(Guid roleId, List<SimplifiedPermissionDTO> permissions)
        {
            try
            {
                var currentTenantId = _userContext.CurrentUser?.TenantId;
                if (currentTenantId.HasValue && currentTenantId.Value != Guid.Empty)
                {
                    var role = await _roleRepository.GetByIdAsync(roleId);
                    if (role == null || role.TenantId != currentTenantId.Value)
                        return StaticResponseBuilder<bool>.BuildError("Role não encontrada");
                }

                var result = await AssignPermissionsInternalAsync(roleId, permissions);
                return StaticResponseBuilder<bool>.BuildOk(result);
            }
            catch (Exception ex)
            {
                return StaticResponseBuilder<bool>.BuildErrorResponse(ex);
            }
        }

        public async Task<ResponseDTO<bool>> RemovePermissionsFromRoleAsync(Guid roleId, List<string> moduleKeys)
        {
            try
            {
                var currentTenantId = _userContext.CurrentUser?.TenantId;
                if (currentTenantId.HasValue && currentTenantId.Value != Guid.Empty)
                {
                    var role = await _roleRepository.GetByIdAsync(roleId);
                    if (role == null || role.TenantId != currentTenantId.Value)
                        return StaticResponseBuilder<bool>.BuildError("Role não encontrada");
                }

                var removed = await _rolePermissionRepository.RemoveByRoleAndModulesAsync(roleId, moduleKeys);
                return StaticResponseBuilder<bool>.BuildOk(removed);
            }
            catch (Exception ex)
            {
                return StaticResponseBuilder<bool>.BuildErrorResponse(ex);
            }
        }

        public async Task<ResponseDTO<IEnumerable<AccessGroupDTO>>> GetAccessGroupsByRoleAsync(Guid roleId)
        {
            try
            {
                var currentTenantId = _userContext.CurrentUser?.TenantId;
                if (currentTenantId.HasValue && currentTenantId.Value != Guid.Empty)
                {
                    var role = await _roleRepository.GetByIdAsync(roleId);
                    if (role == null || role.TenantId != currentTenantId.Value)
                        return StaticResponseBuilder<IEnumerable<AccessGroupDTO>>.BuildOk([]);
                }

                var relations = await _roleAccessGroupRepository.GetByRoleIdAsync(roleId);

                var groups = relations.Where(r => r.IsActive && r.AccessGroup != null).Select(r => r.AccessGroup!);
                if (!groups.Any())
                    return StaticResponseBuilder<IEnumerable<AccessGroupDTO>>.BuildOk([]);

                var dtos = _mapper.Map<IEnumerable<AccessGroupDTO>>(groups);
                return StaticResponseBuilder<IEnumerable<AccessGroupDTO>>.BuildOk(dtos);
            }
            catch (Exception ex)
            {
                return StaticResponseBuilder<IEnumerable<AccessGroupDTO>>.BuildErrorResponse(ex);
            }
        }

        public async Task<ResponseDTO<bool>> AssignAccessGroupsToRoleAsync(Guid roleId, List<Guid> accessGroupIds)
        {
            try
            {
                var currentTenantId = _userContext.CurrentUser?.TenantId;
                if (currentTenantId.HasValue && currentTenantId.Value != Guid.Empty)
                {
                    var role = await _roleRepository.GetByIdAsync(roleId);
                    if (role == null || role.TenantId != currentTenantId.Value)
                        return StaticResponseBuilder<bool>.BuildError("Role não encontrada");
                }

                var result = await AssignAccessGroupsInternalAsync(roleId, accessGroupIds);
                return StaticResponseBuilder<bool>.BuildOk(result);
            }
            catch (Exception ex)
            {
                return StaticResponseBuilder<bool>.BuildErrorResponse(ex);
            }
        }

        public async Task<ResponseDTO<bool>> RemoveAccessGroupsFromRoleAsync(Guid roleId, List<Guid> accessGroupIds)
        {
            try
            {
                var currentTenantId = _userContext.CurrentUser?.TenantId;
                if (currentTenantId.HasValue && currentTenantId.Value != Guid.Empty)
                {
                    var role = await _roleRepository.GetByIdAsync(roleId);
                    if (role == null || role.TenantId != currentTenantId.Value)
                        return StaticResponseBuilder<bool>.BuildError("Role não encontrada");
                }

                var removed = await _roleAccessGroupRepository.RemoveByRoleAndGroupsAsync(roleId, accessGroupIds);
                return StaticResponseBuilder<bool>.BuildOk(removed);
            }
            catch (Exception ex)
            {
                return StaticResponseBuilder<bool>.BuildErrorResponse(ex);
            }
        }

        private async Task<bool> AssignPermissionsInternalAsync(Guid roleId, List<SimplifiedPermissionDTO> permissions)
        {
            try
            {
                var role = await _roleRepository.GetByIdAsync(roleId);
                if (role == null)
                    throw new ArgumentException("Role não encontrada");

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
            catch (Exception)
            {
                return false;
            }
        }

        private async Task<bool> AssignAccessGroupsInternalAsync(Guid roleId, List<Guid> accessGroupIds)
        {
            try
            {
                var role = await _roleRepository.GetByIdAsync(roleId) ?? throw new ArgumentException("Role não encontrada");
                foreach (var groupId in accessGroupIds)
                {
                    var group = await _accessGroupRepository.GetByIdAsync(groupId) ?? throw new ArgumentException($"Grupo de acesso com ID {groupId} não encontrado");
                }

                var existing = await _roleAccessGroupRepository.GetByRoleIdAsync(roleId);
                var existingIds = existing.Where(rag => rag.IsActive).Select(rag => rag.AccessGroupId).ToList();
                var newIds = accessGroupIds.Except(existingIds).ToList();

                var relations = new List<RoleAccessGroupEntity>();
                foreach (var gid in newIds)
                {
                    relations.Add(new RoleAccessGroupEntity
                    {
                        Id = Guid.NewGuid(),
                        RoleId = roleId,
                        AccessGroupId = gid,
                        IsActive = true,
                        CreatedAt = DateTime.Now
                    });
                }

                if (relations.Count > 0)
                    await _roleAccessGroupRepository.AddRangeAsync(relations);

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
            
        }

        private async Task<ResponseDTO<bool>> SyncRolePermissionsInternalAsync(Guid roleId, List<SimplifiedPermissionDTO> permissions)
        {
            try
            {
                var existing = await _rolePermissionRepository.GetAllRolePermissionsByRoleIdAsync(roleId);
                
                var newModuleKeys = permissions.Select(p => p.Module).ToList();
                var toRemove = existing.Where(rp => !newModuleKeys.Contains(rp.ModuleKey)).Select(rp => rp.ModuleKey).ToList();
                
                if (toRemove.Count != 0)
                    await _rolePermissionRepository.RemoveByRoleAndModulesAsync(roleId, toRemove);

                await AssignPermissionsInternalAsync(roleId, permissions);

                return StaticResponseBuilder<bool>.BuildOk(true);
            }
            catch (Exception ex)
            {
                return StaticResponseBuilder<bool>.BuildErrorResponse(ex);
            }
        }

        private async Task<ResponseDTO<bool>> SyncRoleAccessGroupsInternalAsync(Guid roleId, List<Guid> accessGroupIds)
        {
            try
            {
                var existing = await _roleAccessGroupRepository.GetByRoleIdAsync(roleId);
                var toAdd = accessGroupIds.Except(existing.Where(rag => rag.IsActive).Select(rag => rag.AccessGroupId)).ToList();
                var toRemove = existing.Where(rag => rag.IsActive && !accessGroupIds.Contains(rag.AccessGroupId)).Select(rag => rag.AccessGroupId).ToList();

                if (toAdd.Any())
                    await AssignAccessGroupsInternalAsync(roleId, toAdd);
                if (toRemove.Any())
                    await _roleAccessGroupRepository.RemoveByRoleAndGroupsAsync(roleId, toRemove);

                return StaticResponseBuilder<bool>.BuildOk(true);
            }
            catch (Exception ex)
            {
                return StaticResponseBuilder<bool>.BuildErrorResponse(ex);
            }
            
        }

        public async Task<ResponseDTO<RoleDTO>> ToggleStatus(Guid id, RoleUpdateDTO dto)
        {
            try
            {
                var roleEntity = await _roleRepository.GetByIdAsync(id, include: r => r
                .Include(x => x.RolePermissions)
                .Include(x => x.RoleAccessGroups)!.ThenInclude(rag => rag.AccessGroup));

                if (roleEntity == null)
                    return StaticResponseBuilder<RoleDTO>.BuildError("Role não encontrada");

                roleEntity.IsActive = dto.IsActive;
                roleEntity.UpdatedAt = DateTime.Now;
                await _roleRepository.UpdateAsync(roleEntity);
                var resultDto = _mapper.Map<RoleDTO>(roleEntity);
                return StaticResponseBuilder<RoleDTO>.BuildOk(resultDto);
            }
            catch (Exception ex)
            {
                return StaticResponseBuilder<RoleDTO>.BuildErrorResponse(ex);
            }
            
        }
    }
}

