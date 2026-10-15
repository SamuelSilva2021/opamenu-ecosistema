using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Opamenu.Infrastructure.Services;
using Opamenu.Infrastructure.Shared.Data.Context.AccessControl;
using Opamenu.Infrastructure.Shared.Data.Context.MultTenant;

namespace OpamenuApp.Tests.Services;

public class TenantModuleGateTests
{
    private static TenantModuleGate CreateGate(bool enforced)
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?> { ["PlanEnforcement:Enabled"] = enforced.ToString() })
            .Build();

        var multiTenant = new MultiTenantDbContext(
            new DbContextOptionsBuilder<MultiTenantDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
        var accessControl = new AccessControlDbContext(
            new DbContextOptionsBuilder<AccessControlDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
        var cache = new MemoryDistributedCache(Microsoft.Extensions.Options.Options.Create(new MemoryDistributedCacheOptions()));

        return new TenantModuleGate(multiTenant, accessControl, cache, configuration);
    }

    [Fact]
    public async Task WhenNotEnforced_AllowsAnyModule()
    {
        var gate = CreateGate(enforced: false);

        Assert.False(gate.Enforced);
        Assert.True(await gate.HasModuleAsync(Guid.NewGuid(), "APP_GESTOR"));
    }

    [Fact]
    public void WhenConfiguredEnabled_ReportsEnforced()
    {
        var gate = CreateGate(enforced: true);

        Assert.True(gate.Enforced);
    }
}
