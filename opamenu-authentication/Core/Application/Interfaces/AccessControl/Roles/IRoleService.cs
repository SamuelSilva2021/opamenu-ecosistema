using Authenticator.API.Core.Domain.AccessControl.Roles.DTOs;
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
        Task<ResponseDTO<RoleDTO>> ToggleStatus(Guid id, RoleUpdateDTO dto);
        Task<ResponseDTO<bool>> DeleteRoleAsync(Guid id);
        Task<ResponseDTO<IEnumerable<SimplifiedPermissionDTO>>> GetPermissionsByRoleAsync(Guid roleId);
        Task<ResponseDTO<bool>> AssignPermissionsToRoleAsync(Guid roleId, List<SimplifiedPermissionDTO> permissions);
        Task<ResponseDTO<bool>> RemovePermissionsFromRoleAsync(Guid roleId, List<string> moduleKeys);
        Task<ResponseDTO<IEnumerable<AccessGroupDTO>>> GetAccessGroupsByRoleAsync(Guid roleId);
        Task<ResponseDTO<bool>> AssignAccessGroupsToRoleAsync(Guid roleId, List<Guid> accessGroupIds);
        Task<ResponseDTO<bool>> RemoveAccessGroupsFromRoleAsync(Guid roleId, List<Guid> accessGroupIds);
    }
}