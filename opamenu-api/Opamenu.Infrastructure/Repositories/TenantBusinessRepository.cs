using Microsoft.EntityFrameworkCore;
using Opamenu.Domain.Interfaces;
using Opamenu.Infrastructure.Shared.Entities.MultiTenant.Tenant;
using Opamenu.Infrastructure.Shared.Data.Context;
using Opamenu.Infrastructure.Shared.Data.Context.MultTenant;

namespace Opamenu.Infrastructure.Repositories;

public class TenantBusinessRepository(MultiTenantDbContext context) : MultiTenantRepository<TenantBusinessEntity>(context), ITenantBusinessRepository
{
    public async Task<TenantBusinessEntity?> GetByTenantIdAsync(Guid tenantId)
    {
        return await _dbSet
            .Include (t => t.Tenant)
            .ThenInclude(t => t.BankDetails)
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

