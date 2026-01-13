using Microsoft.EntityFrameworkCore;
using OpaMenu.Domain.Interfaces;
using OpaMenu.Infrastructure.Shared.Entities.MultiTenant.Subscription;
using OpaMenu.Infrastructure.Shared.Data.Context;

namespace OpaMenu.Infrastructure.Repositories;

public class SubscriptionRepository(MultiTenantDbContext context) : ISubscriptionRepository
{
    private readonly MultiTenantDbContext _context = context;

    public async Task<SubscriptionEntity?> GetActiveSubscriptionAsync(Guid tenantId)
    {
        return await _context.Set<SubscriptionEntity>()
            .Include(s => s.Plan)
            .Include(s => s.Product)
            .Where(s => s.TenantId == tenantId && s.Status == ESubscriptionStatus.Ativo)
            .OrderByDescending(s => s.CurrentPeriodEnd)
            .FirstOrDefaultAsync();
    }

    public async Task UpdateAsync(SubscriptionEntity subscription)
    {
        _context.Set<SubscriptionEntity>().Update(subscription);
        await _context.SaveChangesAsync();
    }
}

