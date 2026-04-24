namespace OpaMenu.Domain.DTOs.AccessControl;

public sealed class ModuleDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Url { get; set; }
    public string? Key { get; set; }
    public string? Code { get; set; }
    public Guid? ApplicationId { get; set; }
    public Guid? ModuleTypeId { get; set; }
    public string? ModuleTypeName { get; set; }
    public string? ApplicationName { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
