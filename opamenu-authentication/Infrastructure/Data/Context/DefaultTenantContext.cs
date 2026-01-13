using OpaMenu.Infrastructure.Shared.Interfaces;

namespace Authenticator.API.Infrastructure.Data.Context
{
    /// <summary>
    /// Implementação padrão para cenários sem tenant (ex.: testes)
    /// </summary>
    public class DefaultTenantContext : ITenantContext
    {
        public Guid? TenantId => null;
        public string? TenantSlug => null;
        public string? TenantName => null;
        public bool HasTenant => false;
        public void SetTenant(Guid? tenantId, string? tenantSlug, string? tenantName) { }
    }
}

