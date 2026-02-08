using Authenticator.API.Core.Domain.AccessControl.Roles.DTOs;
using Authenticator.API.Core.Domain.Api;

namespace Authenticator.API.Core.Application.Interfaces.AccessControl.Roles
{
    public interface IRolePainelService
    {
        Task<ResponseDTO<PagedResponseDTO<RoleDTO>>> GetAllRolesPagedAsync(int page, int limit, string? name = null);
        Task<ResponseDTO<RoleDTO>> GetRoleByIdAsync(Guid id);
        Task<ResponseDTO<RoleDTO>> AddRoleAsync(RoleCreateDTO dto);
        Task<ResponseDTO<RoleDTO>> UpdateRoleAsync(Guid id, RoleUpdateDTO dto);
        Task<ResponseDTO<bool>> DeleteRoleAsync(Guid id);
        Task<ResponseDTO<IEnumerable<SimplifiedModuleDTO>>> GetAvailableModulesAsync();
    }
}
