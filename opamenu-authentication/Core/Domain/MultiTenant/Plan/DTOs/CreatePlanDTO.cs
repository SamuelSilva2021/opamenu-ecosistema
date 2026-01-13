using OpaMenu.Infrastructure.Shared.Entities.MultiTenant.Plan;
using System.ComponentModel.DataAnnotations;

namespace Authenticator.API.Core.Domain.MultiTenant.Plan.DTOs
{
    public class CreatePlanDTO
    {
        [Required]
        [MaxLength(255)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Slug { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }

        public EBillingCycle BillingCycle { get; set; } = EBillingCycle.Mensal;

        [Range(1, int.MaxValue)]
        public int MaxUsers { get; set; } = 1;

        [Range(1, int.MaxValue)]
        public int MaxStorageGb { get; set; } = 1;

        public string? Features { get; set; }

        public EPlanStatus Status { get; set; } = EPlanStatus.Ativo;

        public int SortOrder { get; set; } = 0;

        public bool? IsTrial { get; set; } = false;

        public int? TrialPeriodDays { get; set; } = 0;
    }
}

