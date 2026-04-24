namespace OpaMenu.Domain.DTOs.AccessControl;

public sealed class PermissionOperationBulkRequestDto
{
    public Guid PermissionId { get; set; }
    public List<string> OperationIds { get; set; } = [];
}
