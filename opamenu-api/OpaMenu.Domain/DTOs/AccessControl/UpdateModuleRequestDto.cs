namespace OpaMenu.Domain.DTOs.AccessControl;

public sealed class UpdateModuleRequestDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string Key { get; set; } = string.Empty;
    public string? Code { get; set; }
    public Guid? ApplicationId { get; set; }
    public Guid? ModuleTypeId { get; set; }
    public bool IsActive { get; set; }
}
