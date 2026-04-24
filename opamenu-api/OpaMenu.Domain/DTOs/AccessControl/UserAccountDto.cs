namespace OpaMenu.Domain.DTOs.AccessControl;

public sealed class UserAccountDto
{
    public Guid Id { get; set; }
    public Guid? TenantId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string Status { get; set; } = string.Empty;
    public bool IsEmailVerified { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public string FullName { get; set; } = string.Empty;
    public Guid? RoleId { get; set; }
    public string? RoleName { get; set; }
}
