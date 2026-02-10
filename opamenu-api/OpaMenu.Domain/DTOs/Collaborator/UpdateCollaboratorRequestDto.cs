using OpaMenu.Infrastructure.Shared.Enums.Opamenu;

namespace OpaMenu.Domain.DTOs.Collaborator;

public class UpdateCollaboratorRequestDto
{
    public string? Name { get; set; }
    public string? Phone { get; set; }
    public ECollaboratorType? Type { get; set; }
    public string? Role { get; set; }
    public bool? Active { get; set; }
    public Guid? UserAccountId { get; set; }
}
