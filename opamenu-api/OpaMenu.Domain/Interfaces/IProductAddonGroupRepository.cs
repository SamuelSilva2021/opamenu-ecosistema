using OpaMenu.Infrastructure.Shared.Entities;

namespace OpaMenu.Domain.Interfaces;

public interface IProductAddonGroupRepository
{
    Task<ProductAddonGroupEntity?> GetByIdAsync(int id);
    Task<IEnumerable<ProductAddonGroupEntity>> GetByProductIdAsync(int productId);
    Task<ProductAddonGroupEntity?> GetByProductAndAddonGroupAsync(int productId, int addonGroupId);
    Task<ProductAddonGroupEntity> AddAsync(ProductAddonGroupEntity productAddonGroup);
    Task<ProductAddonGroupEntity> UpdateAsync(ProductAddonGroupEntity productAddonGroup);
    Task DeleteAsync(ProductAddonGroupEntity productAddonGroup);
    Task<IEnumerable<ProductAddonGroupEntity>> AddRangeAsync(IEnumerable<ProductAddonGroupEntity> productAddonGroups);
    Task DeleteRangeAsync(IEnumerable<ProductAddonGroupEntity> productAddonGroups);
    Task<bool> ExistsAsync(int productId, int addonGroupId);
    Task<IEnumerable<ProductEntity>> GetProductsByAddonGroupAsync(int addonGroupId);
    Task SaveChangesAsync();
}

