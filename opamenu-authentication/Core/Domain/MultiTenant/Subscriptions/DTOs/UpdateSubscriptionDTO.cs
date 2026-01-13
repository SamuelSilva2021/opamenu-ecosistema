using OpaMenu.Infrastructure.Shared.Entities.MultiTenant.Subscription;
using System.ComponentModel.DataAnnotations;

namespace Authenticator.API.Core.Domain.MultiTenant.Subscriptions.DTOs
{
    public class UpdateSubscriptionDTO
    {
        [MaxLength(20)]
        public ESubscriptionStatus Status { get; set; }

        public DateTime? TrialEndsAt { get; set; }

        public DateTime? CurrentPeriodStart { get; set; }

        public DateTime? CurrentPeriodEnd { get; set; }

        public bool? CancelAtPeriodEnd { get; set; }

        public DateTime? CancelledAt { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? CustomPricing { get; set; }

        public string? UsageLimits { get; set; }
    }
}

