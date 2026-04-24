using OpaMenu.Domain.DTOs.AccessControl;

namespace OpaMenu.Application.Services.Interfaces.AccessControl;

public interface IAccessGroupService
{
    Task<PagedResultDto<AccessGroupDto>> GetGroupsAsync(int page, int limit, string? search);
    Task<AccessGroupDto?> GetGroupByIdAsync(Guid id);
    Task<AccessGroupDto?> CreateGroupAsync(CreateAccessGroupRequestDto request);
    Task<(AccessGroupDto? Group, bool NotFound, bool BadRequest)> UpdateGroupAsync(Guid id, UpdateAccessGroupRequestDto request);
    Task DeleteGroupAsync(Guid id);

    Task<IReadOnlyList<GroupTypeDto>> GetGroupTypesAsync();
    Task<GroupTypeDto?> GetGroupTypeByIdAsync(Guid id);
    Task<GroupTypeDto?> CreateGroupTypeAsync(CreateGroupTypeRequestDto request);
    Task<(GroupTypeDto? GroupType, bool NotFound, bool BadRequest)> UpdateGroupTypeAsync(Guid id, UpdateGroupTypeRequestDto request);
    Task DeleteGroupTypeAsync(Guid id);
}

