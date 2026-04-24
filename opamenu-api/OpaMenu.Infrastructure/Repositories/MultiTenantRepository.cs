using Microsoft.EntityFrameworkCore;
using OpaMenu.Infrastructure.Shared.Data.Context.MultTenant;

namespace OpaMenu.Infrastructure.Repositories;

/// <summary>
/// Repositório específico para MultiTenantDbContext
/// </summary>
public class MultiTenantRepository<T> where T : class
{
    protected readonly MultiTenantDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public MultiTenantRepository(MultiTenantDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }
}
