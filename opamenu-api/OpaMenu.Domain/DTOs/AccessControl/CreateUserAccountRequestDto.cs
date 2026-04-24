namespace OpaMenu.Domain.DTOs.AccessControl;

public sealed class CreateUserAccountRequestDto
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public Guid? TenantId { get; set; }
    public Guid? RoleId { get; set; }
}
