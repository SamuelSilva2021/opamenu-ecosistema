using Opamenu.Infrastructure.Shared.Entities.MultiTenant.TenantProduct;

namespace Opamenu.Domain.Interfaces;

public interface IMultiTenantProductRepository
{
    Task<TenantProductEntity?> GetFirstAsync();
}
