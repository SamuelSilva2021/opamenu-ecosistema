using Authenticator.API.Core.Application.Interfaces;
using Authenticator.API.Core.Application.Interfaces.MultiTenant;
using OpaMenu.Infrastructure.Shared.Entities.MultiTenant.TenantProduct;

namespace Authenticator.API.Infrastructure.Repositories.MultiTenant
{
    public class TenantProductRepository(IDbContextProvider dbContextProvider) : BaseRepository<TenantProductEntity>(dbContextProvider), ITenantProductRepository
    {
        public async Task<TenantProductEntity?> GetDefaultProductAsync() =>
            await FirstOrDefaultAsync(p => p.Status == EProductStatus.Ativo);
    }
}

