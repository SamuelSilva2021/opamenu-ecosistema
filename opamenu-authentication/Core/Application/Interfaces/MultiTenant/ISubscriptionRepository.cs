using OpaMenu.Infrastructure.Shared.Entities.MultiTenant.Subscription;

namespace Authenticator.API.Core.Application.Interfaces.MultiTenant
{
    public interface ISubscriptionRepository : IBaseRepository<SubscriptionEntity>
    {
        Task<SubscriptionEntity?> GetActiveByTenantIdAsync(Guid tenantId);
        Task<SubscriptionEntity?> GetByTenantIdAsync(Guid tenantId);
    }
}

