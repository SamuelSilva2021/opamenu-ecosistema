using Microsoft.EntityFrameworkCore;
using Opamenu.Domain.Interfaces;
using Opamenu.Infrastructure.Shared.Data.Context.AccessControl;
using Opamenu.Infrastructure.Shared.Entities.AccessControl;

namespace Opamenu.Infrastructure.Repositories;

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

