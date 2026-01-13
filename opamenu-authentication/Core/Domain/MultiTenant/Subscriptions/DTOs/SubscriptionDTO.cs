using OpaMenu.Infrastructure.Shared.Entities.MultiTenant.Subscription;
using Authenticator.API.Core.Domain.MultiTenant.Plan.DTOs;
using Authenticator.API.Core.Domain.MultiTenant.Tenant.DTOs;
using Authenticator.API.Core.Domain.MultiTenant.TenantProduct.DTOs;

namespace Authenticator.API.Core.Domain.MultiTenant.Subscriptions.DTOs
{
    public class SubscriptionDTO
    {
        public Guid Id { get; set; }
        public Guid TenantId { get; set; }
        public Guid ProductId { get; set; }
        public Guid PlanId { get; set; }
        public ESubscriptionStatus Status { get; set; } = ESubscriptionStatus.Ativo;
        public DateTime? TrialEndsAt { get; set; }
        public DateTime CurrentPeriodStart { get; set; }
        public DateTime CurrentPeriodEnd { get; set; }
        public bool CancelAtPeriodEnd { get; set; } = false;
        public DateTime? CancelledAt { get; set; }
        public decimal? CustomPricing { get; set; }
        public string? UsageLimits { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // Navigation properties
        public TenantSummaryDTO? Tenant { get; set; }
        public TenantProductDTO? Product { get; set; }
        public PlanDTO? Plan { get; set; }
    }
}

