namespace OpaMenu.Domain.DTOs.AccessControl;

public sealed class RolePainelDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Code { get; set; }
    public bool IsDefault { get; set; }
    public bool IsActive { get; set; }
    public List<SimplifiedPermissionDto> Permissions { get; set; } = [];
}
