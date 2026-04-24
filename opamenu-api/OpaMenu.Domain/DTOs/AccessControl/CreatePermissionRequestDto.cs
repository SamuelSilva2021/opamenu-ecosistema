namespace OpaMenu.Domain.DTOs.AccessControl;

public sealed class CreatePermissionRequestDto
{
    public Guid? TenantId { get; set; }
    public Guid? RoleId { get; set; }
    public Guid ModuleId { get; set; }
    public List<string>? OperationIds { get; set; }
    public bool? IsActive { get; set; }
}
