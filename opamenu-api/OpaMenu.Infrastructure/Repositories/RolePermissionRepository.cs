using Microsoft.EntityFrameworkCore;
using OpaMenu.Domain.Interfaces;
using OpaMenu.Infrastructure.Shared.Data.Context.AccessControl;
using OpaMenu.Infrastructure.Shared.Entities.AccessControl;

namespace OpaMenu.Infrastructure.Repositories;

public sealed class RolePermissionRepository(AccessControlDbContext dbContext) : IRolePermissionRepository
{
    private readonly AccessControlDbContext _dbContext = dbContext;

    public async Task<(IReadOnlyList<RolePermissionEntity> Items, int Total)> GetPagedAsync(int page, int limit, string? search, Guid? tenantId, string? moduleKey)
    {
        page = page <= 0 ? 1 : page;
        limit = limit <= 0 ? 10 : limit;

        var query = _dbContext.RolePermissions
            .AsNoTracking()
            .Include(rp => rp.Role)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.Trim();
            query = query.Where(rp =>
                rp.ModuleKey.Contains(s) ||
                rp.Role.Name.Contains(s) ||
                (rp.Role.Code != null && rp.Role.Code.Contains(s)));
        }

        if (tenantId.HasValue && tenantId.Value != Guid.Empty)
        {
            query = query.Where(rp => rp.Role.TenantId == tenantId.Value);
        }

        if (!string.IsNullOrWhiteSpace(moduleKey))
        {
            query = query.Where(rp => rp.ModuleKey == moduleKey);
        }

        var total = await query.CountAsync();
        var items = await query
            .OrderByDescending(rp => rp.CreatedAt)
            .Skip((page - 1) * limit)
            .Take(limit)
            .ToListAsync();

        return (items, total);
    }

    public async Task<IReadOnlyList<RolePermissionEntity>> GetByIdsWithRoleAsync(IReadOnlyCollection<Guid> ids)
    {
        if (ids.Count == 0)
        {
            return [];
        }

        return await _dbContext.RolePermissions
            .AsNoTracking()
            .Include(rp => rp.Role)
            .Where(rp => ids.Contains(rp.Id))
            .ToListAsync();
    }

    public Task<RolePermissionEntity?> GetByIdWithRoleAsync(Guid id) =>
        _dbContext.RolePermissions.AsNoTracking()
            .Include(rp => rp.Role)
            .FirstOrDefaultAsync(rp => rp.Id == id);

    public Task<RolePermissionEntity?> GetByIdTrackedAsync(Guid id) =>
        _dbContext.RolePermissions.FirstOrDefaultAsync(rp => rp.Id == id);

    public Task<RolePermissionEntity?> GetByRoleIdAndModuleKeyTrackedAsync(Guid roleId, string moduleKey) =>
        _dbContext.RolePermissions.FirstOrDefaultAsync(rp => rp.RoleId == roleId && rp.ModuleKey == moduleKey);

    public async Task<IReadOnlyList<RolePermissionEntity>> GetActiveByRoleIdsAsync(IReadOnlyCollection<Guid> roleIds)
    {
        if (roleIds.Count == 0)
        {
            return [];
        }

        return await _dbContext.RolePermissions
            .AsNoTracking()
            .Where(rp => roleIds.Contains(rp.RoleId) && rp.IsActive)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<RolePermissionEntity>> GetByRoleIdAsync(Guid roleId)
    {
        return await _dbContext.RolePermissions
            .Where(rp => rp.RoleId == roleId)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<RolePermissionEntity>> GetActiveByRoleIdAsync(Guid roleId)
    {
        return await _dbContext.RolePermissions
            .AsNoTracking()
            .Where(rp => rp.RoleId == roleId && rp.IsActive)
            .ToListAsync();
    }

    public Task AddAsync(RolePermissionEntity entity)
    {
        _dbContext.RolePermissions.Add(entity);
        return Task.CompletedTask;
    }

    public Task AddRangeAsync(IEnumerable<RolePermissionEntity> permissions)
    {
        _dbContext.RolePermissions.AddRange(permissions);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(RolePermissionEntity entity)
    {
        _dbContext.RolePermissions.Remove(entity);
        return Task.CompletedTask;
    }

    public Task RemoveRangeAsync(IEnumerable<RolePermissionEntity> permissions)
    {
        _dbContext.RolePermissions.RemoveRange(permissions);
        return Task.CompletedTask;
    }

    public Task SaveChangesAsync() => _dbContext.SaveChangesAsync();
}
