using OpaMenu.Infrastructure.Shared.Entities.AccessControl;

namespace OpaMenu.Domain.Interfaces;

public interface IRoleRepository
{
    Task<(IReadOnlyList<RoleEntity> Items, int Total)> GetPagedAsync(int page, int limit, string? search);
    Task<(IReadOnlyList<RoleEntity> Items, int Total)> GetPagedForTenantAsync(Guid tenantId, int page, int limit, string? search);
    Task<RoleEntity?> GetByIdAsync(Guid id);
    Task<RoleEntity?> GetByIdForTenantAsync(Guid tenantId, Guid id);
    Task<RoleEntity?> GetByIdTrackedAsync(Guid id);
    Task<RoleEntity?> GetByIdForTenantTrackedAsync(Guid tenantId, Guid id);
    Task<bool> CodeExistsAsync(string code, Guid? excludeRoleId = null, Guid? tenantId = null, bool includeNullTenant = true);
    Task AddAsync(RoleEntity role);
    Task UpdateAsync(RoleEntity role);
    Task DeleteAsync(RoleEntity role);
    Task SaveChangesAsync();
}
