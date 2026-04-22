namespace OpaMenu.Domain.DTOs.Auth;

public sealed class UserInfoDto
{
    public Guid Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public UserPermissionsDto Permissions { get; set; } = new();
    public SimplifiedRoleDto? Role { get; set; }
    public TenantInfoDto? Tenant { get; set; }
}

