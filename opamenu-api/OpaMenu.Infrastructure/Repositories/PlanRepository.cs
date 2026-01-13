using Microsoft.EntityFrameworkCore;
using OpaMenu.Domain.Interfaces;
using OpaMenu.Infrastructure.Shared.Entities.MultiTenant.Plan;
using OpaMenu.Infrastructure.Shared.Data.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpaMenu.Infrastructure.Repositories
{
    public class PlanRepository(MultiTenantDbContext context) : IPlanRepository
    {
        private readonly MultiTenantDbContext _context = context;

        public async Task<PlanEntity?> GetByIdAsync(Guid id)
        {
            return await _context.Set<PlanEntity>().FindAsync(id);
        }

        public async Task<IEnumerable<PlanEntity>> GetAllActiveAsync()
        {
            return await _context.Set<PlanEntity>()
                .Where(p => p.Status == EPlanStatus.Ativo)
                .OrderBy(p => p.SortOrder)
                .ToListAsync();
        }
    }
}

