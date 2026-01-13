using OpaMenu.Infrastructure.Shared.Entities.MultiTenant.TenantProduct;

namespace Authenticator.API.Core.Application.Interfaces.MultiTenant
{
    public interface ITenantProductRepository : IBaseRepository<TenantProductEntity>
    {
        Task<TenantProductEntity?> GetDefaultProductAsync();
    }
}

