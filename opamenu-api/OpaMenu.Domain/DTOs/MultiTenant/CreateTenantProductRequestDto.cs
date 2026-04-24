namespace OpaMenu.Domain.DTOs.MultiTenant;

public sealed class CreateTenantProductRequestDto
{
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Category { get; set; } = string.Empty;
    public string? Version { get; set; }
    public string? Status { get; set; }
    public string? ConfigurationSchema { get; set; }
    public string? PricingModel { get; set; }
    public decimal BasePrice { get; set; }
    public decimal? SetupFee { get; set; }
}
