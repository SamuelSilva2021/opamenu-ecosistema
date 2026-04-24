using Microsoft.EntityFrameworkCore;
using OpaMenu.Domain.Interfaces;
using OpaMenu.Infrastructure.Shared.Data.Context.AccessControl;
using OpaMenu.Infrastructure.Shared.Entities.AccessControl;

namespace OpaMenu.Infrastructure.Repositories;

public sealed class ApplicationRepository(AccessControlDbContext dbContext) : IApplicationRepository
{
    private readonly AccessControlDbContext _dbContext = dbContext;

    public async Task<(IReadOnlyList<ApplicationEntity> Items, int Total)> GetPagedAsync(int page, int limit, string? search)
    {
        page = page <= 0 ? 1 : page;
        limit = limit <= 0 ? 10 : limit;

        var query = _dbContext.Applications.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.Trim();
            query = query.Where(a =>
                a.Name.Contains(s) ||
                a.Description.Contains(s) ||
                (a.Code != null && a.Code.Contains(s)));
        }

        var total = await query.CountAsync();
        var items = await query
            .OrderBy(a => a.Name)
            .Skip((page - 1) * limit)
            .Take(limit)
            .ToListAsync();

        return (items, total);
    }

    public Task<ApplicationEntity?> GetByIdAsync(Guid id) =>
        _dbContext.Applications.AsNoTracking().FirstOrDefaultAsync(a => a.Id == id);

    public Task<ApplicationEntity?> GetByIdTrackedAsync(Guid id) =>
        _dbContext.Applications.FirstOrDefaultAsync(a => a.Id == id);

    public Task AddAsync(ApplicationEntity entity)
    {
        _dbContext.Applications.Add(entity);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(ApplicationEntity entity)
    {
        _dbContext.Applications.Remove(entity);
        return Task.CompletedTask;
    }

    public Task SaveChangesAsync() => _dbContext.SaveChangesAsync();
}

