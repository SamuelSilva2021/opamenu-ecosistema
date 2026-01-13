using Authenticator.API.Core.Domain.AccessControl.Roles.DTOs;
using Authenticator.API.Core.Domain.AccessControl.Permissions.DTOs;
using Authenticator.API.Core.Domain.AccessControl.AccessGroup.DTOs;
using Authenticator.API.Core.Domain.Api;

namespace Authenticator.API.Core.Application.Interfaces.AccessControl.Roles
{
    /// <summary>
    /// Interface para o serviço de roles
    /// </summary>
    public interface IRoleService
    {
        Task<ResponseDTO<IEnumerable<RoleDTO>>> GetAllRolesAsync();
        Task<ResponseDTO<PagedResponseDTO<RoleDTO>>> GetAllRolesPagedAsync(int page, int limit);
        Task<ResponseDTO<RoleDTO>> GetRoleByIdAsync(Guid id);
        Task<ResponseDTO<IEnumerable<RoleDTO>>> GetRolesByTenantAsync(Guid tenantId);
        Task<ResponseDTO<RoleDTO>> AddRoleAsync(RoleCreateDTO dto);
        Task<ResponseDTO<RoleDTO>> UpdateRoleAsync(Guid id, RoleUpdateDTO dto);
        Task<ResponseDTO<bool>> DeleteRoleAsync(Guid id);
        Task<ResponseDTO<IEnumerable<PermissionDTO>>> GetPermissionsByRoleAsync(Guid roleId);
        Task<ResponseDTO<bool>> AssignPermissionsToRoleAsync(Guid roleId, List<Guid> permissionIds);
        Task<ResponseDTO<bool>> RemovePermissionsFromRoleAsync(Guid roleId, List<Guid> permissionIds);
        Task<ResponseDTO<IEnumerable<AccessGroupDTO>>> GetAccessGroupsByRoleAsync(Guid roleId);
        Task<ResponseDTO<bool>> AssignAccessGroupsToRoleAsync(Guid roleId, List<Guid> accessGroupIds);
        Task<ResponseDTO<bool>> RemoveAccessGroupsFromRoleAsync(Guid roleId, List<Guid> accessGroupIds);
    }
}