using OpaMenu.Infrastructure.Shared.Entities.AccessControl;

namespace OpaMenu.Domain.Interfaces;

public interface IAccessGroupRepository
{
    Task<IReadOnlyList<Guid>> GetExistingIdsAsync(IReadOnlyCollection<Guid> ids);
    Task<IReadOnlyList<AccessGroupEntity>> GetActiveGroupsWithTypeByUserIdAsync(Guid userId);
}

