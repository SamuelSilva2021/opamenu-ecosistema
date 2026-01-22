using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Npgsql;
using OpaMenu.Application.Mappings;
using OpaMenu.Application.Services.Interfaces;
using OpaMenu.Domain.Interfaces;
using OpaMenu.Infrastructure.Authentication;
using OpaMenu.Infrastructure.Data.Interceptors;
using OpaMenu.Infrastructure.Configurations;
using OpaMenu.Web.Services;
using System.Text;
using OpaMenu.Infrastructure.Shared.Interfaces;
using OpaMenu.Infrastructure.Shared.Data.Context;
using OpaMenu.Infrastructure.Shared.Data.Context.Opamenu;
using OpaMenu.Infrastructure.Shared.Data.Context.AccessControl;
using OpaMenu.Infrastructure.Shared.Data.Context.MultTenant;


namespace OpaMenu.Web.Extensions;

/// <summary>
/// Extenção para IServiceCollection
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Configura os bancos de dados
    /// </summary>
    public static IServiceCollection AddDatabaseServices(
    this IServiceCollection services,
    IConfiguration configuration)
    {
        // Contexto de tenant e interceptor
        services.AddScoped<ITenantContext, DefaultTenantContext>();
        services.AddScoped<TenantSaveChangesInterceptor>();

        // ---------- OpaMenu ----------
        services.AddDbContext<OpamenuDbContext>((sp, options) =>
        {
            var connectionString = configuration.GetConnectionString("OpamenuDatabase");
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new InvalidOperationException("ConnectionString 'OpamenuDatabase' nÃ£o configurada.");

            options.UseNpgsql(connectionString, npgsql =>
            {
                npgsql.EnableRetryOnFailure(5);
            });

            options.AddInterceptors(
                sp.GetRequiredService<TenantSaveChangesInterceptor>()
            );
        });

        // ---------- Access Control ----------
        services.AddDbContext<AccessControlDbContext>((sp, options) =>
        {
            var connectionString = configuration.GetConnectionString("AccessControlDatabase");
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new InvalidOperationException("ConnectionString 'AccessControlDatabase' nÃ£o configurada.");

            options.UseNpgsql(connectionString, npgsql =>
            {
                npgsql.EnableRetryOnFailure(5);
            });

            options.AddInterceptors(
                sp.GetRequiredService<TenantSaveChangesInterceptor>()
            );
        });

        // ---------- Multi-Tenant ----------
        services.AddDbContext<MultiTenantDbContext>(options =>
        {
            var connectionString = configuration.GetConnectionString("MultiTenantDatabase");
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new InvalidOperationException("ConnectionString 'MultiTenantDatabase' nÃ£o configurada.");

            options.UseNpgsql(connectionString, npgsql =>
            {
                npgsql.EnableRetryOnFailure(5);
            });
        });

        return services;
    }

    /// <summary>
    /// Registrar serviços da aplicação automaticamente
    /// </summary>
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(ProductMappingProfile));
        services.AddConfigureScrutor();

        // Configurar Cache Distribuído (Memory Cache como padrão, pronto para Redis)
        services.AddDistributedMemoryCache();

        // Registrar NotificationService com Hub especÃ­fico (substitui o registro do Scrutor)
        services.AddScoped<INotificationService, SignalRNotificationServiceWrapper>();

        // Registrar serviços de autenticação e usuário atual
        services.AddScoped<IAuthenticationService, ExternalAuthenticationService>();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        
        return services;
    }

    /// <summary>
    /// Configura a autenticação JWT
    /// </summary>
    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtSecret = configuration["Authentication:JwtSecret"];
        var jwtIssuer = configuration["Authentication:JwtIssuer"];
        var jwtAudience = configuration["Authentication:JwtAudience"];

        if (string.IsNullOrEmpty(jwtSecret))
        {
            throw new InvalidOperationException("JWT Secret nÃ£o configurado");
        }

        var key = Encoding.UTF8.GetBytes(jwtSecret);

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.RequireHttpsMetadata = false;
            options.SaveToken = true;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = !string.IsNullOrEmpty(jwtIssuer),
                ValidIssuer = jwtIssuer,
                ValidateAudience = !string.IsNullOrEmpty(jwtAudience),
                ValidAudience = jwtAudience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            options.Events = new JwtBearerEvents
            {
                OnAuthenticationFailed = context =>
                {
                    var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
                    logger.LogWarning("Falha na autenticaÃ§Ã£o JWT: {Error}", context.Exception.Message);
                    return Task.CompletedTask;
                },
                OnTokenValidated = context =>
                {
                    var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
                    var userId = context.Principal?.FindFirst("user_id")?.Value ?? 
                                context.Principal?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                    logger.LogInformation("Token JWT validado para usuÃ¡rio: {UserId}", userId);
                    return Task.CompletedTask;
                }
            };
        });

        services.AddAuthorization();

        return services;
    }
    /// <summary>
    /// Adiciona e configura os serviços CORS com base na
    /// configuration.
    /// </summary>
    /// <param name="services">The service collection to which the CORS services will be added.</param>
    /// <param name="configuration">The application configuration containing CORS settings, such as allowed origins.</param>
    /// <returns>The original service collection with CORS services configured.</returns>
    public static IServiceCollection AddCorsServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("CorsPolicy", builder =>
            {
                var allowedOrigins = configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? ["*"];

                if (allowedOrigins.Contains("*"))
                {
                    builder.AllowAnyOrigin()
                           .AllowAnyHeader()
                           .AllowAnyMethod();
                }
                else
                {
                    builder.WithOrigins(allowedOrigins)
                           .AllowAnyHeader()
                           .AllowAnyMethod();
                }
            });
        });

        return services;
    }
}
