using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpaMenu.Domain.DTOs.Subscription
{
    public class SubscriptionCreateDto
    {
        [Required]
        public Guid TenantId { get; set; }

        [Required]
        public Guid ProductId { get; set; }

        [Required]
        public Guid PlanId { get; set; }

        public DateTime? TrialEndsAt { get; set; }

        public decimal? CustomPricing { get; set; }

        public string? UsageLimits { get; set; }

        public bool CancelAtPeriodEnd { get; set; } = false;
    }
}
