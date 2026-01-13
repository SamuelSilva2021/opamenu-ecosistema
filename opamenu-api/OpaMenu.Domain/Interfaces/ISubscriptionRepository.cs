using OpaMenu.Infrastructure.Shared.Entities.MultiTenant.Subscription;

namespace OpaMenu.Domain.Interfaces;

public interface ISubscriptionRepository
{
    Task<SubscriptionEntity?> GetActiveSubscriptionAsync(Guid tenantId);
    Task UpdateAsync(SubscriptionEntity subscription);
}

