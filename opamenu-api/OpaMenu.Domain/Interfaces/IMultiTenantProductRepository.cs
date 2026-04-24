using OpaMenu.Infrastructure.Shared.Entities.MultiTenant.TenantProduct;

namespace OpaMenu.Domain.Interfaces;

public interface IMultiTenantProductRepository
{
    Task<TenantProductEntity?> GetFirstAsync();
}
