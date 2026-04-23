namespace OpaMenu.Domain.DTOs.MultiTenant;

public sealed class SubscriptionPlanDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public string BillingCycle { get; set; } = string.Empty;
}

public sealed class SubscriptionDto
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid ProductId { get; set; }
    public Guid PlanId { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime? TrialEndsAt { get; set; }
    public DateTime CurrentPeriodStart { get; set; }
    public DateTime CurrentPeriodEnd { get; set; }
    public bool CancelAtPeriodEnd { get; set; }
    public DateTime? CancelledAt { get; set; }
    public decimal? CustomPricing { get; set; }
    public string? UsageLimits { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public TenantSummaryDto? Tenant { get; set; }
    public SubscriptionPlanDto? Plan { get; set; }
}

