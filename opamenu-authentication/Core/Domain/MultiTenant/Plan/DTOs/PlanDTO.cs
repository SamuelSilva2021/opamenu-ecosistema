using OpaMenu.Infrastructure.Shared.Entities.MultiTenant.Plan;
namespace Authenticator.API.Core.Domain.MultiTenant.Plan.DTOs
{
    public class PlanDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public EBillingCycle BillingCycle { get; set; } = EBillingCycle.Mensal;
        public int MaxUsers { get; set; } = 1;
        public int MaxStorageGb { get; set; } = 1;
        public string? Features { get; set; }
        public EPlanStatus Status { get; set; } = EPlanStatus.Ativo;
        public int SortOrder { get; set; } = 0;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int TotalSubscriptions { get; set; }
        public int ActiveSubscriptions { get; set; }
        public decimal MonthlyRecurringRevenue { get; set; }
        public bool? IsTrial { get; set; } = false;

        public int? TrialPeriodDays { get; set; } = 0;
    }
}

