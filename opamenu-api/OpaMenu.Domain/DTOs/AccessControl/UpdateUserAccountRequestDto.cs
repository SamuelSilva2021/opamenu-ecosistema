namespace OpaMenu.Domain.DTOs.AccessControl;

public sealed class UpdateUserAccountRequestDto
{
    public string? Username { get; set; }
    public string? Email { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Status { get; set; }
    public bool? IsEmailVerified { get; set; }
    public Guid? TenantId { get; set; }
    public Guid? RoleId { get; set; }
    public DateTime? CreatedAt { get; set; }
}
