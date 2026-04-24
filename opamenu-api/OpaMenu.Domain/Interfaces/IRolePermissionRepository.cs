using OpaMenu.Infrastructure.Shared.Entities.AccessControl;

namespace OpaMenu.Domain.Interfaces;

public interface IRolePermissionRepository
{
    Task<IReadOnlyList<RolePermissionEntity>> GetActiveByRoleIdsAsync(IReadOnlyCollection<Guid> roleIds);
    Task<IReadOnlyList<RolePermissionEntity>> GetByRoleIdAsync(Guid roleId);
    Task<IReadOnlyList<RolePermissionEntity>> GetActiveByRoleIdAsync(Guid roleId);
    Task AddRangeAsync(IEnumerable<RolePermissionEntity> permissions);
    Task RemoveRangeAsync(IEnumerable<RolePermissionEntity> permissions);
    Task SaveChangesAsync();
}
