

using OpaMenu.Application.Common.Models;
using OpaMenu.Application.Services.Interfaces;
using OpaMenu.Domain.DTOs;
using OpaMenu.Domain.Interfaces;

namespace OpaMenu.Application.Services;

/// <summary>
/// Implementação do serviço de validação de produtos usando recursos modernos do C# 13
/// </summary>
public class ProductValidationService(
    IProductRepository productRepository,
    ICategoryRepository categoryRepository,
    IOrderRepository orderRepository,
    ICurrentUserService currentUserService
    ) : IProductValidationService
{
    private readonly IProductRepository _productRepository = productRepository;
    private readonly ICategoryRepository _categoryRepository = categoryRepository;
    private readonly IOrderRepository _orderRepository = orderRepository;
    private readonly ICurrentUserService _currentUserService = currentUserService;

    /// <summary>
    /// Valida se uma categoria existe e está ativa
    /// </summary>
    public async Task<bool> IsCategoryValidAsync(Guid categoryId)
    {
        var category = await _categoryRepository.GetByIdAsync(categoryId, _currentUserService.GetTenantGuid()!.Value);
        return category != null && category.IsActive;
    }

    /// <summary>
    /// Valida se o nome do produto é único
    /// </summary>
    public async Task<bool> IsProductNameUniqueAsync(string name, Guid? excludeProductId = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            return false;

        var existingProducts = await _productRepository.GetAllAsync(_currentUserService.GetTenantGuid()!.Value);
        
        return !existingProducts.Any(p => 
            p.Name.Equals(name.Trim(), StringComparison.OrdinalIgnoreCase) && 
            p.Id != excludeProductId);
    }

    /// <summary>
    /// Valida se um produto pode ser excluído
    /// </summary>
    public async Task<bool> CanDeleteProductAsync(Guid productId)
    {
        // Verifica se o produto existe em pedidos ativos
        var activeOrders = await _orderRepository.GetActiveOrdersWithProductAsync(productId);
        return !activeOrders.Any();
    }

    /// <summary>
    /// Valida dados de criação de produto
    /// </summary>
    public async Task<IEnumerable<string>> ValidateCreateProductAsync(CreateProductRequestDto request)
    {
        var errors = new List<string>();

        // Validação de nome único
        if (!await IsProductNameUniqueAsync(request.Name))
            errors.Add("Já existe um produto com este nome");

        // Validações de negócio adicionais
        if (request.Price <= 0)
            errors.Add("O preço deve ser maior que zero");

        if (request.DisplayOrder < 0)
            errors.Add("A ordem de exibição não pode ser negativa");

        return errors;
    }

    /// <summary>
    /// Valida dados de atualização de produto
    /// </summary>
    public async Task<IEnumerable<string>> ValidateUpdateProductAsync(Guid productId, UpdateProductRequest request)
    {
        var errors = new List<string>();

        // Verifica se o produto existe
        var existingProduct = await _productRepository.GetByIdAsync(productId, _currentUserService.GetTenantGuid()!.Value);
        if (existingProduct == null)
        {
            errors.Add("Produto não encontrado");
            return errors;
        }

        // Validação de categoria
        if (!await IsCategoryValidAsync(request.CategoryId))
        {
            errors.Add("Categoria não encontrada ou inativa");
        }

        // Validação de nome único
        if (!await IsProductNameUniqueAsync(request.Name, productId))
        {
            errors.Add("Já existe um produto com este nome");
        }

        // Validações de negócio adicionais
        if (request.Price <= 0)
        {
            errors.Add("O preço deve ser maior que zero");
        }

        if (request.DisplayOrder < 0)
        {
            errors.Add("A ordem de exibição não pode ser negativa");
        }

        return errors;
    }
    
    /// <summary>
    /// Valida dados de criação de produto com resultado estruturado
    /// </summary>
    public async Task<ValidationResult> ValidateCreateProductRequestAsync(CreateProductRequestDto request)
    {
        var errors = await ValidateCreateProductAsync(request);
        var errorList = errors.ToList();
        
        return errorList.Count == 0 
            ? ValidationResult.Success() 
            : ValidationResult.Failure(errorList);
    }
    
    /// <summary>
    /// Valida dados de atualização de produto com resultado estruturado
    /// </summary>
    public async Task<ValidationResult> ValidateUpdateProductRequestAsync(Guid productId, UpdateProductRequest request)
    {
        var errors = await ValidateUpdateProductAsync(productId, request);
        var errorList = errors.ToList();
        
        return errorList.Count == 0 
            ? ValidationResult.Success() 
            : ValidationResult.Failure(errorList);
    }
}