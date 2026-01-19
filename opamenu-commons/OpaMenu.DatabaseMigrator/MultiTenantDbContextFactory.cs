using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using OpaMenu.Infrastructure.Shared.Data.Context;

namespace OpaMenu.DatabaseMigrator;

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
        optionsBuilder.UseNpgsql(connectionString, b => b.MigrationsAssembly("OpaMenu.Infrastructure.Shared"));

        return new MultiTenantDbContext(optionsBuilder.Options);
    }
}
