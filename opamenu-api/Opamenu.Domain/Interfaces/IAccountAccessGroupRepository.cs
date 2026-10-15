using Opamenu.Infrastructure.Shared.Entities.AccessControl;

namespace Opamenu.Domain.Interfaces;

public interface IAccountAccessGroupRepository
{
    Task<IReadOnlyList<AccountAccessGroupEntity>> GetByUserIdAsync(Guid userId);
    Task<AccountAccessGroupEntity?> GetByUserIdAndGroupIdAsync(Guid userId, Guid groupId);
    Task AddAsync(AccountAccessGroupEntity entity);
    Task SaveChangesAsync();
}

