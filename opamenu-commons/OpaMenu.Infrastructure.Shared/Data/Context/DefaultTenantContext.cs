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
        public Guid? TenantId => null;
        public string? TenantSlug => null;
        public string? TenantName => null;
        public bool HasTenant => false;
        public void SetTenant(Guid? tenantId, string? tenantSlug, string? tenantName) { }
    }
}

