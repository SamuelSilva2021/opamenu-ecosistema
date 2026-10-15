using Microsoft.EntityFrameworkCore;
using Opamenu.Domain.Interfaces;
using Opamenu.Infrastructure.Shared.Data.Context.Opamenu;
using Opamenu.Infrastructure.Shared.Entities.Opamenu;
using Opamenu.Infrastructure.Shared.Enums.Opamenu;

namespace Opamenu.Infrastructure.Repositories;

public class TabRepository : BaseRepository<TabEntity>, ITabRepository
{
    public TabRepository(OpamenuDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<TabEntity>> GetByTableIdAsync(Guid tenantId, Guid tableId, ETabStatus? status = null)
    {
        var query = _dbSet.Where(t => t.TenantId == tenantId && t.TableId == tableId);
        if (status.HasValue)
            query = query.Where(t => t.Status == status.Value);

        return await query
            .OrderByDescending(t => t.OpenedAt)
            .ToListAsync();
    }

    public async Task<int> CountByTableIdAsync(Guid tenantId, Guid tableId)
    {
        return await _dbSet.CountAsync(t => t.TenantId == tenantId && t.TableId == tableId);
    }
    public async Task<TabEntity?> GetFullTabByIdAndTableIdAsync(Guid tenantId, Guid tableId, Guid tabId)
    {
        return await _dbSet
            .Where(t => t.TenantId == tenantId && t.TableId == tableId && t.Id == tabId)
            .Include(t => t.Orders)
                .ThenInclude(o => o.Items)
                .ThenInclude(i => i.Product)
            .FirstOrDefaultAsync();
    }
}

