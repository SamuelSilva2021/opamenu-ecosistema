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

    public Task<TenantModuleEntity?> GetByTenantAndModuleTrackedAsync(Guid tenantId, Guid moduleId) =>
        _dbContext.Set<TenantModuleEntity>().FirstOrDefaultAsync(tm => tm.TenantId == tenantId && tm.ModuleId == moduleId);

    public Task AddAsync(TenantModuleEntity entity)
    {
        _dbContext.Set<TenantModuleEntity>().Add(entity);
        return Task.CompletedTask;
    }

    public Task SaveChangesAsync() => _dbContext.SaveChangesAsync();
}

