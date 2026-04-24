namespace OpaMenu.Domain.DTOs.AccessControl;

public sealed class CreateGroupTypeRequestDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Code { get; set; } = string.Empty;
    public bool? IsActive { get; set; }
}
