using OpaMenu.Infrastructure.Shared.Entities.AccessControl;

namespace OpaMenu.Domain.Interfaces;

public interface IGroupTypeRepository
{
    Task<IReadOnlyList<GroupTypeEntity>> GetAllAsync();
    Task<GroupTypeEntity?> GetByIdAsync(Guid id);
    Task<GroupTypeEntity?> GetByIdTrackedAsync(Guid id);
    Task<bool> ExistsAsync(Guid id);
    Task<bool> CodeExistsAsync(string code, Guid? excludeId = null);
    Task AddAsync(GroupTypeEntity entity);
    Task DeleteAsync(GroupTypeEntity entity);
    Task SaveChangesAsync();
}

