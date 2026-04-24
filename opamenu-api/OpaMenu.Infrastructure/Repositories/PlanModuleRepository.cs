using Microsoft.EntityFrameworkCore;
using OpaMenu.Domain.Interfaces;
using OpaMenu.Infrastructure.Shared.Data.Context.MultTenant;
using OpaMenu.Infrastructure.Shared.Entities.MultiTenant.PlanModule;

namespace OpaMenu.Infrastructure.Repositories;

public sealed class PlanModuleRepository(MultiTenantDbContext dbContext) : IPlanModuleRepository
{
    private readonly MultiTenantDbContext _dbContext = dbContext;

    public async Task<IReadOnlyList<Guid>> GetModuleIdsByPlanIdAsync(Guid planId)
    {
        return await _dbContext.Set<PlanModuleEntity>()
            .AsNoTracking()
            .Where(pm => pm.PlanId == planId)
            .Select(pm => pm.ModuleId)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<PlanModuleEntity>> GetByPlanIdAsync(Guid planId)
    {
        return await _dbContext.Set<PlanModuleEntity>()
            .AsNoTracking()
            .Where(pm => pm.PlanId == planId)
            .ToListAsync();
    }
}

