using OpaMenu.Domain.DTOs.Plan;
using OpaMenu.Domain.DTOs.Product;
using OpaMenu.Domain.DTOs.Tenant;
using System;

namespace OpaMenu.Domain.DTOs.Subscription
{
    public class SubscriptionStatusResponseDto
    {
        public Guid Id { get; set; }

        public Guid TenantId { get; set; }
        public Guid ProductId { get; set; }
        public Guid PlanId { get; set; }

        public string Status { get; set; } = string.Empty;

        public string PlanName { get; set; } = string.Empty;
        public int DaysRemaining { get; set; }
        public bool IsTrial { get; set; }

        public DateTime? TrialEndsAt { get; set; }

        public DateTime CurrentPeriodStart { get; set; }
        public DateTime CurrentPeriodEnd { get; set; }

        public bool CancelAtPeriodEnd { get; set; }

        public DateTime? CancelledAt { get; set; }

        public decimal? CustomPricing { get; set; }

        public string? UsageLimits { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public TenantSummaryDto Tenant { get; set; } = null!;
        public ProductSummaryDto Product { get; set; } = null!;
        public PlanSummaryDto Plan { get; set; } = null!;
    }
}