using Authenticator.API.Core.Application.Interfaces.AccessControl.Roles;
using Authenticator.API.Core.Application.Interfaces.AccessControl.RolePermissions;
using Authenticator.API.Core.Application.Interfaces.AccessControl.RoleAccessGroups;
using Authenticator.API.Core.Application.Interfaces.AccessControl.Permissions;
using Authenticator.API.Core.Application.Interfaces.AccessControl.AccessGroup;
using OpaMenu.Infrastructure.Shared.Entities.AccessControl;
using Authenticator.API.Core.Domain.AccessControl.Roles.DTOs;
using Authenticator.API.Core.Domain.AccessControl.Permissions.DTOs;
using Authenticator.API.Core.Domain.AccessControl.AccessGroup.DTOs;
using Authenticator.API.Core.Domain.Api;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace Authenticator.API.Core.Application.Implementation.AccessControl.Roles
{
    /// <summary>
    /// ServiÃ§o para gerenciar Roles e seus relacionamentos
    /// </summary>
    public class RoleService(
        IRoleRepository roleRepository,
        IPermissionRepository permissionRepository,
        IAccessGroupRepository accessGroupRepository,
        IRolePermissionRepository rolePermissionRepository,
        IRoleAccessGroupRepository roleAccessGroupRepository,
        IMapper mapper
    ) : IRoleService
    {
        private readonly IRoleRepository _roleRepository = roleRepository;
        private readonly IPermissionRepository _permissionRepository = permissionRepository;
        private readonly IAccessGroupRepository _accessGroupRepository = accessGroupRepository;
        private readonly IRolePermissionRepository _rolePermissionRepository = rolePermissionRepository;
        private readonly IRoleAccessGroupRepository _roleAccessGroupRepository = roleAccessGroupRepository;
        private readonly IMapper _mapper = mapper;

        public async Task<ResponseDTO<IEnumerable<RoleDTO>>> GetAllRolesAsync()
        {
            try
            {
                var entities = await _roleRepository.GetAllAsync(include: r => r
                    .Include(x => x.RolePermissions)!.ThenInclude(rp => rp.Permission)
                    .Include(x => x.RoleAccessGroups)!.ThenInclude(rag => rag.AccessGroup));

                var dtos = _mapper.Map<IEnumerable<RoleDTO>>(entities);
                return ResponseBuilder<IEnumerable<RoleDTO>>.Ok(dtos).Build();
            }
            catch (Exception ex)
            {
                return ResponseBuilder<IEnumerable<RoleDTO>>
                    .Fail(new ErrorDTO { Message = ex.Message })
                    .WithException(ex)
                    .WithCode(500)
                    .Build();
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
                        .Include(x => x.RolePermissions)!.ThenInclude(rp => rp.Permission)
                        .Include(x => x.RoleAccessGroups)!.ThenInclude(rag => rag.AccessGroup));

                var dtos = _mapper.Map<IEnumerable<RoleDTO>>(entities);
                var paged = new PagedResponseDTO<RoleDTO>
                {
                    Items = dtos,
                    Page = page,
                    Limit = limit,
                    Total = entities.Count(),
                    TotalPages = (int)Math.Ceiling((double)entities.Count() / limit)
                };
                return ResponseBuilder<PagedResponseDTO<RoleDTO>>.Ok(paged).Build();
            }
            catch (Exception ex)
            {
                return ResponseBuilder<PagedResponseDTO<RoleDTO>>
                    .Fail(new ErrorDTO { Message = ex.Message })
                    .WithException(ex)
                    .WithCode(500)
                    .Build();
            }
        }

        public async Task<ResponseDTO<RoleDTO>> GetRoleByIdAsync(Guid id)
        {
            try
            {
                var entity = await _roleRepository.GetByIdAsync(id, include: r => r
                    .Include(x => x.RolePermissions)!.ThenInclude(rp => rp.Permission)
                    .Include(x => x.RoleAccessGroups)!.ThenInclude(rag => rag.AccessGroup));

                if (entity == null)
                    return ResponseBuilder<RoleDTO>
                        .Fail(new ErrorDTO { Message = "Role nÃ£o encontrada" })
                        .WithCode(404)
                        .Build();

                var dto = _mapper.Map<RoleDTO>(entity);
                return ResponseBuilder<RoleDTO>.Ok(dto).Build();
            }
            catch (Exception ex)
            {
                return ResponseBuilder<RoleDTO>
                    .Fail(new ErrorDTO { Message = ex.Message })
                    .WithException(ex)
                    .WithCode(500)
                    .Build();
            }
        }

        public async Task<ResponseDTO<IEnumerable<RoleDTO>>> GetRolesByTenantAsync(Guid tenantId)
        {
            try
            {
                // Utiliza mÃ©todo existente no repositÃ³rio para obter roles por tenant
                var entities = await _roleRepository.GetAllByTenantAsync(tenantId);

                // Carregar includes manualmente se necessÃ¡rio
                var entitiesWithIncludes = new List<RoleEntity>();
                foreach (var role in entities)
                {
                    var full = await _roleRepository.GetByIdAsync(role.Id, include: r => r
                        .Include(x => x.RolePermissions)!.ThenInclude(rp => rp.Permission)
                        .Include(x => x.RoleAccessGroups)!.ThenInclude(rag => rag.AccessGroup));
                    if (full != null)
                        entitiesWithIncludes.Add(full);
                }

                var dtos = _mapper.Map<IEnumerable<RoleDTO>>(entitiesWithIncludes);
                return ResponseBuilder<IEnumerable<RoleDTO>>.Ok(dtos).Build();
            }
            catch (Exception ex)
            {
                return ResponseBuilder<IEnumerable<RoleDTO>>
                    .Fail(new ErrorDTO { Message = ex.Message })
                    .WithException(ex)
                    .WithCode(500)
                    .Build();
            }
        }

        public async Task<ResponseDTO<RoleDTO>> AddRoleAsync(RoleCreateDTO dto)
        {
            try
            {
                var entity = _mapper.Map<RoleEntity>(dto);
                entity.CreatedAt = DateTime.Now;

                var created = await _roleRepository.AddAsync(entity);

                // Associar permissÃµes
                if (dto.PermissionIds != null && dto.PermissionIds.Any())
                    await AssignPermissionsInternalAsync(created.Id, dto.PermissionIds);

                // Associar grupos de acesso
                if (dto.AccessGroupIds != null && dto.AccessGroupIds.Any())
                    await AssignAccessGroupsInternalAsync(created.Id, dto.AccessGroupIds);

                var full = await _roleRepository.GetByIdAsync(created.Id, include: r => r
                    .Include(x => x.RolePermissions)!.ThenInclude(rp => rp.Permission)
                    .Include(x => x.RoleAccessGroups)!.ThenInclude(rag => rag.AccessGroup));

                var resultDto = _mapper.Map<RoleDTO>(full);
                return ResponseBuilder<RoleDTO>.Ok(resultDto).WithCode(201).Build();
            }
            catch (Exception ex)
            {
                return ResponseBuilder<RoleDTO>
                    .Fail(new ErrorDTO { Message = ex.Message })
                    .WithException(ex)
                    .WithCode(500)
                    .Build();
            }
        }

        public async Task<ResponseDTO<RoleDTO>> UpdateRoleAsync(Guid id, RoleUpdateDTO dto)
        {
            try
            {
                var existing = await _roleRepository.GetByIdAsync(id, include: r => r
                    .Include(x => x.RolePermissions)!.ThenInclude(rp => rp.Permission)
                    .Include(x => x.RoleAccessGroups)!.ThenInclude(rag => rag.AccessGroup));

                if (existing == null)
                    return ResponseBuilder<RoleDTO>
                        .Fail(new ErrorDTO { Message = "Role nÃ£o encontrada" })
                        .WithCode(404)
                        .Build();

                _mapper.Map(dto, existing);
                existing.UpdatedAt = DateTime.Now;
                await _roleRepository.UpdateAsync(existing);

                // Sincronizar permissÃµes, se lista fornecida
                if (dto.PermissionIds != null)
                    await SyncRolePermissionsInternalAsync(id, dto.PermissionIds);

                // Sincronizar grupos de acesso, se lista fornecida
                if (dto.AccessGroupIds != null)
                    await SyncRoleAccessGroupsInternalAsync(id, dto.AccessGroupIds);

                var resultDto = _mapper.Map<RoleDTO>(existing);
                return ResponseBuilder<RoleDTO>.Ok(resultDto).Build();
            }
            catch (Exception ex)
            {
                return ResponseBuilder<RoleDTO>
                    .Fail(new ErrorDTO { Message = ex.Message })
                    .WithException(ex)
                    .WithCode(500)
                    .Build();
            }
        }

        public async Task<ResponseDTO<bool>> DeleteRoleAsync(Guid id)
        {
            try
            {
                var existing = await _roleRepository.GetByIdAsync(id);
                if (existing == null)
                    return ResponseBuilder<bool>
                        .Fail(new ErrorDTO { Message = "Role nÃ£o encontrada" })
                        .WithCode(404)
                        .Build();

                // Remover relaÃ§Ãµes (soft delete) antes de excluir a role
                await _rolePermissionRepository.RemoveAllByRoleIdAsync(id);
                await _roleAccessGroupRepository.RemoveAllByRoleIdAsync(id);

                await _roleRepository.DeleteAsync(existing);
                return ResponseBuilder<bool>.Ok(true).Build();
            }
            catch (Exception ex)
            {
                return ResponseBuilder<bool>
                    .Fail(new ErrorDTO { Message = ex.Message })
                    .WithException(ex)
                    .WithCode(500)
                    .Build();
            }
        }

        public async Task<ResponseDTO<IEnumerable<PermissionDTO>>> GetPermissionsByRoleAsync(Guid roleId)
        {
            try
            {
                var relations = await _rolePermissionRepository.GetAllRolePermissionsByRoleIdAsync(roleId);
                var permissionIds = relations
                    .Where(rp => rp.IsActive)
                    .Select(rp => rp.PermissionId)
                    .Distinct()
                    .ToList();

                IEnumerable<PermissionEntity> permissions = Enumerable.Empty<PermissionEntity>();
                if (permissionIds.Any())
                {
                    permissions = await _permissionRepository.GetAllAsync(
                        filter: p => permissionIds.Contains(p.Id),
                        include: p => p
                            .Include(x => x.Module)
                            .Include(x => x.PermissionOperations)!.ThenInclude(po => po.Operation));
                }

                var dtos = _mapper.Map<IEnumerable<PermissionDTO>>(permissions);
                return ResponseBuilder<IEnumerable<PermissionDTO>>.Ok(dtos).Build();
            }
            catch (Exception ex)
            {
                return ResponseBuilder<IEnumerable<PermissionDTO>>
                    .Fail(new ErrorDTO { Message = ex.Message })
                    .WithException(ex)
                    .WithCode(500)
                    .Build();
            }
        }

        public async Task<ResponseDTO<bool>> AssignPermissionsToRoleAsync(Guid roleId, List<Guid> permissionIds)
        {
            try
            {
                var result = await AssignPermissionsInternalAsync(roleId, permissionIds);
                return ResponseBuilder<bool>.Ok(result).Build();
            }
            catch (Exception ex)
            {
                return ResponseBuilder<bool>
                    .Fail(new ErrorDTO { Message = ex.Message })
                    .WithException(ex)
                    .WithCode(500)
                    .Build();
            }
        }

        public async Task<ResponseDTO<bool>> RemovePermissionsFromRoleAsync(Guid roleId, List<Guid> permissionIds)
        {
            try
            {
                var removed = await _rolePermissionRepository.RemoveByRoleAndPermissionsAsync(roleId, permissionIds);
                return ResponseBuilder<bool>.Ok(removed).Build();
            }
            catch (Exception ex)
            {
                return ResponseBuilder<bool>
                    .Fail(new ErrorDTO { Message = ex.Message })
                    .WithException(ex)
                    .WithCode(500)
                    .Build();
            }
        }

        public async Task<ResponseDTO<IEnumerable<AccessGroupDTO>>> GetAccessGroupsByRoleAsync(Guid roleId)
        {
            try
            {
                var relations = await _roleAccessGroupRepository.GetByRoleIdAsync(roleId);
                var groups = relations.Where(r => r.IsActive && r.AccessGroup != null).Select(r => r.AccessGroup!);
                var dtos = _mapper.Map<IEnumerable<AccessGroupDTO>>(groups);
                return ResponseBuilder<IEnumerable<AccessGroupDTO>>.Ok(dtos).Build();
            }
            catch (Exception ex)
            {
                return ResponseBuilder<IEnumerable<AccessGroupDTO>>
                    .Fail(new ErrorDTO { Message = ex.Message })
                    .WithException(ex)
                    .WithCode(500)
                    .Build();
            }
        }

        public async Task<ResponseDTO<bool>> AssignAccessGroupsToRoleAsync(Guid roleId, List<Guid> accessGroupIds)
        {
            try
            {
                var result = await AssignAccessGroupsInternalAsync(roleId, accessGroupIds);
                return ResponseBuilder<bool>.Ok(result).Build();
            }
            catch (Exception ex)
            {
                return ResponseBuilder<bool>
                    .Fail(new ErrorDTO { Message = ex.Message })
                    .WithException(ex)
                    .WithCode(500)
                    .Build();
            }
        }

        public async Task<ResponseDTO<bool>> RemoveAccessGroupsFromRoleAsync(Guid roleId, List<Guid> accessGroupIds)
        {
            try
            {
                var removed = await _roleAccessGroupRepository.RemoveByRoleAndGroupsAsync(roleId, accessGroupIds);
                return ResponseBuilder<bool>.Ok(removed).Build();
            }
            catch (Exception ex)
            {
                return ResponseBuilder<bool>
                    .Fail(new ErrorDTO { Message = ex.Message })
                    .WithException(ex)
                    .WithCode(500)
                    .Build();
            }
        }

        private async Task<bool> AssignPermissionsInternalAsync(Guid roleId, List<Guid> permissionIds)
        {
            var role = await _roleRepository.GetByIdAsync(roleId);
            if (role == null)
                throw new ArgumentException("Role nÃ£o encontrada");

            foreach (var permissionId in permissionIds)
            {
                var permission = await _permissionRepository.GetByIdAsync(permissionId) ?? 
                    throw new ArgumentException($"PermissÃ£o com ID {permissionId} nÃ£o encontrada");
            }

            var existing = await _rolePermissionRepository.GetAllRolePermissionsByRoleIdAsync(roleId);
            var existingIdsActive = existing.Where(rp => rp.IsActive).Select(rp => rp.PermissionId).ToList();
            var existingInactive = existing.Where(rp => !rp.IsActive).ToList();

            // Reativar relaÃ§Ãµes inativas caso existam
            if (existingInactive.Any())
            {
                foreach (var rel in existingInactive)
                {
                    if (permissionIds.Contains(rel.PermissionId))
                    {
                        rel.IsActive = true;
                        rel.UpdatedAt = DateTime.Now;
                    }
                }
                await _rolePermissionRepository.UpdateRangeAsync(existingInactive);
            }

            var newIds = permissionIds.Except(existingIdsActive).Except(existingInactive.Select(p => p.PermissionId)).ToList();

            var relations = new List<RolePermissionEntity>();
            foreach (var pid in newIds)
            {
                relations.Add(new RolePermissionEntity
                {
                    Id = Guid.NewGuid(),
                    RoleId = roleId,
                    PermissionId = pid,
                    IsActive = true,
                    CreatedAt = DateTime.Now
                });
            }

            if (relations.Count > 0)
                await _rolePermissionRepository.AddRangeAsync(relations);

            return true;
        }

        private async Task<bool> AssignAccessGroupsInternalAsync(Guid roleId, List<Guid> accessGroupIds)
        {
            var role = await _roleRepository.GetByIdAsync(roleId);
            if (role == null)
                throw new ArgumentException("Role nÃ£o encontrada");

            foreach (var groupId in accessGroupIds)
            {
                var group = await _accessGroupRepository.GetByIdAsync(groupId);
                if (group == null)
                    throw new ArgumentException($"Grupo de acesso com ID {groupId} nÃ£o encontrado");
            }

            var existing = await _roleAccessGroupRepository.GetByRoleIdAsync(roleId);
            var existingIds = existing.Where(rag => rag.IsActive).Select(rag => rag.AccessGroupId).ToList();
            var newIds = accessGroupIds.Except(existingIds).ToList();

            // Garanta que estamos usando a entidade correta do namespace RoleAccessGroups
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

        private async Task<bool> SyncRolePermissionsInternalAsync(Guid roleId, List<Guid> permissionIds)
        {
            var existing = await _rolePermissionRepository.GetAllRolePermissionsByRoleIdAsync(roleId);
            var toAdd = permissionIds.Except(existing.Where(rp => rp.IsActive).Select(rp => rp.PermissionId)).ToList();
            var toRemove = existing.Where(rp => rp.IsActive && !permissionIds.Contains(rp.PermissionId)).Select(rp => rp.PermissionId).ToList();

            if (toAdd.Any())
                await AssignPermissionsInternalAsync(roleId, toAdd);
            if (toRemove.Any())
                await _rolePermissionRepository.RemoveByRoleAndPermissionsAsync(roleId, toRemove);

            return true;
        }

        private async Task<bool> SyncRoleAccessGroupsInternalAsync(Guid roleId, List<Guid> accessGroupIds)
        {
            var existing = await _roleAccessGroupRepository.GetByRoleIdAsync(roleId);
            var toAdd = accessGroupIds.Except(existing.Where(rag => rag.IsActive).Select(rag => rag.AccessGroupId)).ToList();
            var toRemove = existing.Where(rag => rag.IsActive && !accessGroupIds.Contains(rag.AccessGroupId)).Select(rag => rag.AccessGroupId).ToList();

            if (toAdd.Any())
                await AssignAccessGroupsInternalAsync(roleId, toAdd);
            if (toRemove.Any())
                // Ajuste para utilizar o mÃ©todo existente no repositÃ³rio
                await _roleAccessGroupRepository.RemoveByRoleAndGroupsAsync(roleId, toRemove);

            return true;
        }
    }
}

