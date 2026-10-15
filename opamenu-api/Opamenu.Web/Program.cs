using CloudinaryDotNet;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Opamenu.Infrastructure.Configurations;
using Opamenu.Infrastructure.Filters;
using Opamenu.Infrastructure.Shared.Data.Context;
using Opamenu.Infrastructure.Shared.Data.Context.Opamenu;
using Opamenu.Web.Extensions;
using Opamenu.Web.Hubs;
using Serilog;


var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("Logs/log.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();


builder.Services.AddControllers(options => 
{
    options.Filters.Add<global::Opamenu.Infrastructure.Filters.SubscriptionStatusFilter>();
})
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.DictionaryKeyPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
    });
builder.Services.Configure<Microsoft.AspNetCore.Builder.ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedFor | Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedProto;
    options.KnownNetworks.Clear();
    options.KnownProxies.Clear();
});

builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    options.AddPolicy("public-write", httpContext =>
    {
        var ip = httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        var slug = $"{httpContext.Request.RouteValues["neighborhood"]}/{httpContext.Request.RouteValues["slug"]}";
        return System.Threading.RateLimiting.RateLimitPartition.GetFixedWindowLimiter($"{ip}|{slug}", _ => new System.Threading.RateLimiting.FixedWindowRateLimiterOptions
        {
            PermitLimit = 20,
            Window = TimeSpan.FromMinutes(1),
            QueueLimit = 0
        });
    });

    options.AddPolicy("auth", httpContext =>
    {
        var ip = httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        return System.Threading.RateLimiting.RateLimitPartition.GetFixedWindowLimiter(ip, _ => new System.Threading.RateLimiting.FixedWindowRateLimiterOptions
        {
            PermitLimit = 10,
            Window = TimeSpan.FromMinutes(1),
            QueueLimit = 0
        });
    });
});

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Opamenu API",
        Version = "v1",
        Description = "API do sistema Opamenu integrada com autenticação JWT"
    });

    c.CustomSchemaIds(type => type.FullName ?? type.Name);

    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = @"Autenticação JWT usando o esquema Bearer.
        
            **Como usar:**
            1. Faça login na própria API (`/api/auth/login`)
            2. Copie o `accessToken` da resposta
            3. Cole o token no campo abaixo (apenas o token, sem 'Bearer ')
            4. Clique em 'Authorize' e teste os endpoints protegidos"
    });

    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddSingleton(_ =>
{
    var account = new Account(
        builder.Configuration["Cloudinary:CloudName"],
        builder.Configuration["Cloudinary:ApiKey"],
        builder.Configuration["Cloudinary:ApiSecret"]
    );

    return new Cloudinary(account);
});

builder.Services.AddSignalR(options =>
{
    options.EnableDetailedErrors = builder.Environment.IsDevelopment();
    options.KeepAliveInterval = TimeSpan.FromSeconds(15);
    options.ClientTimeoutInterval = TimeSpan.FromSeconds(30);
    options.HandshakeTimeout = TimeSpan.FromSeconds(15);
    options.MaximumReceiveMessageSize = 32 * 1024;
});

builder.Services.AddCorsServices(builder.Configuration);

builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpClient();

builder.Services.AddScoped<PermissionAuthorizationFilter>();
builder.Services.AddScoped<global::Opamenu.Infrastructure.Filters.SubscriptionStatusFilter>();
builder.Services.AddDatabaseServices(builder.Configuration);
builder.Services.AddApplicationServices();
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.Configure<IISServerOptions>(options =>
{
    options.MaxRequestBodySize = 52428800;
});

if (builder.Environment.IsProduction())
{
    var productionJwtSecret = Environment.GetEnvironmentVariable("Authentication__JwtSecret");
    if (string.IsNullOrWhiteSpace(productionJwtSecret) || productionJwtSecret.Length < 32)
        throw new InvalidOperationException("Authentication__JwtSecret deve ser definido por variável de ambiente com no mínimo 32 caracteres em produção.");
}

var app = builder.Build();



if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    using (var scope = app.Services.CreateScope())
    {
        var context = scope.ServiceProvider.GetRequiredService<OpamenuDbContext>();
        try
        {
            context.Database.Migrate();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao aplicar migrações: {ex.Message}");
            try
            {
                context.Database.EnsureCreated();
            }
            catch (Exception ex2)
            {
                Console.WriteLine($"Erro ao criar o banco de dados: {ex2.Message}");
            }
        }
    }
}
if (!app.Environment.IsProduction())
{
    app.UseHttpsRedirection();
}

app.UseForwardedHeaders();
app.UseCors("CorsPolicy");
app.UseRateLimiter();
app.UseAuthentication();
app.UseMiddleware<global::Opamenu.Web.Middleware.TenantContextMiddleware>();
app.UseAuthorization();

app.UseStaticFiles();

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(builder.Environment.ContentRootPath, "wwwroot", "uploads")),
    RequestPath = "/uploads",
    OnPrepareResponse = ctx =>
    {
        var cacheDuration = app.Environment.IsDevelopment() ? 3600 : 31536000;
        ctx.Context.Response.Headers.Append("Cache-Control", $"public,max-age={cacheDuration}");
        ctx.Context.Response.Headers.Append("Expires", DateTime.UtcNow.AddSeconds(cacheDuration).ToString("R"));
        
        ctx.Context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
        ctx.Context.Response.Headers.Append("Content-Security-Policy", "default-src 'none'; img-src 'self'");
    }
});

app.MapControllers().RequireCors("CorsPolicy");

app.MapHub<OrderNotificationHub>("/hubs/notifications");

app.MapGet("/health", () => new { Status = "Healthy", Timestamp = DateTime.UtcNow });

app.Run();

