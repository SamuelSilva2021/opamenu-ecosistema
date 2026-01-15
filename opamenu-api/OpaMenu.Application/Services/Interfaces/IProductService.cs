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
        Task<ResponseDTO<IEnumerable<ProductDto>>> GetProductsByCategoryAsync(int categoryId);
        Task<ResponseDTO<ProductDto?>> GetProductByIdAsync(int id);
        Task<ResponseDTO<ProductDto>> UpdateProductAsync(ProductEntity product);
        Task<ResponseDTO<bool>> DeleteProductAsync(int id);

        // Business Operations
        Task<ResponseDTO<IEnumerable<ProductDto>>> GetProductsForMenuAsync();
        Task<ResponseDTO<IEnumerable<ProductDto>>> ReorderProductsAsync(Dictionary<int, int> productOrders);
        Task<ResponseDTO<ProductDto>> ToggleProductStatusAsync(int id);
        Task<ResponseDTO<ProductDto>> UpdateProductPriceAsync(int id, decimal newPrice);

        // Search and Filter
        Task<ResponseDTO<IEnumerable<ProductDto>>> SearchProductsAsync(string searchTerm);
        Task<ResponseDTO<IEnumerable<ProductDto>>> GetProductsByPriceRangeAsync(decimal minPrice, decimal maxPrice);

        // DTO-based methods for better API integration
        Task<ResponseDTO<ProductDto>> CreateProductAsync(CreateProductRequestDto request);
        Task<ResponseDTO<ProductDto>> UpdateProductFromDtoAsync(int id, UpdateProductRequest request);

        // Story 2.3: Quick Operations
        Task UpdateProductAvailabilityAsync(int id, bool isActive);
        Task<BulkOperationResult> BulkUpdateProductsAsync(BulkUpdateRequest request);
        Task<IEnumerable<object>> GetProductPriceHistoryAsync(int productId);
        Task LogProductActivityAsync(int productId, string action, string? previousValue, string? newValue, string? userId = null);

        // Public Menu Operations (Slug-based)
        Task<ResponseDTO<IEnumerable<ProductDto>>> GetActiveProductsBySlugAsync(string slug);
        Task<ResponseDTO<MenuResponseDto>> GetProductsForMenuBySlugAsync(string slug);
        Task<ResponseDTO<IEnumerable<ProductDto>>> GetProductsByCategoryAndSlugAsync(int categoryId, string slug);
        Task<ResponseDTO<ProductDto?>> GetProductByIdAndSlugAsync(int id, string slug);
    }
}

