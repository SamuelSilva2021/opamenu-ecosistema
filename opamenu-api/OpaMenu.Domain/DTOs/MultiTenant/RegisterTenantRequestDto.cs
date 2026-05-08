namespace OpaMenu.Domain.DTOs.MultiTenant;

public sealed class RegisterTenantRequestDto
{
    public string CompanyName { get; set; } = string.Empty;
    public string? Document { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;
    public Guid? ParentTenantId { get; set; }
}
