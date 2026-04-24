namespace OpaMenu.Domain.DTOs.AccessControl;

public sealed class AvailableModuleDto
{
    public string Key { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<string> AvailableActions { get; set; } = [];
}
