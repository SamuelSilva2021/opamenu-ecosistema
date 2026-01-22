using Microsoft.EntityFrameworkCore;
using OpaMenu.Domain.Interfaces;
using OpaMenu.Infrastructure.Shared.Entities.MultiTenant.Tenant;
using OpaMenu.Infrastructure.Shared.Data.Context;
using OpaMenu.Infrastructure.Shared.Data.Context.MultTenant;

namespace OpaMenu.Infrastructure.Repositories;

public class TenantBusinessRepository(MultiTenantDbContext context) : MultiTenantRepository<TenantBusinessEntity>(context), ITenantBusinessRepository
{
    public async Task<TenantBusinessEntity?> GetByTenantIdAsync(Guid tenantId)
    {
        return await _dbSet
            .Include (t => t.Tenant)
            .FirstOrDefaultAsync(x => x.TenantId == tenantId);
    }

    public async Task<TenantBusinessEntity> AddAsync(TenantBusinessEntity entity)
    {
        await _dbSet.AddAsync(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task UpdateAsync(TenantBusinessEntity entity)
    {
        _dbSet.Update(entity);
        await _context.SaveChangesAsync();
    }
}

