using OpaMenu.Application.DTOs;
using OpaMenu.Commons.Api.DTOs;
using OpaMenu.Domain.DTOs;
using OpaMenu.Domain.DTOs.Menu;
using OpaMenu.Domain.DTOs.Product;
using OpaMenu.Infrastructure.Shared.Entities;

namespace OpaMenu.Application.Services.Interfaces
{
    public interface IProductService
    {
        // CRUD Operations
        Task<ResponseDTO<IEnumerable<ProductDto>>> GetAllProductsAsync();
        Task<ResponseDTO<IEnumerable<ProductDto>>> GetProductsByCategoryAsync(Guid categoryId);
        Task<ResponseDTO<ProductDto?>> GetProductByIdAsync(Guid id);
        Task<ResponseDTO<ProductDto>> UpdateProductAsync(ProductEntity product);
        Task<ResponseDTO<bool>> DeleteProductAsync(Guid id);

        // Business Operations
        Task<ResponseDTO<IEnumerable<ProductDto>>> GetProductsForMenuAsync();
        Task<ResponseDTO<IEnumerable<ProductDto>>> ReorderProductsAsync(Dictionary<Guid, int> productOrders);
        Task<ResponseDTO<ProductDto>> ToggleProductStatusAsync(Guid id);
        Task<ResponseDTO<ProductDto>> UpdateProductPriceAsync(Guid id, decimal newPrice);

        // Search and Filter
        Task<ResponseDTO<IEnumerable<ProductDto>>> SearchProductsAsync(string searchTerm);
        Task<ResponseDTO<IEnumerable<ProductDto>>> GetProductsByPriceRangeAsync(decimal minPrice, decimal maxPrice);

        // DTO-based methods for better API integration
        Task<ResponseDTO<ProductDto>> CreateProductAsync(CreateProductRequestDto request);
        Task<ResponseDTO<ProductDto>> UpdateProductFromDtoAsync(Guid id, UpdateProductRequest request);

        // Story 2.3: Quick Operations
        Task UpdateProductAvailabilityAsync(Guid id, bool isActive);
        Task<BulkOperationResult> BulkUpdateProductsAsync(BulkUpdateRequest request);
        Task<IEnumerable<object>> GetProductPriceHistoryAsync(Guid productId);
        Task LogProductActivityAsync(Guid productId, string action, string? previousValue, string? newValue, Guid? userId = null);

        // Public Menu Operations (Slug-based)
        Task<ResponseDTO<IEnumerable<ProductDto>>> GetActiveProductsBySlugAsync(string slug);
        Task<ResponseDTO<MenuResponseDto>> GetProductsForMenuBySlugAsync(string slug);
        Task<ResponseDTO<IEnumerable<ProductDto>>> GetProductsByCategoryAndSlugAsync(Guid categoryId, string slug);
        Task<ResponseDTO<ProductDto?>> GetProductByIdAndSlugAsync(Guid id, string slug);
    }
}

