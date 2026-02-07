using OpaMenu.Infrastructure.Shared.Entities;
using OpaMenu.Infrastructure.Shared.Entities.Opamenu;

namespace OpaMenu.Domain.Interfaces;

public interface IProductAditionalGroupRepository
{
    Task<ProductAditionalGroupEntity?> GetByIdAsync(Guid id);
    Task<IEnumerable<ProductAditionalGroupEntity>> GetByProductIdAsync(Guid productId);
    Task<ProductAditionalGroupEntity?> GetByProductAndAditionalGroupAsync(Guid productId, Guid aditionalGroupId);
    Task<ProductAditionalGroupEntity> AddAsync(ProductAditionalGroupEntity productAditionalGroup);
    Task<ProductAditionalGroupEntity> UpdateAsync(ProductAditionalGroupEntity productAditionalGroup);
    Task DeleteAsync(ProductAditionalGroupEntity productAditionalGroup);
    Task<IEnumerable<ProductAditionalGroupEntity>> AddRangeAsync(IEnumerable<ProductAditionalGroupEntity> productAditionalGroups);
    Task DeleteRangeAsync(IEnumerable<ProductAditionalGroupEntity> productAditionalGroups);
    Task<bool> ExistsAsync(Guid productId, Guid aditionalGroupId);
    Task<IEnumerable<ProductEntity>> GetProductsByAditionalGroupAsync(Guid aditionalGroupId);
    Task SaveChangesAsync();
}
