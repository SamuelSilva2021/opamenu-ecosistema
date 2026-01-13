using OpaMenu.Infrastructure.Shared.Entities.MultiTenant.Subscription;
using Authenticator.API.Core.Domain.MultiTenant.Subscriptions.DTOs;
using OpaMenu.Infrastructure.Shared.Entities.MultiTenant.Tenant;
using OpaMenu.Infrastructure.Shared.Enums.MultiTenant;

namespace Authenticator.API.Core.Domain.MultiTenant.TenantProduct.DTOs
{
    public class TenantProductWithSubscriptionsDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public ETenantProductCategory Category { get; set; } = ETenantProductCategory.WebApp;
        public string Version { get; set; } = "1.0.0";
        public ETenantStatus Status { get; set; } = ETenantStatus.Suspenso;
        public ETenantProductPricingModel PricingModel { get; set; } = ETenantProductPricingModel.Assinatura;
        public decimal BasePrice { get; set; } = 0.00m;
        public decimal SetupFee { get; set; } = 0.00m;
        public DateTime CreatedAt { get; set; }

        public ICollection<SubscriptionSummaryDTO> Subscriptions { get; set; } = [];
        public int TotalSubscriptionsCount => Subscriptions.Count;
        public int ActiveSubscriptionsCount => Subscriptions.Count(s => s.Status == ESubscriptionStatus.Ativo);
    }
}

