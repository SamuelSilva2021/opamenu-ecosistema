using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpaMenu.Infrastructure.Shared.Interfaces
{
    /// <summary>
    /// Contexto do tenant atual
    /// </summary>
    public interface ITenantContext
    {
        /// <summary>
        /// ID do tenant atual, se houver
        /// </summary>
        Guid? TenantId { get; }

        /// <summary>
        /// Slug do tenant atual, se houver
        /// </summary>
        string? TenantSlug { get; }

        /// <summary>
        /// Nome do tenant atual, se houver
        /// </summary>
        string? TenantName { get; }

        /// <summary>
        /// Indica se há um tenant associado ao contexto atual
        /// </summary>
        bool HasTenant { get; }

        /// <summary>
        /// Define o tenant no contexto atual
        /// </summary>
        /// <param name="tenantId">ID do tenant</param>
        /// <param name="tenantSlug">Slug do tenant</param>
        /// <param name="tenantName">Nome do tenant</param>
        void SetTenant(Guid? tenantId, string? tenantSlug, string? tenantName);
    }
}

