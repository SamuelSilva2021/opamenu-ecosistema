using Microsoft.EntityFrameworkCore;
using OpaMenu.Infrastructure.Shared.Entities;
using OpaMenu.Domain.Interfaces;
using OpaMenu.Infrastructure.Shared.Data.Context;
using OpaMenu.Infrastructure.Shared.Data.Context.Opamenu;
using OpaMenu.Infrastructure.Shared.Entities.Opamenu;


namespace OpaMenu.Infrastructure.Repositories;

public class AditionalRepository(OpamenuDbContext context) : OpamenuRepository<AditionalEntity>(context), IAditionalRepository
{
    public async Task<IEnumerable<AditionalEntity>> GetAllAditionalsAsync(Guid tenantId)
    {
        return await _dbSet
            .Where(a => a.TenantId == tenantId)
            .Include(a => a.AditionalGroup)
            .OrderBy(a => a.AditionalGroup.DisplayOrder)
            .ThenBy(a => a.DisplayOrder)
            .ThenBy(a => a.Name)
            .ToListAsync();
    }

    public async Task<IEnumerable<AditionalEntity>> GetByAditionalGroupIdAsync(Guid aditionalGroupId)
    {
        return await _dbSet
            .Where(a => a.AditionalGroupId == aditionalGroupId)
            .Include(a => a.AditionalGroup)
            .OrderBy(a => a.DisplayOrder)
            .ThenBy(a => a.Name)
            .ToListAsync();
    }

    public async Task<IEnumerable<AditionalEntity>> GetActiveByAditionalGroupIdAsync(Guid aditionalGroupId)
    {
        return await _dbSet
            .Where(a => a.AditionalGroupId == aditionalGroupId && a.IsActive)
            .Include(a => a.AditionalGroup)
            .OrderBy(a => a.DisplayOrder)
            .ThenBy(a => a.Name)
            .ToListAsync();
    }

    public async Task<bool> IsNameUniqueInGroupAsync(string name, Guid aditionalGroupId, Guid? excludeId = null)
    {
        var query = _dbSet.Where(a => a.Name.ToLower() == name.ToLower() && 
                                     a.AditionalGroupId == aditionalGroupId);
        
        if (excludeId.HasValue)
            query = query.Where(a => a.Id != excludeId.Value);
            
        return !await query.AnyAsync();
    }

    public async Task<int> GetNextDisplayOrderAsync(Guid aditionalGroupId)
    {
        var maxOrder = await _dbSet
            .Where(a => a.AditionalGroupId == aditionalGroupId)
            .MaxAsync(a => (int?)a.DisplayOrder);
            
        return (maxOrder ?? 0) + 1;
    }

    public async Task UpdateDisplayOrdersAsync(Dictionary<int, int> aditionalDisplayOrders)
    {
        foreach (var kvp in aditionalDisplayOrders)
        {
            var aditional = await _dbSet.FindAsync(kvp.Key);
            if (aditional != null)
            {
                aditional.DisplayOrder = kvp.Value;
                aditional.UpdatedAt = DateTime.UtcNow;
            }
        }
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<AditionalEntity>> GetActiveAditionalsAsync()
    {
        return await _dbSet
            .Where(a => a.IsActive)
            .Include(a => a.AditionalGroup)
            .OrderBy(a => a.AditionalGroup.DisplayOrder)
            .ThenBy(a => a.DisplayOrder)
            .ThenBy(a => a.Name)
            .ToListAsync();
    }

    public async Task<IEnumerable<AditionalEntity>> GetByTenantIdAsync(Guid tenantId)
    {
        return await _dbSet
            .Where(a => a.TenantId == tenantId)
            .Include(a => a.AditionalGroup)
            .OrderBy(a => a.AditionalGroup.DisplayOrder)
            .ThenBy(a => a.DisplayOrder)
            .ThenBy(a => a.Name)
            .ToListAsync();
    }
}
