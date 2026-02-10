using OpaMenu.Infrastructure.Shared.Enums.Opamenu;

namespace OpaMenu.Domain.DTOs.Collaborator;

public class CollaboratorResponseDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public ECollaboratorType Type { get; set; }
    public string? Role { get; set; }
    public bool Active { get; set; }
    public Guid? UserAccountId { get; set; }
    public Guid TenantId { get; set; }
    public DateTime CreatedAt { get; set; }
}
