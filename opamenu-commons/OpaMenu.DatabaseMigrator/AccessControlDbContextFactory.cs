using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using OpaMenu.Infrastructure.Shared.Data.Context.AccessControl;

namespace OpaMenu.DatabaseMigrator;

public class AccessControlDbContextFactory : IDesignTimeDbContextFactory<AccessControlDbContext>
{
    public AccessControlDbContext CreateDbContext(string[] args)
    {
        var basePath = AppContext.BaseDirectory;
        var configuration = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        var connectionString = configuration.GetConnectionString("AccessControlDatabase");

        var optionsBuilder = new DbContextOptionsBuilder<AccessControlDbContext>();
        optionsBuilder.UseNpgsql(connectionString, b => b.MigrationsAssembly("OpaMenu.Infrastructure.Shared"));

        return new AccessControlDbContext(optionsBuilder.Options);
    }
}
