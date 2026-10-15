using Opamenu.Infrastructure.Shared.Entities.MultiTenant.Tenant;

namespace Opamenu.Domain.Interfaces;

public interface ITenantBusinessRepository
{
    Task<TenantBusinessEntity?> GetByTenantIdAsync(Guid tenantId);
    Task<TenantBusinessEntity> AddAsync(TenantBusinessEntity entity);
    Task UpdateAsync(TenantBusinessEntity entity);
}

