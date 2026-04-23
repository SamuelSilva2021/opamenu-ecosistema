namespace OpaMenu.Domain.DTOs.AccessControl;

public sealed class PermissionDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Code { get; set; }
    public Guid? TenantId { get; set; }
    public Guid? RoleId { get; set; }
    public Guid? ModuleId { get; set; }
    public string? ModuleName { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public List<OperationDto>? Operations { get; set; }
}

public sealed class CreatePermissionRequestDto
{
    public Guid? TenantId { get; set; }
    public Guid? RoleId { get; set; }
    public Guid ModuleId { get; set; }
    public List<string>? OperationIds { get; set; }
    public bool? IsActive { get; set; }
}

public sealed class UpdatePermissionRequestDto
{
    public Guid? TenantId { get; set; }
    public Guid? RoleId { get; set; }
    public Guid ModuleId { get; set; }
    public List<string>? OperationIds { get; set; }
    public bool? IsActive { get; set; }
}

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

public sealed class PermissionOperationBulkRequestDto
{
    public Guid PermissionId { get; set; }
    public List<string> OperationIds { get; set; } = [];
}

