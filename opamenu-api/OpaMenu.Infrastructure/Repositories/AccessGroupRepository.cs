using Microsoft.EntityFrameworkCore;
using OpaMenu.Domain.Interfaces;
using OpaMenu.Infrastructure.Shared.Data.Context.AccessControl;
using OpaMenu.Infrastructure.Shared.Entities.AccessControl;

namespace OpaMenu.Infrastructure.Repositories;

public sealed class AccessGroupRepository(AccessControlDbContext dbContext) : IAccessGroupRepository
{
    private readonly AccessControlDbContext _dbContext = dbContext;

    public async Task<IReadOnlyList<Guid>> GetExistingIdsAsync(IReadOnlyCollection<Guid> ids)
    {
        if (ids.Count == 0)
        {
            return [];
        }

        return await _dbContext.AccessGroups.AsNoTracking()
            .Where(g => ids.Contains(g.Id))
            .Select(g => g.Id)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<AccessGroupEntity>> GetActiveGroupsWithTypeByUserIdAsync(Guid userId)
    {
        return await _dbContext.AccountAccessGroups
            .AsNoTracking()
            .Where(aag => aag.UserAccountId == userId && aag.IsActive)
            .Include(aag => aag.AccessGroup)
            .ThenInclude(ag => ag.GroupType)
            .Select(aag => aag.AccessGroup)
            .Where(ag => ag.IsActive)
            .ToListAsync();
    }
}

