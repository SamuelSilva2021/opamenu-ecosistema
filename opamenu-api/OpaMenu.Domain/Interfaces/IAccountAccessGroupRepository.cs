using OpaMenu.Infrastructure.Shared.Entities.AccessControl;

namespace OpaMenu.Domain.Interfaces;

public interface IAccountAccessGroupRepository
{
    Task<IReadOnlyList<AccountAccessGroupEntity>> GetByUserIdAsync(Guid userId);
    Task<AccountAccessGroupEntity?> GetByUserIdAndGroupIdAsync(Guid userId, Guid groupId);
    Task AddAsync(AccountAccessGroupEntity entity);
    Task SaveChangesAsync();
}

