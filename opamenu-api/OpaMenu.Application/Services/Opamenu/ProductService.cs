using AutoMapper;
using OpaMenu.Application.Extensions;
using OpaMenu.Domain.DTOs;
using OpaMenu.Domain.DTOs.Category;
using OpaMenu.Domain.DTOs.Menu;
using OpaMenu.Domain.DTOs.Product;
using OpaMenu.Domain.DTOs.Tenant;
using OpaMenu.Infrastructure.Shared.Entities;
using OpaMenu.Domain.Interfaces;
using OpaMenu.Commons.Api.DTOs;
using OpaMenu.Commons.Api.Commons;
using OpaMenu.Application.Services.Interfaces.Opamenu;
using OpaMenu.Infrastructure.Shared.Entities.Opamenu;

namespace OpaMenu.Application.Services.Opamenu;

/// <summary>
/// ServiÃ§o de produtos refatorado com AutoMapper e recursos modernos do C# 13
/// </summary>
public class ProductService(
    IProductRepository productRepository,
    ICategoryRepository categoryRepository,
    IProductAditionalGroupService productAditionalGroupService,
    IMapper mapper,
    ICurrentUserService currentUserService,
    ITenantRepository tenantRepository,
    ITenantBusinessRepository tenantBusinessRepository

    ) : IProductService
{
    private readonly IProductRepository _productRepository = productRepository;
    private readonly ICategoryRepository _categoryRepository = categoryRepository;
    private readonly IProductAditionalGroupService _productAditionalGroupService = productAditionalGroupService;
    private readonly IMapper _mapper = mapper;
    private readonly ICurrentUserService _currentUserService = currentUserService;
    private readonly ITenantRepository _tenantRepository = tenantRepository;
    private readonly ITenantBusinessRepository _tenantBusinessRepository = tenantBusinessRepository;


    public async Task<ResponseDTO<IEnumerable<ProductDto>>> GetAllProductsAsync()
    {
        try
        {
            var productsEntity = await _productRepository.GetAllProductsAsync(_currentUserService.GetTenantGuid()!.Value);
            var products = _mapper.Map<IEnumerable<ProductDto>>(productsEntity);
            return StaticResponseBuilder<IEnumerable<ProductDto>>.BuildOk(products);
        }
        catch (Exception ex)
        {
            return StaticResponseBuilder<IEnumerable<ProductDto>>.BuildErrorResponse(ex);
        }

    }

    public async Task<ResponseDTO<IEnumerable<ProductDto>>> GetProductsByCategoryAsync(Guid categoryId)
    {
        try
        {
            var productsEntity = await _productRepository.GetProductsByCategoryAsync(categoryId);
            var products = _mapper.Map<IEnumerable<ProductDto>>(productsEntity);
            return StaticResponseBuilder<IEnumerable<ProductDto>>.BuildOk(products);
        }
        catch (Exception ex)
        {
            return StaticResponseBuilder<IEnumerable<ProductDto>>.BuildErrorResponse(ex);
        }
    }

    public async Task<ResponseDTO<ProductDto?>> GetProductByIdAsync(Guid id)
    {
        try
        {
            var product = await _productRepository.GetByIdAsync(id, _currentUserService.GetTenantGuid()!.Value);
            var productDto = _mapper.Map<ProductDto?>(product);
            return StaticResponseBuilder<ProductDto?>.BuildOk(productDto);
        }
        catch (Exception ex)
        {
            return StaticResponseBuilder<ProductDto?>.BuildErrorResponse(ex);
        }
    }

    public async Task<ResponseDTO<ProductDto>> UpdateProductAsync(ProductEntity product)
    {
        try
        {
            var existingProduct = await _productRepository.GetByIdAsync(product.Id, _currentUserService.GetTenantGuid()!.Value) ?? throw new ArgumentException("Produto nÃ£o encontrado.");

            var category = await _categoryRepository.GetByIdAsync(product.CategoryId, _currentUserService.GetTenantGuid()!.Value) ?? throw new ArgumentException("Categoria nÃ£o encontrada.");

            if (string.IsNullOrWhiteSpace(product.Name))
                throw new ArgumentException("Nome do produto Ã© obrigatorio");

            if (!string.Equals(product.Name, existingProduct.Name, StringComparison.OrdinalIgnoreCase))
            {
                var existingProducts = await _productRepository.SearchProductsAsync(product.Name, _currentUserService.GetTenantGuid()!.Value);
                var duplicateProduct = existingProducts.FirstOrDefault(p =>
                    string.Equals(p.Name, product.Name, StringComparison.OrdinalIgnoreCase) && p.Id != product.Id);

                if (duplicateProduct != null)
                    throw new InvalidOperationException("JÃ¡ existe um produto com esse nome");
            }

            await _productRepository.UpdateAsync(product);
            var productDto = _mapper.Map<ProductDto>(product);
            return StaticResponseBuilder<ProductDto>.BuildOk(productDto);
        }
        catch (Exception ex)
        {
            return StaticResponseBuilder<ProductDto>.BuildErrorResponse(ex);
        }

    }

    public async Task<ResponseDTO<bool>> DeleteProductAsync(Guid id)
    {
        try
        {
            var product = await _productRepository.GetByIdAsync(id, _currentUserService.GetTenantGuid()!.Value) ?? throw new ArgumentException("Product not found");
            await _productRepository.DeleteAsync(product);
            return StaticResponseBuilder<bool>.BuildOk(true);
        }
        catch (Exception)
        {
            return StaticResponseBuilder<bool>.BuildOk(false);
        }

    }

    public async Task<ResponseDTO<IEnumerable<ProductDto>>> GetProductsForMenuAsync()
    {
        try
        {
            var products = await _productRepository.GetProductsForMenuAsync(_currentUserService.GetTenantGuid()!.Value);
            var productsDto = _mapper.Map<IEnumerable<ProductDto>>(products);
            return StaticResponseBuilder<IEnumerable<ProductDto>>.BuildOk(productsDto);
        }
        catch (Exception ex)
        {
            return StaticResponseBuilder<IEnumerable<ProductDto>>.BuildErrorResponse(ex);
        }
    }

    public async Task<ResponseDTO<IEnumerable<ProductDto>>> ReorderProductsAsync(Dictionary<Guid, int> productOrders)
    {
        try
        {
            foreach (var productId in productOrders.Keys)
            {
                if (!await _productRepository.ExistsAsync(productId))
                    throw new ArgumentException($"Produto com ID {productId} nÃ£o encontrado.");
            }
            await _productRepository.ReorderProductsAsync(productOrders);

            var products = await _productRepository.GetAllProductsAsync(_currentUserService.GetTenantGuid()!.Value);
            var productsDto = _mapper.Map<IEnumerable<ProductDto>>(products);
            return StaticResponseBuilder<IEnumerable<ProductDto>>.BuildOk(productsDto);
        }
        catch (Exception ex)
        {
            return StaticResponseBuilder<IEnumerable<ProductDto>>.BuildErrorResponse(ex);
        }

    }

    public async Task<ResponseDTO<ProductDto>> ToggleProductStatusAsync(Guid id)
    {
        try
        {
            var product = await _productRepository.GetByIdAsync(id, _currentUserService.GetTenantGuid()!.Value) ?? throw new ArgumentException("Produto nÃ£o encontrado");
            product.IsActive = !product.IsActive;
            await _productRepository.UpdateAsync(product);
            var productDto = _mapper.Map<ProductDto>(product);
            return StaticResponseBuilder<ProductDto>.BuildOk(productDto);
        }
        catch (Exception ex)
        {
            return StaticResponseBuilder<ProductDto>.BuildErrorResponse(ex);
        }

    }

    public async Task<ResponseDTO<ProductDto>> UpdateProductPriceAsync(Guid id, decimal newPrice)
    {
        try
        {
            var product = await _productRepository.GetByIdAsync(id, _currentUserService.GetTenantGuid()!.Value) ?? throw new ArgumentException("Product not found");

            if (newPrice <= 0)
                throw new ArgumentException("Price must be greater than zero");

            product.Price = newPrice;
            await _productRepository.UpdateAsync(product);
            var productDto = _mapper.Map<ProductDto>(product);
            return StaticResponseBuilder<ProductDto>.BuildOk(productDto);
        }
        catch (Exception ex)
        {
            return StaticResponseBuilder<ProductDto>.BuildErrorResponse(ex);
        }

    }

    public async Task<ResponseDTO<IEnumerable<ProductDto>>> SearchProductsAsync(string searchTerm)
    {
        try
        {
            var products = await _productRepository.SearchProductsAsync(searchTerm, _currentUserService.GetTenantGuid()!.Value);

            if (products == null)
                return StaticResponseBuilder<IEnumerable<ProductDto>>.BuildOk([]);

            var productsDto = _mapper.Map<IEnumerable<ProductDto>>(products);
            return StaticResponseBuilder<IEnumerable<ProductDto>>.BuildOk(productsDto);
        }
        catch (Exception ex)
        {
            return StaticResponseBuilder<IEnumerable<ProductDto>>.BuildErrorResponse(ex);
        }
    }

    public async Task<ResponseDTO<IEnumerable<ProductDto>>> GetProductsByPriceRangeAsync(decimal minPrice, decimal maxPrice)
    {
        try
        {
            if (minPrice < 0 || maxPrice < 0 || minPrice > maxPrice)
                return StaticResponseBuilder<IEnumerable<ProductDto>>.BuildErrorResponse(
                    new ArgumentException("Preço inválido!"));

            var products = await _productRepository.GetProductsByPriceRangeAsync(minPrice, maxPrice);
            if (products == null)
                return StaticResponseBuilder<IEnumerable<ProductDto>>.BuildOk([]);

            var productsDto = _mapper.Map<IEnumerable<ProductDto>>(products);
            return StaticResponseBuilder<IEnumerable<ProductDto>>.BuildOk(productsDto);
        }
        catch (Exception ex)
        {
            return StaticResponseBuilder<IEnumerable<ProductDto>>.BuildErrorResponse(ex);
        }
    }

    public async Task<ResponseDTO<bool>> CanDeleteCategoryAsync(Guid categoryId) {
        try
        {
            var result = !await _productRepository.HasProductsInCategoryAsync(categoryId);
            return StaticResponseBuilder<bool>.BuildOk(result);
        }
        catch (Exception ex)
        {
            return StaticResponseBuilder<bool>.BuildErrorResponse(ex);
        }
    }

    /// <summary>
    /// Cria um produto usando AutoMapper
    /// </summary>
    /// <param name="request">Dados de criaÃ§Ã£o do produto</param>
    /// <returns>Produto criado</returns>
    /// <exception cref="ArgumentNullException">Request Ã© nulo</exception>
    /// <exception cref="ArgumentException">Categoria nÃ£o encontrada ou inativa</exception>
    public async Task<ResponseDTO<ProductDto>> CreateProductAsync(CreateProductRequestDto request)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(request);

            var category = await _categoryRepository.GetByIdAsync(request.CategoryId, _currentUserService.GetTenantGuid()!.Value)
                ?? throw new ArgumentException($"Categoria com ID {request.CategoryId} não encontrada.");

            if (!category.IsActive)
                throw new ArgumentException("A categoria do produto está inativa.");

            var product = _mapper.Map<ProductEntity>(request);

            product.TenantId = _currentUserService.GetTenantGuid()!.Value;
            product.Category = category;

            await _productRepository.AddAsync(product);
            var productDto = _mapper.Map<ProductDto>(product);
            return StaticResponseBuilder<ProductDto>.BuildOk(productDto);
        }
        catch (Exception ex)
        {
            return StaticResponseBuilder<ProductDto>.BuildErrorResponse(ex);
        }

    }

    /// <summary>
    /// Atualiza um produto usando AutoMapper para mapeamento elegante
    /// </summary>
    /// <param name="id">ID do produto</param>
    /// <param name="request">Dados de atualizaÃ§Ã£o</param>
    /// <returns>Produto atualizado</returns>
    /// <exception cref="ArgumentException">Produto nÃ£o encontrado</exception>
    public async Task<ResponseDTO<ProductDto>> UpdateProductFromDtoAsync(Guid id, UpdateProductRequest request)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(request);

            var existingProduct = await _productRepository.GetByIdAsync(id, _currentUserService.GetTenantGuid()!.Value)
                ?? throw new ArgumentException($"Product with ID {id} not found");

            var existingCategory = await _categoryRepository.GetByIdAsync(request.CategoryId, _currentUserService.GetTenantGuid()!.Value) ??
                throw new ArgumentException($"Category with ID {request.CategoryId} is invalid or inactive");

            var updatedProduct = existingProduct
                .UpdateBasicInfo(request.Name, request.Description, request.Price)
                .UpdateImage(request.ImageUrl)
                .UpdateStatus(request.IsActive)
                .UpdateDisplayOrder(request.DisplayOrder);

            if (existingProduct.CategoryId != request.CategoryId)
                updatedProduct.Category = existingCategory;

            updatedProduct.IsValid();

            var result = await UpdateProductAsync(updatedProduct);

            if (request.AditionalGroups != null)
            {
                var currentGroups = await _productAditionalGroupService.GetProductAditionalGroupsAsync(id);
                var currentGroupIds = currentGroups.Data!.Select(g => g.AditionalGroupId).ToList();

                var groupsToRemove = currentGroupIds.Except(request.AditionalGroups.Select(g => g.AditionalGroupId));
                if (groupsToRemove.Any())
                {
                    await _productAditionalGroupService.BulkRemoveAditionalGroupsFromProductAsync(id, groupsToRemove);
                }

                var newGroups = request.AditionalGroups.Where(g => !currentGroupIds.Contains(g.AditionalGroupId));
                if (newGroups.Any())
                {
                    await _productAditionalGroupService.BulkAddAditionalGroupsToProductAsync(id, newGroups);
                }

                // Atualizar grupos existentes
                var groupsToUpdate = request.AditionalGroups.Where(g => currentGroupIds.Contains(g.AditionalGroupId));
                foreach (var groupUpdate in groupsToUpdate)
                {
                    var updateRequest = new UpdateProductAditionalGroupRequestDto
                    {
                        DisplayOrder = groupUpdate.DisplayOrder,
                        IsRequired = groupUpdate.IsRequired,
                        MinSelectionsOverride = groupUpdate.MinSelectionsOverride,
                        MaxSelectionsOverride = groupUpdate.MaxSelectionsOverride
                    };

                    await _productAditionalGroupService.UpdateProductAditionalGroupAsync(
                        id, groupUpdate.AditionalGroupId, updateRequest);
                }
            }
            var productDto = _mapper.Map<ProductDto>(result.Data);
            return StaticResponseBuilder<ProductDto>.BuildOk(productDto);
        }
        catch (Exception ex)
        {
            return StaticResponseBuilder<ProductDto>.BuildErrorResponse(ex);
        }

    }

    public async Task<IEnumerable<object>> GetProductPriceHistoryAsync(Guid productId)
    {
        await Task.CompletedTask;
        return [];
    }

    public async Task LogProductActivityAsync(Guid productId, string action, string? previousValue, string? newValue, Guid? userId = null)
    {
        await Task.CompletedTask;
    }

    public async Task<BulkOperationResult> BulkUpdateProductsAsync(BulkUpdateRequest request)
    {
        await Task.CompletedTask;
        return new BulkOperationResult { SuccessCount = 0, FailureCount = 0, Errors = new List<string>() };
    }

    public async Task UpdateProductAvailabilityAsync(Guid id, bool isActive)
    {
        var product = await _productRepository.GetByIdAsync(id, _currentUserService.GetTenantGuid()!.Value);
        if (product != null)
        {
            product.IsActive = isActive;
            await _productRepository.UpdateAsync(product);
        }
    }

    // Public Menu Operations (Slug-based)
    public async Task<ResponseDTO<IEnumerable<ProductDto>>> GetActiveProductsBySlugAsync(string slug)
    {
        try
        {
            var tenantId = await _tenantRepository.GetTenantIdBySlugAsyn(slug);
            var productsEntity = await _productRepository.GetProductsForMenuAsync(tenantId);
            
            if (!productsEntity.Any())
                return StaticResponseBuilder<IEnumerable<ProductDto>>.BuildOk([]);

            var products = _mapper.Map<IEnumerable<ProductDto>>(productsEntity);
            return StaticResponseBuilder<IEnumerable<ProductDto>>.BuildOk(products);
        }
        catch (Exception ex)
        {
            return StaticResponseBuilder<IEnumerable<ProductDto>>.BuildErrorResponse(ex);
        }
    }

    public async Task<ResponseDTO<MenuResponseDto>> GetProductsForMenuBySlugAsync(string slug)
    {
        try
        {
            var tenantId = await _tenantRepository.GetTenantIdBySlugAsyn(slug);
            TenantBusinessResponseDto? tenantBusinness = null;
            IEnumerable<ProductDto> products = [];
            IEnumerable<CategoryResponseDto>? categories = null;

            var tenantBusinnessEntity = await _tenantBusinessRepository.GetByTenantIdAsync(tenantId);
            if (tenantBusinnessEntity != null)
                tenantBusinness = _mapper.Map<TenantBusinessResponseDto>(tenantBusinnessEntity);

            var productsEntity = await _productRepository.GetProductsForMenuAsync(tenantId);
            if(productsEntity.Any())
                products = _mapper.Map<IEnumerable<ProductDto>>(productsEntity);

            var cateriesEntity = productsEntity.Select(p => p.CategoryId).Distinct();
            if(cateriesEntity.Any())
                categories = _mapper.Map<IEnumerable<CategoryResponseDto>>(cateriesEntity);


            var menu = new MenuResponseDto
            {
                TenantBusiness = tenantBusinness,
                Products = products,
                Categories = categories
            };

            return StaticResponseBuilder<MenuResponseDto>.BuildOk(menu);
        }
        catch (Exception ex)
        {
            return StaticResponseBuilder<MenuResponseDto>.BuildErrorResponse(ex);
        }
    }

    public async Task<ResponseDTO<IEnumerable<ProductDto>>> GetProductsByCategoryAndSlugAsync(Guid categoryId, string slug)
    {
        try
        {
            var tenantId = await _tenantRepository.GetTenantIdBySlugAsyn(slug);

            // Verificar se a categoria pertence ao tenant
            var category = await _categoryRepository.GetByIdAsync(categoryId, tenantId);
            if (category == null)
                throw new ArgumentException("Categoria não encontrada para esta loja.");

            var productsEntity = await _productRepository.GetProductsByCategoryAsync(categoryId);
            var products = _mapper.Map<IEnumerable<ProductDto>>(productsEntity);
            return StaticResponseBuilder<IEnumerable<ProductDto>>.BuildOk(products);
        }
        catch (Exception ex)
        {
            return StaticResponseBuilder<IEnumerable<ProductDto>>.BuildErrorResponse(ex);
        }
    }

    public async Task<ResponseDTO<ProductDto?>> GetProductByIdAndSlugAsync(Guid id, string slug)
    {
        try
        {
            var tenantId = await _tenantRepository.GetTenantIdBySlugAsyn(slug);
            var product = await _productRepository.GetProductWithDetailsAsync(id, tenantId);

            if (product == null)
                return StaticResponseBuilder<ProductDto?>.BuildError("Produto não encontrado");

            var productDto = _mapper.Map<ProductDto?>(product);
            return StaticResponseBuilder<ProductDto?>.BuildOk(productDto);
        }
        catch (Exception ex)
        {
            return StaticResponseBuilder<ProductDto?>.BuildErrorResponse(ex);
        }
    }
}

