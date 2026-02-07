using OpaMenu.Application.Common.Models;
using OpaMenu.Domain.DTOs;
using OpaMenu.Domain.DTOs.Product;

namespace OpaMenu.Application.Services.Interfaces.Opamenu;

/// <summary>
/// Interface para validações de negócio de grupos de adicionais de produtos
/// </summary>
public interface IProductAditionalGroupValidationService
{
    /// <summary>
    /// Valida se um produto existe e está ativo
    /// </summary>
    /// <param name="productId">ID do produto</param>
    /// <returns>True se o produto é válido</returns>
    Task<bool> IsProductValidAsync(Guid productId);
    
    /// <summary>
    /// Valida se um grupo de adicionais existe e está ativo
    /// </summary>
    /// <param name="aditionalGroupId">ID do grupo de adicionais</param>
    /// <returns>True se o grupo é válido</returns>
    Task<bool> IsAditionalGroupValidAsync(Guid aditionalGroupId);
    
    /// <summary>
    /// Valida se um grupo de adicionais já está associado a um produto
    /// </summary>
    /// <param name="productId">ID do produto</param>
    /// <param name="aditionalGroupId">ID do grupo de adicionais</param>
    /// <returns>True se já existe a associação</returns>
    Task<bool> IsAditionalGroupAlreadyAssignedAsync(Guid productId, Guid aditionalGroupId);
    
    /// <summary>
    /// Valida se um ProductAditionalGroup pode ser excluído (não está em pedidos ativos)
    /// </summary>
    /// <param name="productAditionalGroupId">ID do ProductAditionalGroup</param>
    /// <returns>True se pode ser excluído</returns>
    Task<bool> CanDeleteProductAditionalGroupAsync(Guid productAditionalGroupId);
    
    /// <summary>
    /// Valida dados de adição de grupo de adicionais a produto
    /// </summary>
    /// <param name="productId">ID do produto</param>
    /// <param name="request">Request de adição</param>
    /// <returns>Resultado da validação</returns>
    Task<ValidationResult> ValidateAddAditionalGroupToProductAsync(Guid productId, AddProductAditionalGroupRequestDto request);
    
    /// <summary>
    /// Valida dados de atualização de grupo de adicionais de produto
    /// </summary>
    /// <param name="productAditionalGroupId">ID do ProductAditionalGroup</param>
    /// <param name="request">Request de atualização</param>
    /// <returns>Resultado da validação</returns>
    Task<ValidationResult> ValidateUpdateProductAditionalGroupAsync(Guid productAditionalGroupId, UpdateProductAditionalGroupRequestDto request);
    
    /// <summary>
    /// Valida dados de adição em lote de grupos de adicionais
    /// </summary>
    /// <param name="productId">ID do produto</param>
    /// <param name="requests">Lista de requests de adição</param>
    /// <returns>Resultado da validação</returns>
    Task<ValidationResult> ValidateBulkAddAditionalGroupsAsync(Guid productId, IEnumerable<AddProductAditionalGroupRequestDto> requests);
    
    /// <summary>
    /// Valida se as seleções mínimas e máximas são consistentes
    /// </summary>
    /// <param name="minSelections">Seleções mínimas</param>
    /// <param name="maxSelections">Seleções máximas</param>
    /// <returns>True se são consistentes</returns>
    bool AreSelectionsValid(int? minSelections, int? maxSelections);
    
    /// <summary>
    /// Valida se a ordem de exibição é válida
    /// </summary>
    /// <param name="displayOrder">Ordem de exibição</param>
    /// <returns>True se é válida</returns>
    bool IsDisplayOrderValid(int displayOrder);
}
