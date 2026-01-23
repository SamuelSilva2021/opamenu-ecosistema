using Microsoft.EntityFrameworkCore;
using OpaMenu.Domain.Interfaces;
using OpaMenu.Infrastructure.Shared.Data.Context.MultTenant;
using OpaMenu.Infrastructure.Shared.Entities.MultiTenant.Tenant;

namespace OpaMenu.Infrastructure.Repositories
{
    public class BankDetailsRepository(MultiTenantDbContext context) : MultiTenantRepository<BankDetailsEntity>(context), IBankDetailsRepository
    {
        public async Task<BankDetailsEntity?> GetByIdAsync(Guid tenantId, Guid id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<IEnumerable<BankDetailsEntity>> GetByTenantIdAsync(Guid tenantId)
        {
            return await _dbSet
                .Where(x => x.TenantId == tenantId)
                .ToListAsync();
        }

        public async Task AddAsync(BankDetailsEntity entity)
        {
            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(BankDetailsEntity entity)
        {
            _dbSet.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(BankDetailsEntity entity)
        {
            _dbSet.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<BankDetailsEntity?> GetByPixKeySelected(Guid tenantId, string pixKey) =>
            await _dbSet.FirstOrDefaultAsync(b => b.TenantId == tenantId && b.PixKey == pixKey && b.IsPixKeySelected);

        public async Task<BankDetailsEntity?> GetByPixKey(Guid tenantId, string pixKey) =>
             await _dbSet.FirstOrDefaultAsync(b => b.TenantId == tenantId && b.PixKey == pixKey);

        public async Task UpdateRangeAsync(IEnumerable<BankDetailsEntity> entities)
        {
            _dbSet.UpdateRange(entities);
            await _context.SaveChangesAsync();
        }
    }
}
