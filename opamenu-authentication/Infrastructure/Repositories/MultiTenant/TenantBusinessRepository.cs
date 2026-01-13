using Authenticator.API.Core.Application.Interfaces;
using Authenticator.API.Core.Application.Interfaces.MultiTenant;
using OpaMenu.Infrastructure.Shared.Entities.MultiTenant.Tenant;
using Microsoft.EntityFrameworkCore;

namespace Authenticator.API.Infrastructure.Repositories.MultiTenant
{
    public class TenantBusinessRepository(IDbContextProvider dbContextProvider) : BaseRepository<TenantBusinessEntity>(dbContextProvider), ITenantBusinessRepository
    {
        private readonly DbSet<TenantBusinessEntity> _dbSet = dbContextProvider.GetDbSet<TenantBusinessEntity>();
        private readonly DbContext _context = dbContextProvider.GetContext<TenantBusinessEntity>();

        public async Task<TenantBusinessEntity> DeleteAsync(Guid tenantId)
        {
            var entity = await _dbSet.FirstOrDefaultAsync(x => x.TenantId == tenantId);
            if (entity != null)
            {
                _dbSet.Remove(entity);
                await _context.SaveChangesAsync();
                return entity;
            }
            
            throw new KeyNotFoundException($"TenantBusinessEntity with TenantId {tenantId} not found.");
        }

        public async Task<TenantBusinessEntity?> GetByTenantIdAsync(Guid tenantId)
        {
            return await _dbSet.FirstOrDefaultAsync(x => x.TenantId == tenantId);
        }
    }
}

