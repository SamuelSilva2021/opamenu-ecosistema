using OpaMenu.Infrastructure.Shared.Entities.MultiTenant.Plan;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OpaMenu.Domain.Interfaces
{
    public interface IPlanRepository
    {
        Task<PlanEntity?> GetByIdAsync(Guid id);
        Task<IEnumerable<PlanEntity>> GetAllActiveAsync();
    }
}

