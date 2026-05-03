using Microsoft.EntityFrameworkCore;
using OpaMenu.Domain.Interfaces;
using OpaMenu.Infrastructure.Shared.Data.Context.MultTenant;
using OpaMenu.Infrastructure.Shared.Entities.MultiTenant.PlanModule;

namespace OpaMenu.Infrastructure.Repositories;

public sealed class PlanModuleRepository(MultiTenantDbContext dbContext) : IPlanModuleRepository
{
    private readonly MultiTenantDbContext _dbContext = dbContext;

    public async Task<IReadOnlyList<Guid>> GetModuleIdsByPlanIdAsync(Guid planId) =>
        await _dbContext.Set<PlanModuleEntity>()
            .AsNoTracking()
            .Where(pm => pm.PlanId == planId)
            .Select(pm => pm.ModuleId)
            .ToListAsync();

    public async Task<IReadOnlyList<PlanModuleEntity>> GetByPlanIdAsync(Guid planId) =>
        await _dbContext.Set<PlanModuleEntity>()
            .AsNoTracking()
            .Where(pm => pm.PlanId == planId)
            .ToListAsync();

    public async Task AddRangeAsync(IEnumerable<PlanModuleEntity> entities) =>
        await _dbContext.Set<PlanModuleEntity>().AddRangeAsync(entities);

    public async Task RemoveRangeAsync(IEnumerable<PlanModuleEntity> entities)
    {
        _dbContext.Set<PlanModuleEntity>().RemoveRange(entities);
        await Task.CompletedTask;
    }

    public async Task SaveChangesAsync() => await _dbContext.SaveChangesAsync();
}

