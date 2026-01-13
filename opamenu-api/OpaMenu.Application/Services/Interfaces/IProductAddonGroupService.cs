using OpaMenu.Application.DTOs;
using OpaMenu.Domain.DTOs;
using OpaMenu.Domain.DTOs.Product;
using OpaMenu.Infrastructure.Shared.Entities;

namespace OpaMenu.Application.Services.Interfaces
{
    public interface IProductAddonGroupService
    {
        // Obter grupos de adicionais de um produto
        Task<ResponseDTO<IEnumerable<ProductAddonGroupResponseDto>>> GetProductAddonGroupsAsync(int productId);
        
        // Obter produto com todos os grupos de adicionais
        Task<ResponseDTO<ProductWithAddonsResponseDto?>> GetProductWithAddonsAsync(int productId);
        
        // Adicionar grupo de adicionais a um produto
        Task<ResponseDTO<ProductAddonGroupResponseDto>> AddAddonGroupToProductAsync(int productId, AddProductAddonGroupRequestDto request);
        
        // Atualizar configuraÃ§Ã£o de grupo de adicionais em um produto
        Task<ResponseDTO<ProductAddonGroupResponseDto>> UpdateProductAddonGroupAsync(int productId, int addonGroupId, UpdateProductAddonGroupRequestDto request);
        
        // Remover grupo de adicionais de um produto
        Task<ResponseDTO<object>> RemoveAddonGroupFromProductAsync(int productId, int addonGroupId);
        
        // Reordenar grupos de adicionais de um produto
        Task<ResponseDTO<object>> ReorderProductAddonGroupsAsync(int productId, Dictionary<int, int> groupOrders);

        // OperaÃ§Ãµes em lote
        Task<ResponseDTO<IEnumerable<ProductAddonGroupResponseDto>>> BulkAddAddonGroupsToProductAsync(int productId, IEnumerable<AddProductAddonGroupRequestDto> requests);
        Task<ResponseDTO<bool>> BulkRemoveAddonGroupsFromProductAsync(int productId, IEnumerable<int> addonGroupIds);
        
        // ValidaÃ§Ãµes
        Task<ResponseDTO<bool>> IsAddonGroupAssignedToProductAsync(int productId, int addonGroupId);
        Task<ResponseDTO<bool>> CanRemoveAddonGroupFromProductAsync(int productId, int addonGroupId);
        
        // Busca e filtros
        Task<ResponseDTO<IEnumerable<ProductDto>>> GetProductsWithAddonGroupAsync(int addonGroupId);
        Task<ResponseDTO<IEnumerable<ProductWithAddonsResponseDto>>> GetAllProductsWithAddonsAsync();
    }
}

