using OpaMenu.Infrastructure.Shared.Entities;
using OpaMenu.Infrastructure.Shared.Entities.MultiTenant.Tenant;

namespace OpaMenu.Domain.Interfaces;

public interface ITenantRepository
{
    Task<TenantEntity?> GetBySlugAsync(string slug);
    Task<TenantEntity?> GetBySlugWithBusinessInfoAsync(string slug);
    Task<Guid> GetTenantIdBySlugAsyn(string slug);
    Task<TenantEntity?> GetByIdAsync(Guid id);
    Task UpdateAsync(TenantEntity entity);
}

