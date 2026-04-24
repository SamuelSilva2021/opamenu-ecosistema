using OpaMenu.Infrastructure.Shared.Entities.MultiTenant.Subscription;

namespace OpaMenu.Domain.Interfaces;

public interface ISubscriptionRepository
{
    Task<SubscriptionEntity?> GetActiveSubscriptionAsync(Guid tenantId);
    Task<SubscriptionEntity?> GetByTenantIdWithPlanAndTenantAsync(Guid tenantId);
    Task<SubscriptionEntity?> GetByTenantIdTrackedAsync(Guid tenantId);
    Task AddAsync(SubscriptionEntity entity);
    Task SaveChangesAsync();
    Task UpdateAsync(SubscriptionEntity subscription);
}

