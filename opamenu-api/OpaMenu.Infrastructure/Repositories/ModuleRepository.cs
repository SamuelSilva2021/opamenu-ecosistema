using Microsoft.EntityFrameworkCore;
using OpaMenu.Domain.Interfaces;
using OpaMenu.Infrastructure.Shared.Data.Context.AccessControl;
using OpaMenu.Infrastructure.Shared.Entities.AccessControl;

namespace OpaMenu.Infrastructure.Repositories;

public sealed class ModuleRepository(AccessControlDbContext dbContext) : IModuleRepository
{
    private readonly AccessControlDbContext _dbContext = dbContext;

    public Task<string?> GetKeyByIdAsync(Guid id) =>
        _dbContext.Modules.AsNoTracking()
            .Where(m => m.Id == id)
            .Select(m => m.Key)
            .FirstOrDefaultAsync();

    public Task<ModuleEntity?> GetByIdAsync(Guid id) =>
        _dbContext.Modules.AsNoTracking().FirstOrDefaultAsync(m => m.Id == id);

    public Task<ModuleEntity?> GetByIdWithApplicationAsync(Guid id) =>
        _dbContext.Modules.AsNoTracking()
            .Include(m => m.Application)
            .FirstOrDefaultAsync(m => m.Id == id);

    public Task<ModuleEntity?> GetByIdTrackedAsync(Guid id) =>
        _dbContext.Modules.FirstOrDefaultAsync(m => m.Id == id);

    public async Task<IReadOnlyList<ModuleEntity>> GetByIdsWithApplicationAsync(IReadOnlyCollection<Guid> ids)
    {
        if (ids.Count == 0)
        {
            return [];
        }

        return await _dbContext.Modules.AsNoTracking()
            .Include(m => m.Application)
            .Where(m => ids.Contains(m.Id))
            .ToListAsync();
    }

    public async Task<(IReadOnlyList<ModuleEntity> Items, int Total)> GetPagedAsync(int page, int limit, string? search, bool? isActive, string? sortBy, string? sortOrder)
    {
        page = page <= 0 ? 1 : page;
        limit = limit <= 0 ? 10 : limit;

        var query = _dbContext.Modules
            .AsNoTracking()
            .Include(m => m.Application)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.Trim();
            query = query.Where(m =>
                m.Name.Contains(s) ||
                m.Description.Contains(s) ||
                (m.Key != null && m.Key.Contains(s)) ||
                (m.Code != null && m.Code.Contains(s)));
        }

        if (isActive.HasValue)
        {
            query = query.Where(m => m.IsActive == isActive.Value);
        }

        query = ApplyModuleSorting(query, sortBy, sortOrder);

        var total = await query.CountAsync();
        var items = await query
            .Skip((page - 1) * limit)
            .Take(limit)
            .ToListAsync();

        return (items, total);
    }

    public async Task<bool> KeyExistsAsync(string key, Guid? excludeId = null)
    {
        var query = _dbContext.Modules.AsNoTracking().Where(m => m.Key != null && m.Key == key);
        if (excludeId.HasValue)
        {
            query = query.Where(m => m.Id != excludeId.Value);
        }

        return await query.AnyAsync();
    }

    public Task AddAsync(ModuleEntity entity)
    {
        _dbContext.Modules.Add(entity);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(ModuleEntity entity)
    {
        _dbContext.Modules.Remove(entity);
        return Task.CompletedTask;
    }

    public Task SaveChangesAsync() => _dbContext.SaveChangesAsync();

    public async Task<(Guid Id, string Name)?> GetIdAndNameByKeyAsync(string key)
    {
        var module = await _dbContext.Modules.AsNoTracking()
            .Where(m => m.Key != null && m.Key == key)
            .Select(m => new { m.Id, m.Name })
            .FirstOrDefaultAsync();

        return module == null ? null : (module.Id, module.Name);
    }

    public async Task<IReadOnlyList<(Guid Id, string Key, string Name)>> GetIdAndNameByKeysAsync(IReadOnlyCollection<string> keys)
    {
        if (keys.Count == 0)
        {
            return [];
        }

        var modules = await _dbContext.Modules.AsNoTracking()
            .Where(m => m.Key != null && keys.Contains(m.Key))
            .Select(m => new { m.Id, Key = m.Key!, m.Name })
            .ToListAsync();

        return modules.Select(m => (m.Id, m.Key, m.Name)).ToList();
    }

    private static IQueryable<ModuleEntity> ApplyModuleSorting(IQueryable<ModuleEntity> query, string? sortBy, string? sortOrder)
    {
        var desc = string.Equals(sortOrder, "desc", StringComparison.OrdinalIgnoreCase);

        return (sortBy ?? string.Empty).Trim().ToLowerInvariant() switch
        {
            "name" => desc ? query.OrderByDescending(m => m.Name) : query.OrderBy(m => m.Name),
            "key" => desc ? query.OrderByDescending(m => m.Key) : query.OrderBy(m => m.Key),
            "code" => desc ? query.OrderByDescending(m => m.Code) : query.OrderBy(m => m.Code),
            "isactive" => desc ? query.OrderByDescending(m => m.IsActive) : query.OrderBy(m => m.IsActive),
            _ => query.OrderBy(m => m.Name)
        };
    }
}
