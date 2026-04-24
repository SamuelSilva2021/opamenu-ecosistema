using Microsoft.EntityFrameworkCore;
using OpaMenu.Domain.Interfaces;
using OpaMenu.Infrastructure.Shared.Data.Context.MultTenant;
using OpaMenu.Infrastructure.Shared.Entities.MultiTenant.TenantModule;

namespace OpaMenu.Infrastructure.Repositories;

public sealed class TenantModuleRepository(MultiTenantDbContext dbContext) : ITenantModuleRepository
{
    private readonly MultiTenantDbContext _dbContext = dbContext;

    public async Task<IReadOnlyList<Guid>> GetEnabledModuleIdsAsync(Guid tenantId)
    {
        return await _dbContext.Set<TenantModuleEntity>()
            .AsNoTracking()
            .Where(tm => tm.TenantId == tenantId && tm.IsEnabled)
            .Select(tm => tm.ModuleId)
            .ToListAsync();
    }

    public Task<List<TenantModuleEntity>> GetByTenantTrackedAsync(Guid tenantId) =>
        _dbContext.Set<TenantModuleEntity>().Where(tm => tm.TenantId == tenantId).ToListAsync();

    public Task<TenantModuleEntity?> GetByTenantAndModuleTrackedAsync(Guid tenantId, Guid moduleId) =>
        _dbContext.Set<TenantModuleEntity>().FirstOrDefaultAsync(tm => tm.TenantId == tenantId && tm.ModuleId == moduleId);

    public Task AddAsync(TenantModuleEntity entity)
    {
        _dbContext.Set<TenantModuleEntity>().Add(entity);
        return Task.CompletedTask;
    }

    public Task AddRangeAsync(IEnumerable<TenantModuleEntity> entities)
    {
        _dbContext.Set<TenantModuleEntity>().AddRange(entities);
        return Task.CompletedTask;
    }

    public Task RemoveRangeAsync(IEnumerable<TenantModuleEntity> entities)
    {
        _dbContext.Set<TenantModuleEntity>().RemoveRange(entities);
        return Task.CompletedTask;
    }

    public Task SaveChangesAsync() => _dbContext.SaveChangesAsync();
}
