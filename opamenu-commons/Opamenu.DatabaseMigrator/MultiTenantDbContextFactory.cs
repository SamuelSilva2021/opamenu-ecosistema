using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Opamenu.Infrastructure.Shared.Data.Context.MultTenant;

namespace Opamenu.DatabaseMigrator;

public class MultiTenantDbContextFactory : IDesignTimeDbContextFactory<MultiTenantDbContext>
{
    public MultiTenantDbContext CreateDbContext(string[] args)
    {
        var basePath = AppContext.BaseDirectory;
        var configuration = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        var connectionString = configuration.GetConnectionString("MultiTenantDatabase");

        var optionsBuilder = new DbContextOptionsBuilder<MultiTenantDbContext>();
        optionsBuilder.UseNpgsql(connectionString, b => b.MigrationsAssembly("Opamenu.Infrastructure.Shared"));

        return new MultiTenantDbContext(optionsBuilder.Options);
    }
}
