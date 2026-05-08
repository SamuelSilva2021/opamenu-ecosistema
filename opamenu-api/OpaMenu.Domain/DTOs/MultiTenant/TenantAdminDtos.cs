namespace OpaMenu.Domain.DTOs.MultiTenant;

public sealed class TenantSummaryDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? Domain { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string Type { get; set; } = string.Empty;
    public Guid? ParentTenantId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public Guid? ActiveSubscriptionId { get; set; }
}
