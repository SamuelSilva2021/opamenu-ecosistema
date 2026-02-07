using Microsoft.EntityFrameworkCore;
using OpaMenu.Infrastructure.Shared.Entities;
using OpaMenu.Domain.Interfaces;
using OpaMenu.Infrastructure.Shared.Data.Context;
using OpaMenu.Infrastructure.Shared.Data.Context.Opamenu;
using OpaMenu.Infrastructure.Shared.Entities.Opamenu;

namespace OpaMenu.Infrastructure.Repositories;

public class AditionalGroupRepository(OpamenuDbContext context) : OpamenuRepository<AditionalGroupEntity>(context), IAditionalGroupRepository
{
    public async Task<IEnumerable<AditionalGroupEntity>> GetActiveAditionalGroupsAsync()
    {
        return await _dbSet
            .Where(ag => ag.IsActive)
            .Include(ag => ag.Aditionals.Where(a => a.IsActive))
            .OrderBy(ag => ag.DisplayOrder)
            .ThenBy(ag => ag.Name)
            .ToListAsync();
    }

    public async Task<IEnumerable<AditionalGroupEntity>> GetByTenantIdAsync(Guid tenantId)
    {
        return await _dbSet
            .Where(ag => ag.TenantId == tenantId)
            .Include(ag => ag.Aditionals)
            .OrderBy(ag => ag.DisplayOrder)
            .ThenBy(ag => ag.Name)
            .ToListAsync();
    }

    public async Task<IEnumerable<AditionalGroupEntity>> GetActiveByTenantIdAsync(Guid tenantId)
    {
        return await _dbSet
            .Where(ag => ag.IsActive && ag.TenantId == tenantId)
            .Include(ag => ag.Aditionals.Where(a => a.IsActive))
            .OrderBy(ag => ag.DisplayOrder)
            .ThenBy(ag => ag.Name)
            .ToListAsync();
    }

    public async Task<AditionalGroupEntity?> GetWithAditionalsAsync(Guid id)
    {
        return await _dbSet
            .Where(ag => ag.Id == id)
            .Include(ag => ag.Aditionals.Where(a => a.IsActive))
            .FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<AditionalGroupEntity>> GetByProductIdAsync(Guid productId)
    {
        var context = (OpamenuDbContext)_context;
        return await context.ProductAditionalGroups
            .Where(pag => pag.ProductId == productId)
            .Include(pag => pag.AditionalGroup)
                .ThenInclude(ag => ag.Aditionals)
            .Where(pag => pag.AditionalGroup.IsActive)
            .OrderBy(pag => pag.DisplayOrder)
            .Select(pag => pag.AditionalGroup)
            .ToListAsync();
    }

    public async Task<bool> IsNameUniqueAsync(string name, Guid? excludeId = null, Guid? tenantId = null)
    {
        var query = _dbSet.Where(ag => ag.Name.ToLower() == name.ToLower() && ag.IsActive);
        
        if (tenantId.HasValue)
            query = query.Where(ag => ag.TenantId == tenantId);
        
        if (excludeId.HasValue)
            query = query.Where(ag => ag.Id != excludeId.Value);
            
        return !await query.AnyAsync();
    }

    public async Task<int> GetNextDisplayOrderAsync(Guid? tenantId = null)
    {
        var query = _dbSet.AsQueryable();
        
        if (tenantId.HasValue)
            query = query.Where(ag => ag.TenantId == tenantId);

        var maxOrder = await query
            .MaxAsync(ag => (int?)ag.DisplayOrder);
            
        return (maxOrder ?? 0) + 1;
    }

    public async Task UpdateDisplayOrdersAsync(Dictionary<int, int> groupDisplayOrders)
    {
        foreach (var kvp in groupDisplayOrders)
        {
            var group = await _dbSet.FindAsync(kvp.Key);
            if (group != null)
            {
                group.DisplayOrder = kvp.Value;
                group.UpdatedAt = DateTime.UtcNow;
            }
        }
        await _context.SaveChangesAsync();
    }

    public async Task<bool> HasAditionalsAsync(Guid aditionalGroupId)
    {
        var context = (OpamenuDbContext)_context;
        return await context.Aditionals
            .AnyAsync(a => a.AditionalGroupId == aditionalGroupId);
    }
}
