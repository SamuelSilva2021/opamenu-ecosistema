using OpaMenu.Infrastructure.Shared.Entities.MultiTenant.Plan;
namespace Authenticator.API.Core.Domain.MultiTenant.Plan.DTOs
{
    public class PlanWithFeaturesDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public EBillingCycle BillingCycle { get; set; } = EBillingCycle.Mensal;
        public EPlanStatus Status { get; set; } = EPlanStatus.Ativo;
        public int MaxUsers { get; set; } = 1;
        public int MaxStorageGb { get; set; } = 1;
        public int SortOrder { get; set; } = 0;
        public Dictionary<string, object> Features { get; set; } = new();
        public List<string> FeatureList { get; set; } = new();
        public bool? IsTrial { get; set; } = false;
        public int? TrialPeriodDays { get; set; } = 0;

        public DateTime CreatedAt { get; set; }
    }
}

