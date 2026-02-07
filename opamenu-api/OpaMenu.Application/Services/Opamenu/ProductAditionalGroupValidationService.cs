using OpaMenu.Application.Common.Models;
using OpaMenu.Application.Services.Interfaces.Opamenu;
using OpaMenu.Domain.DTOs;
using OpaMenu.Domain.DTOs.Product;
using OpaMenu.Domain.Interfaces;

namespace OpaMenu.Application.Services.Opamenu;

/// <summary>
/// Implementação do serviço de validação de grupos de adicionais de produtos
/// </summary>
public class ProductAditionalGroupValidationService(
    IProductRepository productRepository,
    IAditionalGroupRepository aditionalGroupRepository,
    IProductAditionalGroupRepository productAditionalGroupRepository,
    IOrderRepository orderRepository,
    ICurrentUserService currentUserService
    ) : IProductAditionalGroupValidationService
{
    private readonly IProductRepository _productRepository = productRepository;
    private readonly IAditionalGroupRepository _aditionalGroupRepository = aditionalGroupRepository;
    private readonly IProductAditionalGroupRepository _productAditionalGroupRepository = productAditionalGroupRepository;
    private readonly IOrderRepository _orderRepository = orderRepository;
    private readonly ICurrentUserService _currentUserService = currentUserService;

    /// <summary>
    /// Valida se um produto existe e está ativo
    /// </summary>
    public async Task<bool> IsProductValidAsync(Guid productId)
    {
        var product = await _productRepository.GetByIdAsync(productId, _currentUserService.GetTenantGuid()!.Value);
        return product != null && product.IsActive;
    }

    /// <summary>
    /// Valida se um grupo de adicionais existe e está ativo
    /// </summary>
    public async Task<bool> IsAditionalGroupValidAsync(Guid aditionalGroupId)
    {
        var aditionalGroup = await _aditionalGroupRepository.GetByIdAsync(aditionalGroupId, _currentUserService.GetTenantGuid()!.Value);
        return aditionalGroup != null && aditionalGroup.IsActive; // Assuming AditionalGroup has IsActive or is implicitly active if exists
    }

    /// <summary>
    /// Valida se um grupo de adicionais já está associado a um produto
    /// </summary>
    public async Task<bool> IsAditionalGroupAlreadyAssignedAsync(Guid productId, Guid aditionalGroupId)
    {
        var productAditionalGroups = await _productAditionalGroupRepository.GetByProductIdAsync(productId);
        return productAditionalGroups.Any(pag => pag.AditionalGroupId == aditionalGroupId);
    }

    /// <summary>
    /// Valida se um ProductAditionalGroup pode ser excluído (não está em pedidos ativos)
    /// </summary>
    public async Task<bool> CanDeleteProductAditionalGroupAsync(Guid productAditionalGroupId)
    {
        var activeOrders = await _orderRepository.GetActiveOrdersWithProductAditionalGroupAsync(productAditionalGroupId);
        return !activeOrders.Any();
    }

    /// <summary>
    /// Valida dados de adição de grupo de adicionais a produto
    /// </summary>
    public async Task<ValidationResult> ValidateAddAditionalGroupToProductAsync(Guid productId, AddProductAditionalGroupRequestDto request)
    {
        var errors = new List<string>();

        // Validação de produto
        if (!await IsProductValidAsync(productId))
            errors.Add("Produto não encontrado ou inativo");

        // Validação de grupo de adicionais
        if (!await IsAditionalGroupValidAsync(request.AditionalGroupId))
            errors.Add("Grupo de adicionais não encontrado ou inativo");
        
        // Validação de duplicação
        if (await IsAditionalGroupAlreadyAssignedAsync(productId, request.AditionalGroupId))
            errors.Add("Este grupo de adicionais já está associado ao produto");
        
        // Validações de negócio
        if (!AreSelectionsValid(request.MinSelectionsOverride, request.MaxSelectionsOverride))
            errors.Add("As seleções mínimas não podem ser maiores que as máximas");

        if (!IsDisplayOrderValid(request.DisplayOrder))
            errors.Add("A ordem de exibição deve ser um valor positivo");

        return errors.Count == 0 
            ? ValidationResult.Success() 
            : ValidationResult.Failure("Erro na validação dos dados", errors);
    }

    /// <summary>
    /// Valida dados de atualização de grupo de adicionais de produto
    /// </summary>
    public async Task<ValidationResult> ValidateUpdateProductAditionalGroupAsync(Guid productAditionalGroupId, UpdateProductAditionalGroupRequestDto request)
    {
        var errors = new List<string>();

        // Verifica se o ProductAditionalGroup existe
        var existingProductAditionalGroup = await _productAditionalGroupRepository.GetByIdAsync(productAditionalGroupId);
        if (existingProductAditionalGroup == null)
        {
            errors.Add("Associação de grupo de adicionais não encontrada");
            return ValidationResult.Failure("Associação não encontrada", errors);
        }

        // Validações de negócio
        if (!AreSelectionsValid(request.MinSelectionsOverride, request.MaxSelectionsOverride))
            errors.Add("As seleções mínimas não podem ser maiores que as máximas");

        if (!IsDisplayOrderValid(request.DisplayOrder))
            errors.Add("A ordem de exibição deve ser um valor positivo");

        return errors.Count == 0 
            ? ValidationResult.Success() 
            : ValidationResult.Failure("Erro na validação dos dados", errors);
    }

    /// <summary>
    /// Valida dados de adição em lote de grupos de adicionais
    /// </summary>
    public async Task<ValidationResult> ValidateBulkAddAditionalGroupsAsync(Guid productId, IEnumerable<AddProductAditionalGroupRequestDto> requests)
    {
        var errors = new List<string>();
        var requestList = requests.ToList();

        // Validação de produto uma vez
        if (!await IsProductValidAsync(productId))
        {
            errors.Add("Produto não encontrado ou inativo");
            return ValidationResult.Failure("Produto inválido", errors);
        }

        // Validação de duplicação de grupos na requisição
        var duplicateGroups = requestList
            .GroupBy(r => r.AditionalGroupId)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .ToList();

        if (duplicateGroups.Any())
            errors.Add($"Grupos de adicionais duplicados na requisição: {string.Join(", ", duplicateGroups)}");

        // Validação individual de cada request
        for (int i = 0; i < requestList.Count; i++)
        {
            var request = requestList[i];
            var itemErrors = new List<string>();

            // Validação de grupo de adicionais
            if (!await IsAditionalGroupValidAsync(request.AditionalGroupId))
                itemErrors.Add($"Item {i + 1}: Grupo de adicionais {request.AditionalGroupId} não encontrado ou inativo");

            // Validação de duplicação com grupos já existentes
            if (await IsAditionalGroupAlreadyAssignedAsync(productId, request.AditionalGroupId))
                itemErrors.Add($"Item {i + 1}: Grupo de adicionais {request.AditionalGroupId} já está associado ao produto");

            // Validações de negócio
            if (!AreSelectionsValid(request.MinSelectionsOverride, request.MaxSelectionsOverride))
                itemErrors.Add($"Item {i + 1}: Seleções mínimas não podem ser maiores que as máximas");

            if (!IsDisplayOrderValid(request.DisplayOrder))
                itemErrors.Add($"Item {i + 1}: Ordem de exibição deve ser um valor positivo");

            errors.AddRange(itemErrors);
        }

        return errors.Count == 0 
            ? ValidationResult.Success() 
            : ValidationResult.Failure("Erro na validação dos dados em lote", errors);
    }

    /// <summary>
    /// Valida se as seleções mínimas e máximas são consistentes
    /// </summary>
    public bool AreSelectionsValid(int? minSelections, int? maxSelections)
    {
        // Se ambos são nulos, é válido
        if (!minSelections.HasValue && !maxSelections.HasValue)
            return true;

        // Se apenas um é nulo, é válido
        if (!minSelections.HasValue || !maxSelections.HasValue)
            return true;

        // Se ambos têm valor, min deve ser <= max
        return minSelections.Value <= maxSelections.Value && minSelections.Value >= 0 && maxSelections.Value >= 0;
    }

    /// <summary>
    /// Valida se a ordem de exibição é válida
    /// </summary>
    public bool IsDisplayOrderValid(int displayOrder) => displayOrder >= 0;
}
