namespace OpaMenu.Domain.DTOs.MultiTenant;

public sealed class CreatePlanRequestDto
{
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public string BillingCycle { get; set; } = string.Empty;
    public int MaxUsers { get; set; }
    public int MaxStorageGb { get; set; }
    public string? Features { get; set; }
    public string Status { get; set; } = string.Empty;
    public int SortOrder { get; set; }
    public bool? IsTrial { get; set; }
    public int? TrialPeriodDays { get; set; }
}
