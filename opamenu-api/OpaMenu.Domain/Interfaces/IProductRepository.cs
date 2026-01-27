using OpaMenu.Infrastructure.Shared.Entities;
using OpaMenu.Infrastructure.Shared.Entities.Opamenu;

namespace OpaMenu.Domain.Interfaces;

public interface IProductRepository : IRepository<ProductEntity>
{
    Task<IEnumerable<ProductEntity>> GetAllProductsAsync(Guid tenantId);
    Task<IEnumerable<ProductEntity>> GetActiveProductsAsync(Guid tenantId);
    Task<IEnumerable<ProductEntity>> GetProductsByCategoryAsync(Guid categoryId);
    Task<IEnumerable<ProductEntity>> GetActiveProductsByCategoryAsync(Guid categoryId);
    Task<int> GetNextDisplayOrderAsync(Guid categoryId);
    Task UpdateDisplayOrdersAsync(Dictionary<Guid, int> productDisplayOrders);
    Task<bool> IsNameUniqueInCategoryAsync(string name, Guid categoryId, Guid? excludeId = null);
    Task<bool> HasProductsInCategoryAsync(Guid categoryId);
    Task<IEnumerable<ProductEntity>> GetProductsByPriceRangeAsync(decimal minPrice, decimal maxPrice);
    Task<IEnumerable<ProductEntity>> SearchProductsAsync(string searchTerm, Guid tenantId);
    Task ReorderProductsAsync(Dictionary<Guid, int> productOrders);
    Task<IEnumerable<ProductEntity>> GetProductsForMenuAsync(Guid tenantId);
    Task<ProductEntity?> GetProductWithDetailsAsync(Guid id, Guid tenantId);
}

