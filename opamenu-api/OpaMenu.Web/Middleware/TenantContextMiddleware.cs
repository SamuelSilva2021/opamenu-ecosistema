using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.EntityFrameworkCore;
using OpaMenu.Infrastructure.Shared.Interfaces;
using OpaMenu.Infrastructure.Shared.Data.Context.MultTenant;
using OpaMenu.Infrastructure.Shared.Data.Context.AccessControl;
using OpaMenu.Infrastructure.Shared.Entities.MultiTenant.Subscription;
using System.Security.Claims;
using System.Text.Json;

namespace OpaMenu.Web.Middleware
{
    public class TenantContextMiddleware(RequestDelegate next)
    {
        private readonly RequestDelegate _next = next;

        public async Task InvokeAsync(
            HttpContext context, 
            ITenantContext tenantContext,
            MultiTenantDbContext multiTenantDb,
            AccessControlDbContext accessControlDb,
            IDistributedCache cache)
        {
            if (context.User.Identity?.IsAuthenticated == true)
            {
                var tenantIdClaim = context.User.FindFirst("tenant_id")?.Value;
                if (Guid.TryParse(tenantIdClaim, out var tenantId))
                {
                    var tenantSlug = context.User.FindFirst("tenant_slug")?.Value;
                    tenantContext.SetTenant(tenantId, tenantSlug, null);

                    // 1. Verificar Cache para Status e Módulos
                    var cacheKey = $"tenant:context:{tenantId}";
                    var cachedData = await cache.GetStringAsync(cacheKey);

                    if (!string.IsNullOrEmpty(cachedData))
                    {
                        var info = JsonSerializer.Deserialize<TenantSubscriptionInfo>(cachedData);
                        if (info != null)
                        {
                            tenantContext.SetSubscriptionInfo(info.IsActive, info.Modules);
                        }
                    }
                    else
                    {
                        // 2. Se não estiver no cache, buscar do Banco
                        var subscription = await multiTenantDb.Subscriptions
                            .AsNoTracking()
                            .Where(s => s.TenantId == tenantId)
                            .OrderByDescending(s => s.CreatedAt)
                            .Select(s => new { s.Status })
                            .FirstOrDefaultAsync();

                        var isActive = subscription != null && 
                                     (subscription.Status == ESubscriptionStatus.Ativo || 
                                      subscription.Status == ESubscriptionStatus.Trial);

                        var enabledModuleIds = await multiTenantDb.Set<OpaMenu.Infrastructure.Shared.Entities.MultiTenant.TenantModule.TenantModuleEntity>()
                            .AsNoTracking()
                            .Where(tm => tm.TenantId == tenantId && tm.IsEnabled)
                            .Select(tm => tm.ModuleId)
                            .ToListAsync();

                        var enabledModuleKeys = await accessControlDb.Modules
                            .AsNoTracking()
                            .Where(m => enabledModuleIds.Contains(m.Id))
                            .Select(m => m.Key)
                            .Where(k => k != null)
                            .ToListAsync();

                        tenantContext.SetSubscriptionInfo(isActive, enabledModuleKeys!);

                        // 3. Salvar no Cache (15 minutos)
                        var info = new TenantSubscriptionInfo 
                        { 
                            IsActive = isActive, 
                            Modules = enabledModuleKeys! 
                        };
                        
                        await cache.SetStringAsync(
                            cacheKey, 
                            JsonSerializer.Serialize(info), 
                            new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(15) }
                        );
                    }
                }
            }

            await _next(context);
        }

        private class TenantSubscriptionInfo
        {
            public bool IsActive { get; set; }
            public List<string> Modules { get; set; } = new();
        }
    }
}
