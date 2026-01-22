using OpaMenu.Infrastructure.Shared.Entities.MultiTenant.PlanModule;

namespace Authenticator.API.Core.Application.Interfaces.MultiTenant
{
    public interface IPlanModuleRepository : IBaseRepository<PlanModuleEntity>
    {
        Task<IEnumerable<PlanModuleEntity>> GetByPlanIdAsync(Guid planId);
    }
}
