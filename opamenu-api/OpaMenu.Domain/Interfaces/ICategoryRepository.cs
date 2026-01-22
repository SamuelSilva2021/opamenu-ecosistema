using OpaMenu.Infrastructure.Shared.Entities;

namespace OpaMenu.Domain.Interfaces
{
    public interface ICategoryRepository : IRepository<CategoryEntity>
    {
        Task<IEnumerable<CategoryEntity>> GetActiveCategoriesAsync();
        Task<IEnumerable<CategoryEntity>> GetActiveCategoriesAsync(Guid tenantId);
        Task<IEnumerable<CategoryEntity>> GetCategoriesOrderedAsync(Guid tenantId);
        Task<bool> HasProductsAsync(Guid categoryId);
        Task<int> GetNextDisplayOrderAsync();
        Task UpdateDisplayOrdersAsync(Dictionary<int, int> categoryDisplayOrders);
        Task<bool> IsNameUniqueAsync(Guid tenantId, string name, Guid? excludeId = null);
    }
}

