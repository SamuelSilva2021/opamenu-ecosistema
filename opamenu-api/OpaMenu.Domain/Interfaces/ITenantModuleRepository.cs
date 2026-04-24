using OpaMenu.Infrastructure.Shared.Entities.MultiTenant.TenantModule;

namespace OpaMenu.Domain.Interfaces;

public interface ITenantModuleRepository
{
    Task<IReadOnlyList<Guid>> GetEnabledModuleIdsAsync(Guid tenantId);
    Task<TenantModuleEntity?> GetByTenantAndModuleTrackedAsync(Guid tenantId, Guid moduleId);
    Task AddAsync(TenantModuleEntity entity);
    Task SaveChangesAsync();
}

