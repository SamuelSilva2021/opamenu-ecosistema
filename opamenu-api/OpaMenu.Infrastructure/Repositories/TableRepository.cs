using Microsoft.EntityFrameworkCore;
using OpaMenu.Infrastructure.Shared.Entities;
using OpaMenu.Domain.Interfaces;
using OpaMenu.Infrastructure.Shared.Data.Context;

namespace OpaMenu.Infrastructure.Repositories;

public class TableRepository : BaseRepository<TableEntity>, ITableRepository
{
    public TableRepository(OpamenuDbContext context) : base(context)
    {
    }

    public async Task<TableEntity?> GetByNameAsync(string name, Guid tenantId)
    {
        return await _dbSet.FirstOrDefaultAsync(t => t.Name == name && t.TenantId == tenantId);
    }

    public async Task<bool> ExistsByNameAsync(string name, Guid tenantId)
    {
        return await _dbSet.AnyAsync(t => t.Name == name && t.TenantId == tenantId);
    }

    public async Task<IEnumerable<TableEntity>> GetPagedByTenantIdAsync(Guid tenantId, int pageNumber, int pageSize)
    {
        return await _dbSet
            .Where(t => t.TenantId == tenantId)
            .OrderBy(t => t.Name)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<int> CountByTenantIdAsync(Guid tenantId)
    {
        return await _dbSet.CountAsync(t => t.TenantId == tenantId);
    }
}

