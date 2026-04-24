using OpaMenu.Domain.DTOs.AccessControl;

namespace OpaMenu.Application.Services.Interfaces.AccessControl;

public interface IPermissionService
{
    Task<PagedResultDto<PermissionDto>> GetPermissionsAsync(int page, int limit, string? search, Guid? moduleId, Guid? tenantId);
    Task<PermissionDto?> GetPermissionByIdAsync(Guid id);
    Task<PermissionDto?> CreateAsync(CreatePermissionRequestDto request);
    Task<(PermissionDto? Permission, bool NotFound)> UpdateAsync(Guid id, UpdatePermissionRequestDto request);
    Task DeleteAsync(Guid id);
}

