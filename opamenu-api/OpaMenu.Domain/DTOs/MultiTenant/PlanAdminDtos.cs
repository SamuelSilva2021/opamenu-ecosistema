namespace OpaMenu.Domain.DTOs.MultiTenant;

public sealed class PlanDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public string BillingCycle { get; set; } = string.Empty;
    public int MaxUsers { get; set; }
    public int MaxStorageGb { get; set; }
    public string? Features { get; set; }
    public string Status { get; set; } = string.Empty;
    public string Category { get; set; } = "Customer";
    public int SortOrder { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int TotalSubscriptions { get; set; }
    public int ActiveSubscriptions { get; set; }
    public decimal MonthlyRecurringRevenue { get; set; }
    public bool? IsTrial { get; set; }
    public int? TrialPeriodDays { get; set; }
}
