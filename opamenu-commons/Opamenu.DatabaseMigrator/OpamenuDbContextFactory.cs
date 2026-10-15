using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Opamenu.Infrastructure.Shared.Data.Context.Opamenu;

namespace Opamenu.DatabaseMigrator;

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
        optionsBuilder.UseNpgsql(connectionString, b => b.MigrationsAssembly("Opamenu.Infrastructure.Shared"));

        return new OpamenuDbContext(optionsBuilder.Options);
    }
}
