using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpaMenu.Application.Services;
using OpaMenu.Application.Services.Interfaces;
using OpaMenu.Domain.DTOs;
using OpaMenu.Domain.DTOs.Product;
using OpaMenu.Domain.Interfaces;
using OpaMenu.Web.Models.DTOs;
using OpaMenu.Web.UserEntry;
using OpaMenu.Infrastructure.Anotations;
using OpaMenu.Infrastructure.Filters;

namespace OpaMenu.Web.UserEntry.Products;

/// <summary>
/// Controller para gerenciamento de produtos seguindo princípios SOLID e Clean Architecture
/// </summary>
[ApiController]
[Route("api/products")]
[Authorize]
[ServiceFilter(typeof(PermissionAuthorizationFilter))]
public class ProductsController(
    IProductService productService,
    IProductValidationService validationService,
    ILogger<ProductsController> logger,
    IProductAddonGroupService productAddonGroupService
    ) : BaseController
{
    private readonly IProductService _productService = productService;
    private readonly IProductValidationService _validationService = validationService;
    private readonly ILogger<ProductsController> _logger = logger;
    private readonly IProductAddonGroupService _productAddonGroupService = productAddonGroupService;

    /// <summary>
    /// Get all products with optional filters
    /// </summary>
    [HttpGet]
    [MapPermission(MODULE_PRODUCT, OPERATION_SELECT)]
    public async Task<ActionResult<ApiResponse<IEnumerable<ProductDto>>>> GetProducts([FromQuery] ProductSearchRequest request)
    {
        var serviceResponse = request switch
        {
            { SearchTerm: not null } => await _productService.SearchProductsAsync(request.SearchTerm),
            { MinPrice: not null, MaxPrice: not null } => await _productService.GetProductsByPriceRangeAsync(request.MinPrice.Value, request.MaxPrice.Value),
            { CategoryId: not null } => await _productService.GetProductsByCategoryAsync(request.CategoryId.Value),
            _ => await _productService.GetAllProductsAsync()
        };
        return BuildResponse(serviceResponse);
    }

    /// <summary>
    /// Get products for public menu (only active products and categories)
    /// </summary>
    [HttpGet("menu")]
    [MapPermission(MODULE_PRODUCT, OPERATION_SELECT)]
    public async Task<ActionResult<ApiResponse<IEnumerable<ProductDto>>>> GetProductsForMenu()
    {
        var serviceResponse = await _productService.GetProductsForMenuAsync();
        return BuildResponse(serviceResponse);
    }

    /// <summary>
    /// Get products by category
    /// </summary>
    [HttpGet("by-category/{categoryId}")]
    [MapPermission(MODULE_PRODUCT, OPERATION_SELECT)]
    public async Task<ActionResult<ApiResponse<IEnumerable<ProductDto>>>> GetProductsByCategory(Guid categoryId)
    {
        var serviceResponse = await _productService.GetProductsByCategoryAsync(categoryId);
        return BuildResponse(serviceResponse);

    }

    /// <summary>
    /// Get specific product by ID
    /// </summary>
    [HttpGet("{id}")]
    [MapPermission(MODULE_PRODUCT, OPERATION_SELECT)]
    public async Task<ActionResult<ApiResponse<ProductDto>>> GetProduct(Guid id)
    {
        var serviceResponse = await _productService.GetProductByIdAsync(id);
        return BuildResponse(serviceResponse);
    }

    /// <summary>
    /// Create a new product
    /// </summary>
    [HttpPost]
    [MapPermission(MODULE_PRODUCT, OPERATION_INSERT)]
    public async Task<ActionResult<ApiResponse<ProductDto>>> CreateProduct([FromBody] CreateProductRequestDto request)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToArray();
            return BadRequest(ApiResponse<ProductDto>.ErrorResponse(errors));
        }

        // Validação de negócio
        var validationResult = await _validationService.ValidateCreateProductAsync(request);
        if (validationResult.Any())
            return BadRequest(ApiResponse<ProductDto>.ErrorResponse(validationResult.ToArray()));

        var serviceResponse = await _productService.CreateProductAsync(request);
        return BuildResponse(serviceResponse);
    }

    /// <summary>
    /// Update existing product
    /// </summary>
    [HttpPut("{id}")]
    [MapPermission(MODULE_PRODUCT, OPERATION_UPDATE)]
    public async Task<ActionResult<ApiResponse<ProductDto>>> UpdateProduct(Guid id, [FromBody] UpdateProductRequest request)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToArray();
            return BadRequest(ApiResponse<ProductDto>.ErrorResponse(errors));
        }

        var serviceResponse = await _productService.UpdateProductFromDtoAsync(id, request);
        return BuildResponse(serviceResponse);
    }

    /// <summary>
    /// Delete product
    /// </summary>
    [HttpDelete("{id}")]
    [MapPermission(MODULE_PRODUCT, OPERATION_DELETE)]
    public async Task<ActionResult<ApiResponse<object>>> DeleteProduct(Guid id)
    {
        var product = await _productService.GetProductByIdAsync(id);

        var canDelete = await _validationService.CanDeleteProductAsync(id);
        if (!canDelete)
            return BadRequest(ApiResponse<object>.ErrorResponse("Não é possível excluir este produto pois ele possui pedidos ativos"));

        var serviceResponse = await _productService.DeleteProductAsync(id);
        return BuildResponse(serviceResponse);
    }

    /// <summary>
    /// Toggle product active status
    /// </summary>
    [HttpPatch("{id}/toggle-status")]
    [MapPermission(MODULE_PRODUCT, OPERATION_UPDATE)]
    public async Task<ActionResult<ApiResponse<ProductDto>>> ToggleProductStatus(Guid id)
    {
        var serviceResponse = await _productService.ToggleProductStatusAsync(id);
        return BuildResponse(serviceResponse);

    }
    /// <summary>
    /// Obter todos os produtos com seus grupos de adicionais
    /// </summary>
    [HttpGet("with-addons")]
    [MapPermission(MODULE_PRODUCT, OPERATION_SELECT)]
    public async Task<ActionResult<IEnumerable<ProductWithAddonsResponseDto>>> GetAllProductsWithAddons()
    {
        var serviceResponse = await _productAddonGroupService.GetAllProductsWithAddonsAsync();
        return BuildResponse(serviceResponse);
    }

    /// <summary>
    /// Obter produtos que usam um grupo de adicionais específico
    /// </summary>
    [HttpGet("addon-groups/{addonGroupId:int}/products")]
    [MapPermission(MODULE_PRODUCT, OPERATION_SELECT)]
    public async Task<ActionResult<ApiResponse<IEnumerable<ProductDto>>>> GetProductsWithAddonGroup(Guid addonGroupId)
    {
        var products = await _productAddonGroupService.GetProductsWithAddonGroupAsync(addonGroupId);
        return BuildResponse(products);
    }
}
