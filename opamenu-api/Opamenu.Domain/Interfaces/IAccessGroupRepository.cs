using Opamenu.Infrastructure.Shared.Entities.AccessControl;

namespace Opamenu.Domain.Interfaces;

public interface IAccessGroupRepository
{
    Task<IReadOnlyList<Guid>> GetExistingIdsAsync(IReadOnlyCollection<Guid> ids);
    Task<IReadOnlyList<AccessGroupEntity>> GetActiveGroupsWithTypeByUserIdAsync(Guid userId);
    Task<(IReadOnlyList<AccessGroupEntity> Items, int Total)> GetPagedWithTypeAsync(int page, int limit, string? search);
    Task<AccessGroupEntity?> GetByIdWithTypeAsync(Guid id);
    Task<AccessGroupEntity?> GetByIdTrackedAsync(Guid id);
    Task AddAsync(AccessGroupEntity entity);
    Task DeleteAsync(AccessGroupEntity entity);
    Task SaveChangesAsync();
}
