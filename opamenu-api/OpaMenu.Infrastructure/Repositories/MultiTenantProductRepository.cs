using Microsoft.EntityFrameworkCore;
using OpaMenu.Domain.Interfaces;
using OpaMenu.Infrastructure.Shared.Data.Context.MultTenant;
using OpaMenu.Infrastructure.Shared.Entities.MultiTenant.TenantProduct;

namespace OpaMenu.Infrastructure.Repositories;

public sealed class MultiTenantProductRepository(MultiTenantDbContext dbContext) : IMultiTenantProductRepository
{
    private readonly MultiTenantDbContext _dbContext = dbContext;

    public Task<TenantProductEntity?> GetFirstAsync()
    {
        return _dbContext.Set<TenantProductEntity>().AsNoTracking()
            .OrderBy(p => p.CreatedAt)
            .FirstOrDefaultAsync();
    }
}
