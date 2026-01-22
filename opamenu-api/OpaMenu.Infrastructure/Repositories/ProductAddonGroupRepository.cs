using Microsoft.EntityFrameworkCore;
using OpaMenu.Infrastructure.Shared.Entities;
using OpaMenu.Domain.Interfaces;
using OpaMenu.Infrastructure.Shared.Data.Context;

namespace OpaMenu.Infrastructure.Repositories;

public class ProductAddonGroupRepository(OpamenuDbContext context) : IProductAddonGroupRepository
{
    private readonly OpamenuDbContext _context = context;

    public async Task<ProductAddonGroupEntity?> GetByIdAsync(Guid id)
    {
        return await _context.ProductAddonGroups
            .Include(pag => pag.AddonGroup)
            .Include(pag => pag.Product)
            .FirstOrDefaultAsync(pag => pag.Id == id);
    }

    public async Task<IEnumerable<ProductAddonGroupEntity>> GetByProductIdAsync(Guid productId)
    {
        return await _context.ProductAddonGroups
            .Include(pag => pag.AddonGroup)
                .ThenInclude(ag => ag.Addons.Where(a => a.IsActive))
            .Where(pag => pag.ProductId == productId)
            .OrderBy(pag => pag.DisplayOrder)
            .ToListAsync();
    }

    public async Task<ProductAddonGroupEntity?> GetByProductAndAddonGroupAsync(Guid productId, Guid addonGroupId)
    {
        return await _context.ProductAddonGroups
            .Include(pag => pag.AddonGroup)
            .FirstOrDefaultAsync(pag => pag.ProductId == productId && pag.AddonGroupId == addonGroupId);
    }

    public async Task<ProductAddonGroupEntity> AddAsync(ProductAddonGroupEntity productAddonGroup)
    {
        _context.ProductAddonGroups.Add(productAddonGroup);
        await _context.SaveChangesAsync();

        return await _context.ProductAddonGroups
            .Include(pag => pag.AddonGroup)
            .FirstAsync(pag => pag.Id == productAddonGroup.Id);
    }

    public async Task<ProductAddonGroupEntity> UpdateAsync(ProductAddonGroupEntity productAddonGroup)
    {
        _context.ProductAddonGroups.Update(productAddonGroup);
        await _context.SaveChangesAsync();
        return productAddonGroup;
    }

    public async Task DeleteAsync(ProductAddonGroupEntity productAddonGroup)
    {
        _context.ProductAddonGroups.Remove(productAddonGroup);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<ProductAddonGroupEntity>> AddRangeAsync(IEnumerable<ProductAddonGroupEntity> productAddonGroups)
    {
        _context.ProductAddonGroups.AddRange(productAddonGroups);
        await _context.SaveChangesAsync();

        // Recarregar com navegaÃ§Ã£o
        var addedIds = productAddonGroups.Select(pag => pag.Id).ToList();
        return await _context.ProductAddonGroups
            .Include(pag => pag.AddonGroup)
            .Where(pag => addedIds.Contains(pag.Id))
            .ToListAsync();
    }

    public async Task DeleteRangeAsync(IEnumerable<ProductAddonGroupEntity> productAddonGroups)
    {
        _context.ProductAddonGroups.RemoveRange(productAddonGroups);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> ExistsAsync(Guid productId, Guid addonGroupId)
    {
        return await _context.ProductAddonGroups
            .AnyAsync(pag => pag.ProductId == productId && pag.AddonGroupId == addonGroupId);
    }

    public async Task<IEnumerable<ProductEntity>> GetProductsByAddonGroupAsync(Guid addonGroupId)
    {
        return await _context.ProductAddonGroups
            .Include(pag => pag.Product)
            .Where(pag => pag.AddonGroupId == addonGroupId)
            .Select(pag => pag.Product)
            .ToListAsync();
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}

