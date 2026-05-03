using OpaMenu.Infrastructure.Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpaMenu.Infrastructure.Shared.Data.Context
{
    /// <summary>
    /// Implementação padrão para cenários sem tenant (ex.: testes)
    /// </summary>
    public class DefaultTenantContext : ITenantContext
    {
        public Guid? TenantId { get; private set; }
        public string? TenantSlug { get; private set; }
        public string? TenantName { get; private set; }
        public bool HasTenant => TenantId.HasValue && TenantId.Value != Guid.Empty;
        public bool IsSubscriptionActive { get; private set; } = true; // Default to true for non-tenant contexts
        public IEnumerable<string> EnabledModules { get; private set; } = Enumerable.Empty<string>();

        public void SetTenant(Guid? tenantId, string? tenantSlug, string? tenantName)
        {
            TenantId = tenantId;
            TenantSlug = tenantSlug;
            TenantName = tenantName;
        }

        public void SetSubscriptionInfo(bool isActive, IEnumerable<string> enabledModules)
        {
            IsSubscriptionActive = isActive;
            EnabledModules = enabledModules;
        }
    }
}
