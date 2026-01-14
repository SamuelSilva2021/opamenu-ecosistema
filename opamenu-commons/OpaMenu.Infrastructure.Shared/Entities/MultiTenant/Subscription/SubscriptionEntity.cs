using OpaMenu.Infrastructure.Shared.Entities;
using OpaMenu.Infrastructure.Shared.Entities.MultiTenant.Plan;
using OpaMenu.Infrastructure.Shared.Entities.MultiTenant.Tenant;
using OpaMenu.Infrastructure.Shared.Entities.MultiTenant.TenantProduct;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpaMenu.Infrastructure.Shared.Entities.MultiTenant.Subscription
{
    /// <summary>
    /// Representa uma assinatura/plano de um tenant
    /// </summary>
    public class SubscriptionEntity
    {
        /// <summary>
        /// ID único da assinatura
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// ID do tenant
        /// </summary>
        public Guid TenantId { get; set; }

        /// <summary>
        /// ID do produto
        /// </summary>
        public Guid ProductId { get; set; }

        /// <summary>
        /// ID do plano
        /// </summary>
        public Guid PlanId { get; set; }

        /// <summary>
        /// Status da assinatura
        /// </summary>
        [Required]
        public ESubscriptionStatus Status { get; set; } = ESubscriptionStatus.Ativo;

        /// <summary>
        /// Data de fim do período de teste
        /// </summary>
        public DateTime? TrialEndsAt { get; set; }

        /// <summary>
        /// Início do período atual
        /// </summary>
        public DateTime CurrentPeriodStart { get; set; }

        /// <summary>
        /// Fim do período atual
        /// </summary>
        public DateTime CurrentPeriodEnd { get; set; }

        /// <summary>
        /// Se deve cancelar no final do período
        /// </summary>
        public bool CancelAtPeriodEnd { get; set; } = false;

        /// <summary>
        /// Data de cancelamento
        /// </summary>
        public DateTime? CancelledAt { get; set; }

        /// <summary>
        /// Preço customizado
        /// </summary>
        public decimal? CustomPricing { get; set; }

        /// <summary>
        /// Limites de uso
        /// </summary>
        public string? UsageLimits { get; set; }

        /// <summary>
        /// Data de criação
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Data de atualização
        /// </summary>
        public DateTime? UpdatedAt { get; set; }

        // Navigation properties
        public virtual TenantEntity Tenant { get; set; } = null!;
        public virtual TenantProductEntity Product { get; set; } = null!;
        public virtual PlanEntity Plan { get; set; } = null!;
    }
}

