using Authenticator.API.Core.Application.Interfaces;
using Authenticator.API.Core.Application.Interfaces.MultiTenant;
using OpaMenu.Infrastructure.Shared.Entities.MultiTenant.Subscription;

namespace Authenticator.API.Infrastructure.Repositories.MultiTenant
{
    public class SubscriptionRepository(IDbContextProvider dbContextProvider) : BaseRepository<SubscriptionEntity>(dbContextProvider), ISubscriptionRepository
    {
        public async Task<SubscriptionEntity?> GetActiveByTenantIdAsync(Guid tenantId)
        {
            return await FirstOrDefaultAsync(s => s.TenantId == tenantId && (s.Status == ESubscriptionStatus.Ativo || s.Status == ESubscriptionStatus.Trial));
        }

        public async Task<SubscriptionEntity?> GetByTenantIdAsync(Guid tenantId)
        {
            return await FirstOrDefaultAsync(s => s.TenantId == tenantId);
        }
    }
}

