using Authenticator.API.Core.Application.Interfaces;
using Authenticator.API.Core.Application.Interfaces.MultiTenant;
using OpaMenu.Infrastructure.Shared.Entities.MultiTenant.TenantModule;
using Microsoft.EntityFrameworkCore;

namespace Authenticator.API.Infrastructure.Repositories.MultiTenant
{
    public class TenantModuleRepository(IDbContextProvider dbContextProvider) : BaseRepository<TenantModuleEntity>(dbContextProvider), ITenantModuleRepository
    {
        private readonly DbContext _context = dbContextProvider.GetContext<TenantModuleEntity>();
        private readonly DbSet<TenantModuleEntity> _dbSet = dbContextProvider.GetDbSet<TenantModuleEntity>();

        public async Task<IEnumerable<TenantModuleEntity>> GetByTenantIdAsync(Guid tenantId)
        {
            return await _dbSet.Where(tm => tm.TenantId == tenantId).ToListAsync();
        }

        public async Task RemoveByTenantIdAsync(Guid tenantId)
        {
             var modules = await _dbSet.Where(tm => tm.TenantId == tenantId).ToListAsync();
             if (modules.Any())
             {
                 _dbSet.RemoveRange(modules);
                 await _context.SaveChangesAsync();
             }
        }

        public async Task AddRangeAsync(IEnumerable<TenantModuleEntity> modules)
        {
            await _dbSet.AddRangeAsync(modules);
            await _context.SaveChangesAsync();
        }
    }
}
