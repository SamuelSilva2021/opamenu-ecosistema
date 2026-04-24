namespace OpaMenu.Domain.DTOs.AccessControl;

public sealed class UpdateRoleRequestDto
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? Code { get; set; }
    public bool? IsActive { get; set; }
    public Guid? TenantId { get; set; }
    public Guid? ApplicationId { get; set; }
    public List<SimplifiedPermissionDto>? Permissions { get; set; }
}
