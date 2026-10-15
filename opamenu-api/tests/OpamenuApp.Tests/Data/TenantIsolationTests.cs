using Microsoft.EntityFrameworkCore;
using Opamenu.Infrastructure.Shared.Data.Context;
using Opamenu.Infrastructure.Shared.Data.Context.Opamenu;
using Opamenu.Infrastructure.Shared.Entities.Opamenu;
using Opamenu.Infrastructure.Shared.Interfaces;

namespace OpamenuApp.Tests.Data;

public class TenantIsolationTests
{
    private static readonly Guid TenantA = Guid.NewGuid();
    private static readonly Guid TenantB = Guid.NewGuid();

    private static DbContextOptions<OpamenuDbContext> NewOptions() =>
        new DbContextOptionsBuilder<OpamenuDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

    private static ITenantContext TenantContextFor(Guid? tenantId)
    {
        var context = new DefaultTenantContext();
        context.SetTenant(tenantId, null, null);
        return context;
    }

    private static async Task SeedAsync(DbContextOptions<OpamenuDbContext> options)
    {
        await using var db = new OpamenuDbContext(options);
        db.Categories.AddRange(
            new CategoryEntity { Id = Guid.NewGuid(), Name = "Pizzas A", TenantId = TenantA },
            new CategoryEntity { Id = Guid.NewGuid(), Name = "Bebidas A", TenantId = TenantA },
            new CategoryEntity { Id = Guid.NewGuid(), Name = "Lanches B", TenantId = TenantB });
        await db.SaveChangesAsync();
    }

    [Fact]
    public async Task TenantA_DoesNotSeeTenantBData()
    {
        var options = NewOptions();
        await SeedAsync(options);

        await using var db = new OpamenuDbContext(options, TenantContextFor(TenantA));
        var categories = await db.Categories.ToListAsync();

        Assert.Equal(2, categories.Count);
        Assert.All(categories, c => Assert.Equal(TenantA, c.TenantId));
    }

    [Fact]
    public async Task TenantB_DoesNotSeeTenantAData()
    {
        var options = NewOptions();
        await SeedAsync(options);

        await using var db = new OpamenuDbContext(options, TenantContextFor(TenantB));
        var categories = await db.Categories.ToListAsync();

        var category = Assert.Single(categories);
        Assert.Equal("Lanches B", category.Name);
    }

    [Fact]
    public async Task TenantA_CannotFindTenantBRecordById()
    {
        var options = NewOptions();
        await SeedAsync(options);

        Guid tenantBCategoryId;
        await using (var seedDb = new OpamenuDbContext(options))
            tenantBCategoryId = (await seedDb.Categories.SingleAsync(c => c.TenantId == TenantB)).Id;

        await using var db = new OpamenuDbContext(options, TenantContextFor(TenantA));
        var found = await db.Categories.FirstOrDefaultAsync(c => c.Id == tenantBCategoryId);

        Assert.Null(found);
    }

    [Fact]
    public async Task WithoutTenant_SeesAllData()
    {
        var options = NewOptions();
        await SeedAsync(options);

        await using var db = new OpamenuDbContext(options, TenantContextFor(null));
        var categories = await db.Categories.ToListAsync();

        Assert.Equal(3, categories.Count);
    }

    [Fact]
    public async Task IgnoreQueryFilters_ReturnsAllData()
    {
        var options = NewOptions();
        await SeedAsync(options);

        await using var db = new OpamenuDbContext(options, TenantContextFor(TenantA));
        var categories = await db.Categories.IgnoreQueryFilters().ToListAsync();

        Assert.Equal(3, categories.Count);
    }

    [Fact]
    public async Task GlobalRecordsWithoutTenant_AreVisibleToEveryTenant()
    {
        var options = NewOptions();
        await using (var seedDb = new OpamenuDbContext(options))
        {
            seedDb.Categories.Add(new CategoryEntity { Id = Guid.NewGuid(), Name = "Global", TenantId = null });
            await seedDb.SaveChangesAsync();
        }

        await using var db = new OpamenuDbContext(options, TenantContextFor(TenantA));
        var categories = await db.Categories.ToListAsync();

        Assert.Single(categories);
    }
}
