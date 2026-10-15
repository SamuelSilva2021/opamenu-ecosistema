using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Opamenu.Domain.Interfaces;
using Opamenu.Infrastructure.Shared.Data.Context.AccessControl;
using Opamenu.Infrastructure.Shared.Data.Context.MultTenant;
using Opamenu.Infrastructure.Shared.Entities.MultiTenant.PlanModule;
using Opamenu.Infrastructure.Shared.Entities.MultiTenant.Subscription;
using Opamenu.Infrastructure.Shared.Entities.MultiTenant.TenantModule;
using System.Text.Json;

namespace Opamenu.Infrastructure.Services;

public class TenantModuleGate(
    MultiTenantDbContext multiTenantDbContext,
    AccessControlDbContext accessControlDbContext,
    IDistributedCache cache,
    IConfiguration configuration) : ITenantModuleGate
{
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(5);

    public bool Enforced => bool.TryParse(configuration["PlanEnforcement:Enabled"], out var enabled) && enabled;

    public async Task<bool> HasModuleAsync(Guid tenantId, string moduleKey)
    {
        if (!Enforced)
            return true;

        var keys = await GetModuleKeysAsync(tenantId);
        return keys.Contains(moduleKey, StringComparer.OrdinalIgnoreCase);
    }

    private async Task<List<string>> GetModuleKeysAsync(Guid tenantId)
    {
        var cacheKey = $"tenant:module-keys:{tenantId}";
        var cached = await cache.GetStringAsync(cacheKey);
        if (!string.IsNullOrEmpty(cached))
            return JsonSerializer.Deserialize<List<string>>(cached) ?? [];

        var activePlanIds = await multiTenantDbContext.Subscriptions
            .AsNoTracking()
            .Where(s => s.TenantId == tenantId &&
                        (s.Status == ESubscriptionStatus.Ativo ||
                         (s.Status == ESubscriptionStatus.Trial && s.CurrentPeriodEnd > DateTime.UtcNow)))
            .Select(s => s.PlanId)
            .ToListAsync();

        var planModuleIds = await multiTenantDbContext.Set<PlanModuleEntity>()
            .AsNoTracking()
            .Where(pm => activePlanIds.Contains(pm.PlanId))
            .Select(pm => pm.ModuleId)
            .ToListAsync();

        var tenantModuleIds = await multiTenantDbContext.Set<TenantModuleEntity>()
            .AsNoTracking()
            .Where(tm => tm.TenantId == tenantId && tm.IsEnabled)
            .Select(tm => tm.ModuleId)
            .ToListAsync();

        var enabledModuleIds = planModuleIds.Union(tenantModuleIds).Distinct().ToList();

        var keys = await accessControlDbContext.Modules
            .AsNoTracking()
            .Where(m => enabledModuleIds.Contains(m.Id) && m.IsActive && m.Key != null)
            .Select(m => m.Key!)
            .ToListAsync();

        await cache.SetStringAsync(
            cacheKey,
            JsonSerializer.Serialize(keys),
            new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = CacheDuration });

        return keys;
    }
}
