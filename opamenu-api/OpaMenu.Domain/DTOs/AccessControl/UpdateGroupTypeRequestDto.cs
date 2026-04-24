namespace OpaMenu.Domain.DTOs.AccessControl;

public sealed class UpdateGroupTypeRequestDto
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? Code { get; set; }
    public bool? IsActive { get; set; }
    public DateTime? CreatedAt { get; set; }
}
