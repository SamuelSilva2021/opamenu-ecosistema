using Authenticator.API.Core.Application.Interfaces;
using Authenticator.API.Core.Application.Interfaces.MultiTenant;
using OpaMenu.Infrastructure.Shared.Entities.MultiTenant.PlanModule;
using Microsoft.EntityFrameworkCore;

namespace Authenticator.API.Infrastructure.Repositories.MultiTenant
{
    public class PlanModuleRepository(IDbContextProvider dbContextProvider) : BaseRepository<PlanModuleEntity>(dbContextProvider), IPlanModuleRepository
    {
        public async Task<IEnumerable<PlanModuleEntity>> GetByPlanIdAsync(Guid planId)
        {
            return await FindAsync(pm => pm.PlanId == planId && pm.IsIncluded);
        }
    }
}
