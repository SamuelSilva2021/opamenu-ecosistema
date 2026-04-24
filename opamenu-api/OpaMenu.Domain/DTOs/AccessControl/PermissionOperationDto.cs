namespace OpaMenu.Domain.DTOs.AccessControl;

public sealed class PermissionOperationDto
{
    public Guid Id { get; set; }
    public Guid PermissionId { get; set; }
    public string OperationId { get; set; } = string.Empty;
    public string PermissionName { get; set; } = string.Empty;
    public string OperationName { get; set; } = string.Empty;
    public string OperationCode { get; set; } = string.Empty;
    public string? OperationDescription { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
