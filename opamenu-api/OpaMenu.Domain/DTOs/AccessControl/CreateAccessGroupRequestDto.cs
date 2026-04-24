namespace OpaMenu.Domain.DTOs.AccessControl;

public sealed class CreateAccessGroupRequestDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? Code { get; set; }
    public Guid? TenantId { get; set; }
    public Guid GroupTypeId { get; set; }
}
