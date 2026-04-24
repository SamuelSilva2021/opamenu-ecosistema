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

    public async Task<(IReadOnlyList<AccessGroupEntity> Items, int Total)> GetPagedWithTypeAsync(int page, int limit, string? search)
    {
        page = page <= 0 ? 1 : page;
        limit = limit <= 0 ? 10 : limit;

        var query = _dbContext.AccessGroups.AsNoTracking()
            .Include(g => g.GroupType)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.Trim();
            query = query.Where(g =>
                g.Name.Contains(s) ||
                g.Description.Contains(s) ||
                (g.Code != null && g.Code.Contains(s)));
        }

        var total = await query.CountAsync();
        var items = await query
            .OrderByDescending(g => g.CreatedAt)
            .Skip((page - 1) * limit)
            .Take(limit)
            .ToListAsync();

        return (items, total);
    }

    public Task<AccessGroupEntity?> GetByIdWithTypeAsync(Guid id) =>
        _dbContext.AccessGroups.AsNoTracking()
            .Include(g => g.GroupType)
            .FirstOrDefaultAsync(g => g.Id == id);

    public Task<AccessGroupEntity?> GetByIdTrackedAsync(Guid id) =>
        _dbContext.AccessGroups.FirstOrDefaultAsync(g => g.Id == id);

    public Task AddAsync(AccessGroupEntity entity)
    {
        _dbContext.AccessGroups.Add(entity);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(AccessGroupEntity entity)
    {
        _dbContext.AccessGroups.Remove(entity);
        return Task.CompletedTask;
    }

    public Task SaveChangesAsync() => _dbContext.SaveChangesAsync();
}
