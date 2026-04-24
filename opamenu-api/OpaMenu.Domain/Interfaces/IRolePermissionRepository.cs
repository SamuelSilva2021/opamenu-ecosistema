using OpaMenu.Infrastructure.Shared.Entities.AccessControl;

namespace OpaMenu.Domain.Interfaces;

public interface IRolePermissionRepository
{
    Task<(IReadOnlyList<RolePermissionEntity> Items, int Total)> GetPagedAsync(int page, int limit, string? search, Guid? tenantId, string? moduleKey);
    Task<IReadOnlyList<RolePermissionEntity>> GetByIdsWithRoleAsync(IReadOnlyCollection<Guid> ids);
    Task<RolePermissionEntity?> GetByIdWithRoleAsync(Guid id);
    Task<RolePermissionEntity?> GetByIdTrackedAsync(Guid id);
    Task<RolePermissionEntity?> GetByRoleIdAndModuleKeyTrackedAsync(Guid roleId, string moduleKey);
    Task<IReadOnlyList<RolePermissionEntity>> GetActiveByRoleIdsAsync(IReadOnlyCollection<Guid> roleIds);
    Task<IReadOnlyList<RolePermissionEntity>> GetByRoleIdAsync(Guid roleId);
    Task<IReadOnlyList<RolePermissionEntity>> GetActiveByRoleIdAsync(Guid roleId);
    Task AddAsync(RolePermissionEntity entity);
    Task AddRangeAsync(IEnumerable<RolePermissionEntity> permissions);
    Task DeleteAsync(RolePermissionEntity entity);
    Task RemoveRangeAsync(IEnumerable<RolePermissionEntity> permissions);
    Task SaveChangesAsync();
}
