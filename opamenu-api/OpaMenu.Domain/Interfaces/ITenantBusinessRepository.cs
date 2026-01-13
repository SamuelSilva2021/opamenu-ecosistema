using OpaMenu.Infrastructure.Shared.Entities.MultiTenant.Tenant;

namespace OpaMenu.Domain.Interfaces;

public interface ITenantBusinessRepository
{
    Task<TenantBusinessEntity?> GetByTenantIdAsync(Guid tenantId);
    Task<TenantBusinessEntity> AddAsync(TenantBusinessEntity entity);
    Task UpdateAsync(TenantBusinessEntity entity);
}

