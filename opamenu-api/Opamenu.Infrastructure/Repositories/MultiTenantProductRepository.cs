using Microsoft.EntityFrameworkCore;
using Opamenu.Domain.Interfaces;
using Opamenu.Infrastructure.Shared.Data.Context.MultTenant;
using Opamenu.Infrastructure.Shared.Entities.MultiTenant.TenantProduct;

namespace Opamenu.Infrastructure.Repositories;

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
