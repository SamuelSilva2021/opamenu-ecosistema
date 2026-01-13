using System;
using System.ComponentModel.DataAnnotations;

namespace OpaMenu.Domain.DTOs.Subscription
{
    public class ChangePlanRequestDto
    {
        [Required]
        public Guid NewPlanId { get; set; }
    }

    public class CancelSubscriptionRequestDto
    {
        public string? Reason { get; set; }
    }
}
