using Microsoft.EntityFrameworkCore;
using OpaMenu.Infrastructure.Shared.Entities;
using OpaMenu.Domain.Interfaces;
using OpaMenu.Infrastructure.Shared.Data.Context;
using OpaMenu.Infrastructure.Shared.Data.Context.Opamenu;
using OpaMenu.Infrastructure.Shared.Entities.Opamenu;


namespace OpaMenu.Infrastructure.Repositories;

public class AddonRepository(OpamenuDbContext context) : OpamenuRepository<AddonEntity>(context), IAddonRepository
{
    public async Task<IEnumerable<AddonEntity>> GetAllAddonsAsync(Guid tenantId)
    {
        return await _dbSet
            .Where(a => a.TenantId == tenantId)
            .Include(a => a.AddonGroup)
            .OrderBy(a => a.AddonGroup.DisplayOrder)
            .ThenBy(a => a.DisplayOrder)
            .ThenBy(a => a.Name)
            .ToListAsync();
    }

    public async Task<IEnumerable<AddonEntity>> GetByAddonGroupIdAsync(Guid addonGroupId)
    {
        return await _dbSet
            .Where(a => a.AddonGroupId == addonGroupId)
            .Include(a => a.AddonGroup)
            .OrderBy(a => a.DisplayOrder)
            .ThenBy(a => a.Name)
            .ToListAsync();
    }

    public async Task<IEnumerable<AddonEntity>> GetActiveByAddonGroupIdAsync(Guid addonGroupId)
    {
        return await _dbSet
            .Where(a => a.AddonGroupId == addonGroupId && a.IsActive)
            .Include(a => a.AddonGroup)
            .OrderBy(a => a.DisplayOrder)
            .ThenBy(a => a.Name)
            .ToListAsync();
    }

    public async Task<bool> IsNameUniqueInGroupAsync(string name, Guid addonGroupId, Guid? excludeId = null)
    {
        var query = _dbSet.Where(a => a.Name.ToLower() == name.ToLower() && 
                                     a.AddonGroupId == addonGroupId);
        
        if (excludeId.HasValue)
            query = query.Where(a => a.Id != excludeId.Value);
            
        return !await query.AnyAsync();
    }

    public async Task<int> GetNextDisplayOrderAsync(Guid addonGroupId)
    {
        var maxOrder = await _dbSet
            .Where(a => a.AddonGroupId == addonGroupId)
            .MaxAsync(a => (int?)a.DisplayOrder);
            
        return (maxOrder ?? 0) + 1;
    }

    public async Task UpdateDisplayOrdersAsync(Dictionary<int, int> addonDisplayOrders)
    {
        foreach (var kvp in addonDisplayOrders)
        {
            var addon = await _dbSet.FindAsync(kvp.Key);
            if (addon != null)
            {
                addon.DisplayOrder = kvp.Value;
                addon.UpdatedAt = DateTime.UtcNow;
            }
        }
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<AddonEntity>> GetActiveAddonsAsync()
    {
        return await _dbSet
            .Where(a => a.IsActive)
            .Include(a => a.AddonGroup)
            .OrderBy(a => a.AddonGroup.DisplayOrder)
            .ThenBy(a => a.DisplayOrder)
            .ThenBy(a => a.Name)
            .ToListAsync();
    }

    public async Task<IEnumerable<AddonEntity>> GetByTenantIdAsync(Guid tenantId)
    {
        return await _dbSet
            .Where(a => a.TenantId == tenantId)
            .Include(a => a.AddonGroup)
            .OrderBy(a => a.AddonGroup.DisplayOrder)
            .ThenBy(a => a.DisplayOrder)
            .ThenBy(a => a.Name)
            .ToListAsync();
    }
}
