using OpaMenu.Infrastructure.Shared.Entities;

namespace OpaMenu.Domain.Interfaces;

public interface IProductRepository : IRepository<ProductEntity>
{
    Task<IEnumerable<ProductEntity>> GetAllProductsAsync(Guid tenantId);
    Task<IEnumerable<ProductEntity>> GetActiveProductsAsync(Guid tenantId);
    Task<IEnumerable<ProductEntity>> GetProductsByCategoryAsync(int categoryId);
    Task<IEnumerable<ProductEntity>> GetActiveProductsByCategoryAsync(int categoryId);
    Task<int> GetNextDisplayOrderAsync(int categoryId);
    Task UpdateDisplayOrdersAsync(Dictionary<int, int> productDisplayOrders);
    Task<bool> IsNameUniqueInCategoryAsync(string name, int categoryId, int? excludeId = null);
    Task<bool> HasProductsInCategoryAsync(int categoryId);
    Task<IEnumerable<ProductEntity>> GetProductsByPriceRangeAsync(decimal minPrice, decimal maxPrice);
    Task<IEnumerable<ProductEntity>> SearchProductsAsync(string searchTerm, Guid tenantId);
    Task ReorderProductsAsync(Dictionary<int, int> productOrders);
    Task<IEnumerable<ProductEntity>> GetProductsForMenuAsync(Guid tenantId);
    Task<ProductEntity?> GetProductWithDetailsAsync(int id, Guid tenantId);
}

