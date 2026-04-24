namespace OpaMenu.Domain.DTOs.AccessControl;

public sealed class CreateRoleRequestDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? Code { get; set; }
    public Guid? TenantId { get; set; }
    public Guid? ApplicationId { get; set; }
    public List<SimplifiedPermissionDto>? Permissions { get; set; }
    public bool? IsActive { get; set; }
}
