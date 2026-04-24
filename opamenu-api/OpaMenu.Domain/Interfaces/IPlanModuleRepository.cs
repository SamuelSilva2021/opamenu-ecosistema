using OpaMenu.Infrastructure.Shared.Entities.MultiTenant.PlanModule;

namespace OpaMenu.Domain.Interfaces;

public interface IPlanModuleRepository
{
    Task<IReadOnlyList<Guid>> GetModuleIdsByPlanIdAsync(Guid planId);
    Task<IReadOnlyList<PlanModuleEntity>> GetByPlanIdAsync(Guid planId);
}

