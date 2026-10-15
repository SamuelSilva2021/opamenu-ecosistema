using Microsoft.EntityFrameworkCore;
using Opamenu.Infrastructure.Shared.Entities;
using Opamenu.Domain.Interfaces;
using Opamenu.Infrastructure.Shared.Data.Context;
using Opamenu.Infrastructure.Shared.Data.Context.Opamenu;
using Opamenu.Infrastructure.Repositories;
using Opamenu.Infrastructure.Shared.Entities.Opamenu;

namespace Opamenu.Infrastructure.Repositories;

public class CategoryRepository(OpamenuDbContext context) : OpamenuRepository<CategoryEntity>(context), ICategoryRepository
{
    public async Task<IEnumerable<CategoryEntity>> GetActiveCategoriesAsync()
    {
        return await _dbSet
            .Where(x => x.IsActive)
            .OrderBy(x => x.DisplayOrder)
            .ToListAsync();
    }

    public async Task<IEnumerable<CategoryEntity>> GetActiveCategoriesAsync(Guid tenantId)
    {
        return await _dbSet
            .Where(x => x.IsActive && x.TenantId == tenantId)
            .OrderBy(x => x.DisplayOrder)
            .ToListAsync();
    }

    public async Task<IEnumerable<CategoryEntity>> GetCategoriesOrderedAsync(Guid tenantId)
    {
        return await _dbSet
            .Where(x => x.TenantId == tenantId)
            .Include(x => x.Products)
            .OrderBy(x => x.DisplayOrder)
            .ToListAsync();
    }

    public async Task<bool> HasProductsAsync(Guid categoryId)
    {
        var context = (OpamenuDbContext)_context;
        return await context.Products
            .AnyAsync(x => x.CategoryId == categoryId && x.IsActive);
    }

    public async Task<int> GetNextDisplayOrderAsync()
    {
        var maxOrder = await _dbSet
            .MaxAsync(x => (int?)x.DisplayOrder);
        
        return (maxOrder ?? 0) + 1;
    }

    public async Task UpdateDisplayOrdersAsync(Dictionary<int, int> categoryDisplayOrders)
    {
        foreach (var kvp in categoryDisplayOrders)
        {
            var category = await _dbSet.FindAsync(kvp.Key);
            if (category != null)
            {
                category.DisplayOrder = kvp.Value;
                category.UpdatedAt = DateTime.UtcNow;
            }
        }

        await _context.SaveChangesAsync();
    }

    public async Task<bool> IsNameUniqueAsync(Guid tenantId, string name, Guid? excludeId = null)
    {
        var query = _dbSet.Where(x => x.Name.ToLower() == name.ToLower() && x.TenantId == tenantId);
        
        if (excludeId.HasValue)
        {
            query = query.Where(x => x.Id != excludeId.Value);
        }

        return await query.AnyAsync();
    }
}

