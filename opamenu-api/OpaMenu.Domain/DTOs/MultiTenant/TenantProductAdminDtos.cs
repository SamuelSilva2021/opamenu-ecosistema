namespace OpaMenu.Domain.DTOs.MultiTenant;

public sealed class TenantProductDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Category { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? ConfigurationSchema { get; set; }
    public string PricingModel { get; set; } = string.Empty;
    public decimal BasePrice { get; set; }
    public decimal SetupFee { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int TotalSubscriptions { get; set; }
    public int ActiveSubscriptions { get; set; }
}

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

public sealed class UpdateTenantProductRequestDto
{
    public string? Name { get; set; }
    public string? Slug { get; set; }
    public string? Description { get; set; }
    public string? Category { get; set; }
    public string? Version { get; set; }
    public string? Status { get; set; }
    public string? ConfigurationSchema { get; set; }
    public string? PricingModel { get; set; }
    public decimal? BasePrice { get; set; }
    public decimal? SetupFee { get; set; }
}

