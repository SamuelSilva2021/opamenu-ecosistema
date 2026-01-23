using Microsoft.EntityFrameworkCore;
using OpaMenu.Infrastructure.Shared.Entities;
using OpaMenu.Domain.Interfaces;
using OpaMenu.Infrastructure.Shared.Entities.MultiTenant.Tenant;
using OpaMenu.Infrastructure.Shared.Data.Context;
using OpaMenu.Infrastructure.Shared.Data.Context.MultTenant;

namespace OpaMenu.Infrastructure.Repositories;

public class TenantRepository(MultiTenantDbContext context) : MultiTenantRepository<TenantEntity>(context), ITenantRepository
{
    public async Task<TenantEntity?> GetBySlugAsync(string slug) => await _dbSet.FirstOrDefaultAsync(x => x.Slug == slug);

    public async Task<TenantEntity?> GetBySlugWithBusinessInfoAsync(string slug) => 
        await _dbSet.Include(x => x.BusinessInfo).Include(x => x.BankDetails).FirstOrDefaultAsync(x => x.Slug == slug);

    public async Task<Guid> GetTenantIdBySlugAsyn(string slug) => await _dbSet
            .Where(x => x.Slug == slug)
            .Select(x => x.Id)
            .FirstOrDefaultAsync();

    public async Task<TenantEntity?> GetByIdAsync(Guid id) => 
        await _dbSet.Include(x => x.BusinessInfo)
        .Include(x => x.BankDetails)
        .FirstOrDefaultAsync(x => x.Id == id);

    public async Task UpdateAsync(TenantEntity entity)
    {
        _dbSet.Update(entity);
        await _context.SaveChangesAsync();
    }
}

