using System.Security.Claims;
using OpaMenu.Infrastructure.Shared.Interfaces;

namespace Authenticator.API.Infrastructure.Middlewares
{
    /// <summary>
    /// Middleware responsÃ¡vel por popular o ITenantContext a partir dos claims do JWT
    /// e/ou cabeÃ§alhos da requisiÃ§Ã£o
    /// </summary>
    public class TenantContextMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<TenantContextMiddleware> _logger;

        public TenantContextMiddleware(RequestDelegate next, ILogger<TenantContextMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, ITenantContext tenantContext)
        {
            try
            {
                // Claims do JWT
                var tenantIdClaim = context.User?.FindFirst("tenant_id")?.Value;
                var tenantSlugClaim = context.User?.FindFirst("tenant_slug")?.Value;
                var tenantNameClaim = context.User?.FindFirst("tenant_name")?.Value;

                // CabeÃ§alhos (fallback)
                context.Request.Headers.TryGetValue("X-Tenant-Id", out var tenantIdHeader);
                context.Request.Headers.TryGetValue("X-Tenant-Slug", out var tenantSlugHeader);
                context.Request.Headers.TryGetValue("X-Tenant-Name", out var tenantNameHeader);

                Guid? tenantId = null;
                if (!string.IsNullOrWhiteSpace(tenantIdClaim) && Guid.TryParse(tenantIdClaim, out var parsedClaimId))
                {
                    tenantId = parsedClaimId;
                }
                else if (!string.IsNullOrWhiteSpace(tenantIdHeader) && Guid.TryParse(tenantIdHeader!, out var parsedHeaderId))
                {
                    tenantId = parsedHeaderId;
                }

                var tenantSlug = !string.IsNullOrWhiteSpace(tenantSlugClaim) ? tenantSlugClaim : (string?)tenantSlugHeader;
                var tenantName = !string.IsNullOrWhiteSpace(tenantNameClaim) ? tenantNameClaim : (string?)tenantNameHeader;

                tenantContext.SetTenant(tenantId, tenantSlug, tenantName);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Falha ao resolver contexto de tenant");
            }

            await _next(context);
        }
    }
}
