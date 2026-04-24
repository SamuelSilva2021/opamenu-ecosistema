namespace OpaMenu.Domain.DTOs.AccessControl;

public sealed class AssignUserAccessGroupsRequestDto
{
    public List<Guid> AccessGroupIds { get; set; } = [];
}
