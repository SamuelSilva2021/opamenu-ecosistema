using Microsoft.EntityFrameworkCore;
using OpaMenu.Infrastructure.Shared.Entities;
using OpaMenu.Domain.Interfaces;
using OpaMenu.Infrastructure.Shared.Data.Context;
using OpaMenu.Infrastructure.Shared.Data.Context.Opamenu;
using OpaMenu.Infrastructure.Shared.Entities.Opamenu;

namespace OpaMenu.Infrastructure.Repositories;

public class ProductAditionalGroupRepository(OpamenuDbContext context) : IProductAditionalGroupRepository
{
    private readonly OpamenuDbContext _context = context;

    public async Task<ProductAditionalGroupEntity?> GetByIdAsync(Guid id)
    {
        return await _context.ProductAditionalGroups
            .Include(pag => pag.AditionalGroup)
            .Include(pag => pag.Product)
            .FirstOrDefaultAsync(pag => pag.Id == id);
    }

    public async Task<IEnumerable<ProductAditionalGroupEntity>> GetByProductIdAsync(Guid productId)
    {
        return await _context.ProductAditionalGroups
            .Include(pag => pag.AditionalGroup)
                .ThenInclude(ag => ag.Aditionals.Where(a => a.IsActive))
            .Where(pag => pag.ProductId == productId)
            .OrderBy(pag => pag.DisplayOrder)
            .ToListAsync();
    }

    public async Task<ProductAditionalGroupEntity?> GetByProductAndAditionalGroupAsync(Guid productId, Guid aditionalGroupId)
    {
        return await _context.ProductAditionalGroups
            .Include(pag => pag.AditionalGroup)
            .FirstOrDefaultAsync(pag => pag.ProductId == productId && pag.AditionalGroupId == aditionalGroupId);
    }

    public async Task<ProductAditionalGroupEntity> AddAsync(ProductAditionalGroupEntity productAditionalGroup)
    {
        _context.ProductAditionalGroups.Add(productAditionalGroup);
        await _context.SaveChangesAsync();

        return await _context.ProductAditionalGroups
            .Include(pag => pag.AditionalGroup)
            .FirstAsync(pag => pag.Id == productAditionalGroup.Id);
    }

    public async Task<ProductAditionalGroupEntity> UpdateAsync(ProductAditionalGroupEntity productAditionalGroup)
    {
        _context.ProductAditionalGroups.Update(productAditionalGroup);
        await _context.SaveChangesAsync();
        return productAditionalGroup;
    }

    public async Task DeleteAsync(ProductAditionalGroupEntity productAditionalGroup)
    {
        _context.ProductAditionalGroups.Remove(productAditionalGroup);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<ProductAditionalGroupEntity>> AddRangeAsync(IEnumerable<ProductAditionalGroupEntity> productAditionalGroups)
    {
        _context.ProductAditionalGroups.AddRange(productAditionalGroups);
        await _context.SaveChangesAsync();

        // Recarregar com navegação
        var addedIds = productAditionalGroups.Select(pag => pag.Id).ToList();
        return await _context.ProductAditionalGroups
            .Include(pag => pag.AditionalGroup)
            .Where(pag => addedIds.Contains(pag.Id))
            .ToListAsync();
    }

    public async Task DeleteRangeAsync(IEnumerable<ProductAditionalGroupEntity> productAditionalGroups)
    {
        _context.ProductAditionalGroups.RemoveRange(productAditionalGroups);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> ExistsAsync(Guid productId, Guid aditionalGroupId)
    {
        return await _context.ProductAditionalGroups
            .AnyAsync(pag => pag.ProductId == productId && pag.AditionalGroupId == aditionalGroupId);
    }

    public async Task<IEnumerable<ProductEntity>> GetProductsByAditionalGroupAsync(Guid aditionalGroupId)
    {
        return await _context.ProductAditionalGroups
            .Include(pag => pag.Product)
            .Where(pag => pag.AditionalGroupId == aditionalGroupId)
            .Select(pag => pag.Product)
            .ToListAsync();
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
