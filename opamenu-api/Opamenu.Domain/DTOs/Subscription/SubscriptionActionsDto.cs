using System;
using System.ComponentModel.DataAnnotations;

namespace Opamenu.Domain.DTOs.Subscription
{
    public class ChangePlanRequestDto
    {
        [Required]
        public Guid NewPlanId { get; set; }
    }
}
