using OpaMenu.Infrastructure.Shared.Interfaces;

namespace Authenticator.API.Infrastructure.Data.Context
{
    /// <summary>
    /// Implementação do contexto de Tenant para escopo de requisição
    /// </summary>
    public class TenantContext : ITenantContext
    {
        public Guid? TenantId { get; private set; }
        public string? TenantSlug { get; private set; }
        public string? TenantName { get; private set; }

        public bool HasTenant => TenantId.HasValue || !string.IsNullOrWhiteSpace(TenantSlug);

        public void SetTenant(Guid? tenantId, string? tenantSlug, string? tenantName)
        {
            TenantId = tenantId;
            TenantSlug = tenantSlug;
            TenantName = tenantName;
        }
    }
}
