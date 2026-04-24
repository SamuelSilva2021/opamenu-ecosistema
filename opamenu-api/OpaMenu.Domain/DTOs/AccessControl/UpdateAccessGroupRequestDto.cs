namespace OpaMenu.Domain.DTOs.AccessControl;

public sealed class UpdateAccessGroupRequestDto
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? Code { get; set; }
    public Guid? GroupTypeId { get; set; }
    public Guid? TenantId { get; set; }
    public bool? IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}
