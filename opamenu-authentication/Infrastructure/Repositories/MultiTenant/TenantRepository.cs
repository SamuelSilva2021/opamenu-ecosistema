using Authenticator.API.Core.Application.Interfaces;
using Authenticator.API.Core.Application.Interfaces.MultiTenant;
using OpaMenu.Infrastructure.Shared.Entities.MultiTenant.Tenant;

namespace Authenticator.API.Infrastructure.Repositories.MultiTenant
{
    public class TenantRepository(IDbContextProvider dbContextProvider) : BaseRepository<TenantEntity>(dbContextProvider), ITenantRepository
    {
        public async Task<TenantEntity?> GetByDocumentAsync(string document) =>
            await FirstOrDefaultAsync(t => t.Document == document);

        public async Task<bool> ExistingSlug(string slug) =>
            await AnyAsync(t => t.Slug == slug);
    }
}

