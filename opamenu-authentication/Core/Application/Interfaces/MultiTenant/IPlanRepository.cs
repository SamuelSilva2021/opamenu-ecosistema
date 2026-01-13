using OpaMenu.Infrastructure.Shared.Entities.MultiTenant.Plan;

namespace Authenticator.API.Core.Application.Interfaces.MultiTenant
{
    public interface IPlanRepository : IBaseRepository<PlanEntity>
    {
        Task<PlanEntity?> GetDefaultPlanAsync();
    }
}

