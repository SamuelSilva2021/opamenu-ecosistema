using OpaMenu.Infrastructure.Shared.Entities.MultiTenant.TenantModule;

namespace OpaMenu.Domain.Interfaces;

public interface ITenantModuleRepository
{
    Task<IReadOnlyList<Guid>> GetEnabledModuleIdsAsync(Guid tenantId);
    Task<List<TenantModuleEntity>> GetByTenantTrackedAsync(Guid tenantId);
    Task<TenantModuleEntity?> GetByTenantAndModuleTrackedAsync(Guid tenantId, Guid moduleId);
    Task AddAsync(TenantModuleEntity entity);
    Task AddRangeAsync(IEnumerable<TenantModuleEntity> entities);
    Task RemoveRangeAsync(IEnumerable<TenantModuleEntity> entities);
    Task SaveChangesAsync();
}
