using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpaMenu.Infrastructure.Shared.Data.Context;
using OpaMenu.Infrastructure.Shared.Data.Context.AccessControl;
using OpaMenu.Infrastructure.Shared.Data.Context.MultTenant;
using OpaMenu.Infrastructure.Shared.Data.Context.Opamenu;
using OpaMenu.Infrastructure.Shared.Interfaces;

namespace OpaMenu.Infrastructure.Shared.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSharedInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionStringAccessControl = configuration.GetConnectionString("AccessControlDatabase");
        var connectionStringMultiTenant = configuration.GetConnectionString("MultiTenantDatabase");
        var connectionStringOpamenu = configuration.GetConnectionString("OpamenuDatabase");
        
        Console.WriteLine("--- Shared Infrastructure Configuration ---");
        Console.WriteLine($"AccessControlDatabase: {(!string.IsNullOrEmpty(connectionStringAccessControl) ? "Found" : "Missing")}");
        Console.WriteLine($"MultiTenantDatabase: {(!string.IsNullOrEmpty(connectionStringMultiTenant) ? "Found" : "Missing")}");
        Console.WriteLine($"OpamenuDatabase: {(!string.IsNullOrEmpty(connectionStringOpamenu) ? "Found" : "Missing")}");
        Console.WriteLine("-------------------------------------------");

        // Registra o ITenantContext padrão para uso interno das migrations ou quando não houver contexto HTTP
        services.AddScoped<ITenantContext, DefaultTenantContext>();

        services.AddDbContext<AccessControlDbContext>(options => 
            options.UseNpgsql(connectionStringAccessControl, b => b.MigrationsAssembly("OpaMenu.Infrastructure.Shared")));

        services.AddDbContext<MultiTenantDbContext>(options =>
            options.UseNpgsql(connectionStringMultiTenant, b => b.MigrationsAssembly("OpaMenu.Infrastructure.Shared")));

        services.AddDbContext<OpamenuDbContext>(options =>
            options.UseNpgsql(connectionStringOpamenu, b => b.MigrationsAssembly("OpaMenu.Infrastructure.Shared")));

        return services;
    }
}
