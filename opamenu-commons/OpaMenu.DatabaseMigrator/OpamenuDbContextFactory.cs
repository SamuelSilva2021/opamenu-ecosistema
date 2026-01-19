using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using OpaMenu.Infrastructure.Shared.Data.Context;

namespace OpaMenu.DatabaseMigrator;

public class OpamenuDbContextFactory : IDesignTimeDbContextFactory<OpamenuDbContext>
{
    public OpamenuDbContext CreateDbContext(string[] args)
    {
        var basePath = AppContext.BaseDirectory;
        var configuration = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        var connectionString = configuration.GetConnectionString("OpamenuDatabase");

        var optionsBuilder = new DbContextOptionsBuilder<OpamenuDbContext>();
        optionsBuilder.UseNpgsql(connectionString, b => b.MigrationsAssembly("OpaMenu.Infrastructure.Shared"));

        return new OpamenuDbContext(optionsBuilder.Options);
    }
}
