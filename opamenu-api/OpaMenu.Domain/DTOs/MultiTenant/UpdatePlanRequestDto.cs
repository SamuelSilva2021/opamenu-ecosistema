namespace OpaMenu.Domain.DTOs.MultiTenant;

public sealed class UpdatePlanRequestDto
{
    public string? Name { get; set; }
    public string? Slug { get; set; }
    public string? Description { get; set; }
    public decimal? Price { get; set; }
    public string? BillingCycle { get; set; }
    public int? MaxUsers { get; set; }
    public int? MaxStorageGb { get; set; }
    public string? Features { get; set; }
    public string? Status { get; set; }
    public int? SortOrder { get; set; }
    public bool? IsTrial { get; set; }
    public int? TrialPeriodDays { get; set; }
}
