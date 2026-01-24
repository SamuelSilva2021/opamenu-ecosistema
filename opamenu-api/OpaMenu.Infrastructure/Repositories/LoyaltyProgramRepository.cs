using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OpaMenu.Domain.Interfaces;
using OpaMenu.Infrastructure.Shared.Data.Context;
using OpaMenu.Infrastructure.Shared.Data.Context.Opamenu;
using OpaMenu.Infrastructure.Shared.Entities;

namespace OpaMenu.Infrastructure.Repositories;

public class LoyaltyProgramRepository(OpamenuDbContext context) : OpamenuRepository<LoyaltyProgramEntity>(context), ILoyaltyProgramRepository
{
    public async Task<LoyaltyProgramEntity?> GetByTenantIdAsync(Guid tenantId)
    {
        return await _dbSet
            .FirstOrDefaultAsync(p => p.TenantId == tenantId);
    }
}
