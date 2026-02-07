using Microsoft.EntityFrameworkCore;
using OpaMenu.Infrastructure.Shared.Entities;
using OpaMenu.Domain.Interfaces;
using OpaMenu.Infrastructure.Authentication;
using OpaMenu.Infrastructure.Shared.Data.Context;
using OpaMenu.Infrastructure.Shared.Data.Context.Opamenu;
using OpaMenu.Infrastructure.Shared.Entities.Opamenu;


namespace OpaMenu.Infrastructure.Repositories;

public class ProductRepository(
    OpamenuDbContext context
    ) : OpamenuRepository<ProductEntity>(context), IProductRepository
{
    public async Task<IEnumerable<ProductEntity>> GetAllProductsAsync(Guid tenantId) =>
        await _dbSet
            .Where(p => p.TenantId == tenantId)
            .Include(p => p.Category)
            .OrderBy(p => p.DisplayOrder)
            .ToListAsync();

    public override async Task<ProductEntity?> GetByIdAsync(Guid id, Guid tenantId) =>
        await _dbSet
            .Where(p => p.TenantId == tenantId)
            .Include(p => p.Category)
            .FirstOrDefaultAsync(x => x.Id == id);

    public async Task<IEnumerable<ProductEntity>> GetActiveProductsAsync(Guid tenantId)
    {
        return await _dbSet
            .Where(p => p.IsActive && p.TenantId == tenantId)
            .Include(p => p.Category)
            .OrderBy(p => p.DisplayOrder)
            .ToListAsync();
    }

    public async Task<IEnumerable<ProductEntity>> GetProductsByCategoryAsync(Guid categoryId)
    {
        return await _dbSet
            .Where(p => p.CategoryId == categoryId)
            .Include(p => p.Category)
            .OrderBy(p => p.DisplayOrder)
            .ToListAsync();
    }

    public async Task<IEnumerable<ProductEntity>> GetActiveProductsByCategoryAsync(Guid categoryId)
    {
        return await _dbSet
            .Where(p => p.CategoryId == categoryId && p.IsActive)
            .Include(p => p.Category)
            .OrderBy(p => p.DisplayOrder)
            .ToListAsync();
    }

    public async Task<int> GetNextDisplayOrderAsync(Guid categoryId)
    {
        var maxOrder = await _dbSet
            .Where(p => p.CategoryId == categoryId)
            .MaxAsync(p => (int?)p.DisplayOrder);
        
        return (maxOrder ?? 0) + 1;
    }

    public async Task UpdateDisplayOrdersAsync(Dictionary<Guid, int> productDisplayOrders)
    {
        foreach (var item in productDisplayOrders)
        {
            var product = await _dbSet.FindAsync(item.Key);
            if (product != null)
            {
                product.DisplayOrder = item.Value;
                product.UpdatedAt = DateTime.UtcNow;
            }
        }

        await _context.SaveChangesAsync();
    }

    public async Task<bool> IsNameUniqueInCategoryAsync(string name, Guid categoryId, Guid? excludeId = null)
    {
        var query = _dbSet.Where(p => 
            p.Name.ToLower() == name.ToLower() && 
            p.CategoryId == categoryId);
        
        if (excludeId.HasValue)
        {
            query = query.Where(p => p.Id != excludeId.Value);
        }

        return !await query.AnyAsync();
    }

    public async Task<bool> HasProductsInCategoryAsync(Guid categoryId)
    {
        return await _dbSet.AnyAsync(p => p.CategoryId == categoryId);
    }

    public async Task<IEnumerable<ProductEntity>> GetProductsByPriceRangeAsync(decimal minPrice, decimal maxPrice)
    {
        return await _dbSet
            .Where(p => p.Price >= minPrice && p.Price <= maxPrice)
            .Include(p => p.Category)
            .OrderBy(p => p.Price)
            .ToListAsync();
    }

    public async Task<IEnumerable<ProductEntity>> SearchProductsAsync(string searchTerm, Guid tenantId)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            return await GetActiveProductsAsync(tenantId);
        }

        return await _dbSet
            .Where(p => p.IsActive && 
                       (p.Name.Contains(searchTerm) || 
                        p.Description.Contains(searchTerm) ||
                        p.Category.Name.Contains(searchTerm)))
            .Include(p => p.Category)
            .OrderBy(p => p.Name)
            .ToListAsync();
    }

    public async Task ReorderProductsAsync(Dictionary<Guid, int> productOrders)
    {
        foreach (var productOrder in productOrders)
        {
            var product = await _dbSet.FindAsync(productOrder.Key);
            if (product != null)
            {
                product.DisplayOrder = productOrder.Value;
                product.UpdatedAt = DateTime.UtcNow;
            }
        }
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<ProductEntity>> GetProductsForMenuAsync(Guid tenantId)
    {
        return await _dbSet
            .Include(p => p.Category)
            .Include(p => p.AditionalGroups)
                .ThenInclude(pag => pag.AditionalGroup)
                    .ThenInclude(ag => ag.Aditionals)
            .Where(p => p.TenantId == tenantId && p.IsActive && p.Category.IsActive)
            .OrderBy(p => p.Category.DisplayOrder)
            .ThenBy(p => p.DisplayOrder)
            .ThenBy(p => p.Name)
            .ToListAsync();
    }

    public async Task<ProductEntity?> GetProductWithDetailsAsync(Guid id, Guid tenantId)
    {
        return await _dbSet
            .Include(p => p.Category)
            .Include(p => p.AditionalGroups)
                .ThenInclude(pag => pag.AditionalGroup)
                    .ThenInclude(ag => ag.Aditionals)
            .FirstOrDefaultAsync(p => p.Id == id && p.TenantId == tenantId);
    }
}

