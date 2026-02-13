using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OpaMenu.Domain.Interfaces;
using OpaMenu.Infrastructure.Shared.Data.Context;
using OpaMenu.Infrastructure.Shared.Data.Context.Opamenu;
using OpaMenu.Infrastructure.Shared.Entities;
using OpaMenu.Infrastructure.Shared.Entities.Opamenu;

namespace OpaMenu.Infrastructure.Repositories;

public class LoyaltyProgramRepository(OpamenuDbContext context) : OpamenuRepository<LoyaltyProgramEntity>(context), ILoyaltyProgramRepository
{
    public async Task<IEnumerable<LoyaltyProgramEntity>> GetByTenantIdAsync(Guid tenantId)
    {
        return await _dbSet
            .Include(p => p.Filters)
            .Where(p => p.TenantId == tenantId)
            .ToListAsync();
    }
}
