using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Opamenu.Domain.Interfaces;
using Opamenu.Infrastructure.Shared.Data.Context;
using Opamenu.Infrastructure.Shared.Data.Context.Opamenu;
using Opamenu.Infrastructure.Shared.Entities;
using Opamenu.Infrastructure.Shared.Entities.Opamenu;

namespace Opamenu.Infrastructure.Repositories;

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
