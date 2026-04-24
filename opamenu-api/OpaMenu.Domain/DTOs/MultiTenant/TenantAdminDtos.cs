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
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public Guid? ActiveSubscriptionId { get; set; }
}

public sealed class TenantDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? Domain { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? CnpjCpf { get; set; }
    public string? RazaoSocial { get; set; }
    public string? InscricaoEstadual { get; set; }
    public string? InscricaoMunicipal { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Website { get; set; }
    public string? AddressStreet { get; set; }
    public string? AddressNumber { get; set; }
    public string? AddressComplement { get; set; }
    public string? AddressNeighborhood { get; set; }
    public string? AddressCity { get; set; }
    public string? AddressState { get; set; }
    public string? AddressZipcode { get; set; }
    public string? AddressCountry { get; set; }
    public string? BillingStreet { get; set; }
    public string? BillingNumber { get; set; }
    public string? BillingComplement { get; set; }
    public string? BillingNeighborhood { get; set; }
    public string? BillingCity { get; set; }
    public string? BillingState { get; set; }
    public string? BillingZipcode { get; set; }
    public string? BillingCountry { get; set; }
    public string? LegalRepresentativeName { get; set; }
    public string? LegalRepresentativeCpf { get; set; }
    public string? LegalRepresentativeEmail { get; set; }
    public string? LegalRepresentativePhone { get; set; }
    public Guid? ActiveSubscriptionId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public Dictionary<string, object> Settings { get; set; } = new();
}

public sealed class UpdateTenantRequestDto
{
    public string? Name { get; set; }
    public string? Slug { get; set; }
    public string? Domain { get; set; }
    public string? Status { get; set; }
    public string? CnpjCpf { get; set; }
    public string? RazaoSocial { get; set; }
    public string? InscricaoEstadual { get; set; }
    public string? InscricaoMunicipal { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Website { get; set; }
    public string? AddressStreet { get; set; }
    public string? AddressNumber { get; set; }
    public string? AddressComplement { get; set; }
    public string? AddressNeighborhood { get; set; }
    public string? AddressCity { get; set; }
    public string? AddressState { get; set; }
    public string? AddressZipcode { get; set; }
    public string? AddressCountry { get; set; }
    public string? BillingStreet { get; set; }
    public string? BillingNumber { get; set; }
    public string? BillingComplement { get; set; }
    public string? BillingNeighborhood { get; set; }
    public string? BillingCity { get; set; }
    public string? BillingState { get; set; }
    public string? BillingZipcode { get; set; }
    public string? BillingCountry { get; set; }
    public string? LegalRepresentativeName { get; set; }
    public string? LegalRepresentativeCpf { get; set; }
    public string? LegalRepresentativeEmail { get; set; }
    public string? LegalRepresentativePhone { get; set; }
    public Guid? ActiveSubscriptionId { get; set; }
    public Dictionary<string, object>? Settings { get; set; }
}

public sealed class RegisterTenantRequestDto
{
    public string CompanyName { get; set; } = string.Empty;
    public string? Document { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;
}

public sealed class RegisterTenantResponseDto
{
    public Guid TenantId { get; set; }
    public Guid UserId { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public int ExpiresIn { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string Message { get; set; } = "Tenant cadastrado com sucesso!";
    public bool RedirectToPlanSelection { get; set; }
}
