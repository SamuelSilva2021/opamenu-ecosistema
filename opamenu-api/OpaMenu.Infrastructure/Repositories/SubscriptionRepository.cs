using Microsoft.EntityFrameworkCore;
using OpaMenu.Domain.Interfaces;
using OpaMenu.Infrastructure.Shared.Entities.MultiTenant.Subscription;
using OpaMenu.Infrastructure.Shared.Data.Context.MultTenant;

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

    public Task<SubscriptionEntity?> GetByTenantIdWithPlanAndTenantAsync(Guid tenantId)
    {
        return _context.Set<SubscriptionEntity>()
            .AsNoTracking()
            .Include(s => s.Plan)
            .Include(s => s.Tenant)
            .OrderByDescending(s => s.UpdatedAt ?? s.CreatedAt)
            .FirstOrDefaultAsync(s => s.TenantId == tenantId);
    }

    public Task<SubscriptionEntity?> GetByTenantIdTrackedAsync(Guid tenantId)
    {
        return _context.Set<SubscriptionEntity>().FirstOrDefaultAsync(s => s.TenantId == tenantId);
    }

    public Task AddAsync(SubscriptionEntity entity)
    {
        _context.Set<SubscriptionEntity>().Add(entity);
        return Task.CompletedTask;
    }

    public Task SaveChangesAsync() => _context.SaveChangesAsync();

    public async Task UpdateAsync(SubscriptionEntity subscription)
    {
        _context.Set<SubscriptionEntity>().Update(subscription);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<SubscriptionEntity>> GetAllAsync()
    {
        return await _context.Set<SubscriptionEntity>().AsNoTracking().ToListAsync();
    }
}

