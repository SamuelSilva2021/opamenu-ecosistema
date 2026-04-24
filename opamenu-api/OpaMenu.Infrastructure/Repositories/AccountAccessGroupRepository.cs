using Microsoft.EntityFrameworkCore;
using OpaMenu.Domain.Interfaces;
using OpaMenu.Infrastructure.Shared.Data.Context.AccessControl;
using OpaMenu.Infrastructure.Shared.Entities.AccessControl;

namespace OpaMenu.Infrastructure.Repositories;

public sealed class AccountAccessGroupRepository(AccessControlDbContext dbContext) : IAccountAccessGroupRepository
{
    private readonly AccessControlDbContext _dbContext = dbContext;

    public async Task<IReadOnlyList<AccountAccessGroupEntity>> GetByUserIdAsync(Guid userId)
    {
        return await _dbContext.AccountAccessGroups
            .Where(aag => aag.UserAccountId == userId)
            .ToListAsync();
    }

    public Task<AccountAccessGroupEntity?> GetByUserIdAndGroupIdAsync(Guid userId, Guid groupId) =>
        _dbContext.AccountAccessGroups.FirstOrDefaultAsync(aag => aag.UserAccountId == userId && aag.AccessGroupId == groupId);

    public Task AddAsync(AccountAccessGroupEntity entity)
    {
        _dbContext.AccountAccessGroups.Add(entity);
        return Task.CompletedTask;
    }

    public Task SaveChangesAsync() => _dbContext.SaveChangesAsync();
}

