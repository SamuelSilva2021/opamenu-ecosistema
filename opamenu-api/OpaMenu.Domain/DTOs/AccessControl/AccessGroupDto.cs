namespace OpaMenu.Domain.DTOs.AccessControl;

public sealed class AccessGroupDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Code { get; set; }
    public Guid? TenantId { get; set; }
    public Guid? GroupTypeId { get; set; }
    public string? GroupTypeName { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
