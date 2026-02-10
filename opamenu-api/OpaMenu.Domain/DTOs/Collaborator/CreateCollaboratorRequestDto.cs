using System.ComponentModel.DataAnnotations;
using OpaMenu.Infrastructure.Shared.Enums.Opamenu;

namespace OpaMenu.Domain.DTOs.Collaborator;

public class CreateCollaboratorRequestDto
{
    [Required]
    public string Name { get; set; } = string.Empty;
    public string? Phone { get; set; }
    [Required]
    public ECollaboratorType Type { get; set; }
    public string? Role { get; set; }
    public Guid? UserAccountId { get; set; }
}
