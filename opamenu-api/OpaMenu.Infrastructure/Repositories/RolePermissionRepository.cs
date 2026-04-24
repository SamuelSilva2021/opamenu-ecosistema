using Microsoft.EntityFrameworkCore;
using OpaMenu.Domain.Interfaces;
using OpaMenu.Infrastructure.Shared.Data.Context.AccessControl;
using OpaMenu.Infrastructure.Shared.Entities.AccessControl;

namespace OpaMenu.Infrastructure.Repositories;

public sealed class RolePermissionRepository(AccessControlDbContext dbContext) : IRolePermissionRepository
{
    private readonly AccessControlDbContext _dbContext = dbContext;

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

    public Task AddRangeAsync(IEnumerable<RolePermissionEntity> permissions)
    {
        _dbContext.RolePermissions.AddRange(permissions);
        return Task.CompletedTask;
    }

    public Task RemoveRangeAsync(IEnumerable<RolePermissionEntity> permissions)
    {
        _dbContext.RolePermissions.RemoveRange(permissions);
        return Task.CompletedTask;
    }

    public Task SaveChangesAsync() => _dbContext.SaveChangesAsync();
}
