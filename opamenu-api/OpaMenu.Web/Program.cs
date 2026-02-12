using CloudinaryDotNet;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using OpaMenu.Infrastructure.Configurations;
using OpaMenu.Infrastructure.Filters;
using OpaMenu.Infrastructure.Shared.Data.Context;
using OpaMenu.Infrastructure.Shared.Data.Context.Opamenu;
using OpaMenu.Web.Extensions;
using OpaMenu.Web.Hubs;
using Serilog;


var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("Logs/log.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();


// Add services to the container
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.DictionaryKeyPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
    });
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Opa Menu API",
        Version = "v1",
        Description = "API do sistema Opa Menu integrada com autenticação JWT"
    });

    // ConfiguraÃ§Ã£o para autenticaÃ§Ã£o JWT no Swagger
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = @"Autenticação JWT usando o esquema Bearer.
        
            **Como usar:**
            1.Faça login no saas-authentication-api
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

// Configure SignalR
builder.Services.AddSignalR(options =>
{
    options.EnableDetailedErrors = builder.Environment.IsDevelopment();
    options.KeepAliveInterval = TimeSpan.FromSeconds(15);
    options.ClientTimeoutInterval = TimeSpan.FromSeconds(30);
    options.HandshakeTimeout = TimeSpan.FromSeconds(15);
    options.MaximumReceiveMessageSize = 32 * 1024;
});

// Configure CORS
builder.Services.AddCorsServices(builder.Configuration);


// Register application services
builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpClient();

builder.Services.AddScoped<PermissionAuthorizationFilter>();

builder.Services.AddDatabaseServices(builder.Configuration);

// Register additional application services
builder.Services.AddApplicationServices();

// Configure JWT Authentication
builder.Services.AddJwtAuthentication(builder.Configuration);

// Configure file upload settings
builder.Services.Configure<IISServerOptions>(options =>
{
    options.MaxRequestBodySize = 52428800; // 50MB
});

var app = builder.Build();



// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    // Ensure database is created and migrated
    using (var scope = app.Services.CreateScope())
    {
        var context = scope.ServiceProvider.GetRequiredService<OpamenuDbContext>();
        try
        {
            context.Database.Migrate(); // Apply any pending migrations
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
else
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

if (!app.Environment.IsProduction())
{
    app.UseHttpsRedirection();
}

app.UseCors("CorsPolicy");
// Enable authentication and authorization
app.UseAuthentication();
app.UseAuthorization();

// Serve static files
app.UseStaticFiles();

// Configure uploads directory specifically with cache headers
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(builder.Environment.ContentRootPath, "wwwroot", "uploads")),
    RequestPath = "/uploads",
    OnPrepareResponse = ctx =>
    {
        // Cache images for 1 year in production, 1 hour in development
        var cacheDuration = app.Environment.IsDevelopment() ? 3600 : 31536000;
        ctx.Context.Response.Headers.Append("Cache-Control", $"public,max-age={cacheDuration}");
        ctx.Context.Response.Headers.Append("Expires", DateTime.UtcNow.AddSeconds(cacheDuration).ToString("R"));
        
        // Security headers for images
        ctx.Context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
        ctx.Context.Response.Headers.Append("Content-Security-Policy", "default-src 'none'; img-src 'self'");
    }
});

app.MapControllers().RequireCors("CorsPolicy");

// Map SignalR Hub
app.MapHub<OrderNotificationHub>("/hubs/notifications");

// Health check endpoint
app.MapGet("/health", () => new { Status = "Healthy", Timestamp = DateTime.UtcNow });

// Test endpoint
app.MapGet("/api/test", () => new { Message = "Test endpoint working", Timestamp = DateTime.UtcNow });

// SignalR test endpoint
app.MapGet("/signalr/test", () => new { 
    Message = "SignalR Hub configured", 
    HubPath = "/hubs/notifications",
    Timestamp = DateTime.UtcNow 
});

app.Run();

