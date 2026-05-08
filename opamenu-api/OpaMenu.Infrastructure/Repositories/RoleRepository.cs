using Microsoft.EntityFrameworkCore;
using OpaMenu.Domain.Interfaces;
using OpaMenu.Infrastructure.Shared.Data.Context.AccessControl;
using OpaMenu.Infrastructure.Shared.Entities.AccessControl;

namespace OpaMenu.Infrastructure.Repositories;

public sealed class RoleRepository(AccessControlDbContext dbContext) : IRoleRepository
{
    private readonly AccessControlDbContext _dbContext = dbContext;

    public async Task<(IReadOnlyList<RoleEntity> Items, int Total)> GetPagedAsync(int page, int limit, string? search)
    {
        page = page <= 0 ? 1 : page;
        limit = limit <= 0 ? 10 : limit;

        var query = _dbContext.Roles.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.Trim();
            query = query.Where(r =>
                r.Name.Contains(s) ||
                r.Description.Contains(s) ||
                (r.Code != null && r.Code.Contains(s)));
        }

        var total = await query.CountAsync();

        var items = await query
            .OrderByDescending(r => r.CreatedAt)
            .Skip((page - 1) * limit)
            .Take(limit)
            .ToListAsync();

        return (items, total);
    }

    public async Task<(IReadOnlyList<RoleEntity> Items, int Total)> GetPagedForTenantAsync(Guid tenantId, int page, int limit, string? search)
    {
        page = page <= 0 ? 1 : page;
        limit = limit <= 0 ? 10 : limit;

        var query = _dbContext.Roles.AsNoTracking()
            .Where(r => r.TenantId == tenantId || (r.TenantId == null && r.IsSystem))
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.Trim();
            query = query.Where(r => r.Name.Contains(s) || (r.Code != null && r.Code.Contains(s)));
        }

        var total = await query.CountAsync();

        var items = await query
            .OrderByDescending(r => r.CreatedAt)
            .Skip((page - 1) * limit)
            .Take(limit)
            .ToListAsync();

        return (items, total);
    }

    public Task<RoleEntity?> GetByIdAsync(Guid id) =>
        _dbContext.Roles.AsNoTracking().FirstOrDefaultAsync(r => r.Id == id);

    public Task<RoleEntity?> GetByIdForTenantAsync(Guid tenantId, Guid id) =>
        _dbContext.Roles.AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == id && (r.TenantId == tenantId || (r.TenantId == null && r.IsSystem)));

    public Task<RoleEntity?> GetByIdTrackedAsync(Guid id) =>
        _dbContext.Roles.FirstOrDefaultAsync(r => r.Id == id);

    public Task<RoleEntity?> GetByIdForTenantTrackedAsync(Guid tenantId, Guid id) =>
        _dbContext.Roles.FirstOrDefaultAsync(r => r.Id == id && r.TenantId == tenantId);

    public Task<RoleEntity?> GetActiveAdminRoleForTenantAsync(Guid tenantId) =>
        _dbContext.Roles
            .IgnoreQueryFilters()
            .AsNoTracking()
            .Where(r => r.IsActive && (r.TenantId == null || r.TenantId == tenantId) && r.Code != null && r.Code.ToUpper() == "ADMIN")
            .OrderByDescending(r => r.IsSystem)
            .FirstOrDefaultAsync();

    public async Task<bool> CodeExistsAsync(string code, Guid? excludeRoleId = null, Guid? tenantId = null, bool includeNullTenant = true)
    {
        var query = _dbContext.Roles.AsNoTracking().Where(r => r.Code != null && r.Code == code);

        if (excludeRoleId.HasValue)
        {
            query = query.Where(r => r.Id != excludeRoleId.Value);
        }

        if (tenantId.HasValue)
        {
            if (includeNullTenant)
            {
                query = query.Where(r => r.TenantId == tenantId.Value || r.TenantId == null);
            }
            else
            {
                query = query.Where(r => r.TenantId == tenantId.Value);
            }
        }

        return await query.AnyAsync();
    }

    public async Task AddAsync(RoleEntity role)
    {
        _dbContext.Roles.Add(role);
        await Task.CompletedTask;
    }

    public async Task UpdateAsync(RoleEntity role)
    {
        _dbContext.Roles.Update(role);
        await Task.CompletedTask;
    }

    public async Task DeleteAsync(RoleEntity role)
    {
        _dbContext.Roles.Remove(role);
        await Task.CompletedTask;
    }

    public Task SaveChangesAsync() => _dbContext.SaveChangesAsync();
}
