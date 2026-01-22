using OpaMenu.Application.DTOs;
using OpaMenu.Commons.Api.DTOs;
using OpaMenu.Domain.DTOs;
using OpaMenu.Domain.DTOs.Product;
using OpaMenu.Infrastructure.Shared.Entities;

namespace OpaMenu.Application.Services.Interfaces
{
    public interface IProductAddonGroupService
    {
        // Obter grupos de adicionais de um produto
        Task<ResponseDTO<IEnumerable<ProductAddonGroupResponseDto>>> GetProductAddonGroupsAsync(Guid productId);
        
        // Obter produto com todos os grupos de adicionais
        Task<ResponseDTO<ProductWithAddonsResponseDto?>> GetProductWithAddonsAsync(Guid productId);
        
        // Adicionar grupo de adicionais a um produto
        Task<ResponseDTO<ProductAddonGroupResponseDto>> AddAddonGroupToProductAsync(Guid productId, AddProductAddonGroupRequestDto request);
        
        // Atualizar configuraÃ§Ã£o de grupo de adicionais em um produto
        Task<ResponseDTO<ProductAddonGroupResponseDto>> UpdateProductAddonGroupAsync(Guid productId, Guid addonGroupId, UpdateProductAddonGroupRequestDto request);
        
        // Remover grupo de adicionais de um produto
        Task<ResponseDTO<object>> RemoveAddonGroupFromProductAsync(Guid productId, Guid addonGroupId);
        
        // Reordenar grupos de adicionais de um produto
        //Task<ResponseDTO<object>> ReorderProductAddonGroupsAsync(Guid productId, Dictionary<int, int> groupOrders);

        // OperaÃ§Ãµes em lote
        Task<ResponseDTO<IEnumerable<ProductAddonGroupResponseDto>>> BulkAddAddonGroupsToProductAsync(Guid productId, IEnumerable<AddProductAddonGroupRequestDto> requests);
        Task<ResponseDTO<bool>> BulkRemoveAddonGroupsFromProductAsync(Guid productId, IEnumerable<Guid> addonGroupIds);
        
        // ValidaÃ§Ãµes
        Task<ResponseDTO<bool>> IsAddonGroupAssignedToProductAsync(Guid productId, Guid addonGroupId);
        Task<ResponseDTO<bool>> CanRemoveAddonGroupFromProductAsync(Guid productId, Guid addonGroupId);
        
        // Busca e filtros
        Task<ResponseDTO<IEnumerable<ProductDto>>> GetProductsWithAddonGroupAsync(Guid addonGroupId);
        Task<ResponseDTO<IEnumerable<ProductWithAddonsResponseDto>>> GetAllProductsWithAddonsAsync();
    }
}

