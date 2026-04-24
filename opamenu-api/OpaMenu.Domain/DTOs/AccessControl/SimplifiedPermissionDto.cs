namespace OpaMenu.Domain.DTOs.AccessControl;

public sealed class SimplifiedPermissionDto
{
    public string Module { get; set; } = string.Empty;
    public List<string> Actions { get; set; } = [];
}
