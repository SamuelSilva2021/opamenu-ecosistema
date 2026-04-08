using Microsoft.EntityFrameworkCore;
using OpaMenu.Infrastructure.Shared.Entities;
using OpaMenu.Domain.Interfaces;
using OpaMenu.Infrastructure.Shared.Data.Context;
using OpaMenu.Infrastructure.Shared.Data.Context.Opamenu;
using OpaMenu.Infrastructure.Shared.Entities.Opamenu;

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
    public async Task<IEnumerable<TableEntity>> GetPagedWithTabsAsync(Guid tenantId, int pageNumber, int pageSize) =>
        await _dbSet
            .Where(t => t.TenantId == tenantId)
            .Include(t => t.Tabs)
            .OrderBy(t => t.Name)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

    public async Task<TableEntity?> GetByIdWithDetailsAsync(Guid tenantId, Guid tableId)
    {
        return await _dbSet
            .Where(t => t.TenantId == tenantId && t.Id == tableId)
            .Include(t => t.Tabs)
                .ThenInclude(tab => tab.Orders)
                    .ThenInclude(o => o.Items)
                        .ThenInclude(i => i.Product)
            .Include(t => t.Tabs)
                .ThenInclude(tab => tab.Orders)
                    .ThenInclude(o => o.Items)
                        .ThenInclude(i => i.Aditionals)
            .FirstOrDefaultAsync();
    }

    public override async Task<int> CountByTenantIdAsync(Guid tenantId)
    {
        return await _dbSet.CountAsync(t => t.TenantId == tenantId);
    }
}
