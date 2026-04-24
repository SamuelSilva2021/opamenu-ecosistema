using OpaMenu.Commons.Api.DTOs;
using OpaMenu.Domain.DTOs.AccessControl;

namespace OpaMenu.Application.Services.Interfaces.AccessControl;

public interface IRoleService
{
    Task<PagedResultDto<RoleDto>> GetRolesAsync(int page, int limit, string? search);
    Task<RoleDto?> GetByIdAsync(Guid id);
    Task<RoleDto?> CreateAsync(CreateRoleRequestDto request);
    Task<(RoleDto? Role, bool NotFound)> UpdateAsync(Guid id, UpdateRoleRequestDto request);
    Task<bool> DeleteAsync(Guid id);

    Task<ResponseDTO<PagedResultDto<RolePainelDto>>> GetRolesPainelAsync(Guid tenantId, int page, int limit, string? search);
    Task<ResponseDTO<RolePainelDto>> GetRolePainelByIdAsync(Guid tenantId, Guid id);
    Task<ResponseDTO<List<AvailableModuleDto>>> GetAvailableModulesPainelAsync(Guid tenantId);
    Task<ResponseDTO<RolePainelDto>> CreateRolePainelAsync(Guid tenantId, CreateRoleRequestDto request);
    Task<ResponseDTO<RolePainelDto>> UpdateRolePainelAsync(Guid tenantId, Guid id, UpdateRoleRequestDto request);
    Task<ResponseDTO<bool>> DeleteRolePainelAsync(Guid tenantId, Guid id);
}

