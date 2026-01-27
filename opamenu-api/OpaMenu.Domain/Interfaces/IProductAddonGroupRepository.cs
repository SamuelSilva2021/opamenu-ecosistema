using OpaMenu.Infrastructure.Shared.Entities;
using OpaMenu.Infrastructure.Shared.Entities.Opamenu;

namespace OpaMenu.Domain.Interfaces;

public interface IProductAddonGroupRepository
{
    Task<ProductAddonGroupEntity?> GetByIdAsync(Guid id);
    Task<IEnumerable<ProductAddonGroupEntity>> GetByProductIdAsync(Guid productId);
    Task<ProductAddonGroupEntity?> GetByProductAndAddonGroupAsync(Guid productId, Guid addonGroupId);
    Task<ProductAddonGroupEntity> AddAsync(ProductAddonGroupEntity productAddonGroup);
    Task<ProductAddonGroupEntity> UpdateAsync(ProductAddonGroupEntity productAddonGroup);
    Task DeleteAsync(ProductAddonGroupEntity productAddonGroup);
    Task<IEnumerable<ProductAddonGroupEntity>> AddRangeAsync(IEnumerable<ProductAddonGroupEntity> productAddonGroups);
    Task DeleteRangeAsync(IEnumerable<ProductAddonGroupEntity> productAddonGroups);
    Task<bool> ExistsAsync(Guid productId, Guid addonGroupId);
    Task<IEnumerable<ProductEntity>> GetProductsByAddonGroupAsync(Guid addonGroupId);
    Task SaveChangesAsync();
}

