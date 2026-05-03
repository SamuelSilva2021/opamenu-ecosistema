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
        /// Indica se a assinatura do tenant está ativa (Ativo ou Trial)
        /// </summary>
        bool IsSubscriptionActive { get; }

        /// <summary>
        /// Lista de chaves de módulos habilitados para este tenant baseados no plano
        /// </summary>
        IEnumerable<string> EnabledModules { get; }

        /// <summary>
        /// Define o tenant no contexto atual
        /// </summary>
        void SetTenant(Guid? tenantId, string? tenantSlug, string? tenantName);

        /// <summary>
        /// Define as informações de assinatura e módulos
        /// </summary>
        void SetSubscriptionInfo(bool isActive, IEnumerable<string> enabledModules);
    }
}
