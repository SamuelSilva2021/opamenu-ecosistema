using OpaMenu.Application.DTOs;
using OpaMenu.Commons.Api.DTOs;
using OpaMenu.Domain.DTOs;
using OpaMenu.Domain.DTOs.Product;
using OpaMenu.Infrastructure.Shared.Entities;
using OpaMenu.Infrastructure.Shared.Entities.Opamenu;

namespace OpaMenu.Application.Services.Interfaces.Opamenu
{
    public interface IProductAditionalGroupService
    {
        // Obter grupos de adicionais de um produto
        Task<ResponseDTO<IEnumerable<ProductAditionalGroupResponseDto>>> GetProductAditionalGroupsAsync(Guid productId);
        
        // Obter produto com todos os grupos de adicionais
        Task<ResponseDTO<ProductWithAditionalsResponseDto?>> GetProductWithAditionalsAsync(Guid productId);
        
        // Adicionar grupo de adicionais a um produto
        Task<ResponseDTO<ProductAditionalGroupResponseDto>> AddAditionalGroupToProductAsync(Guid productId, AddProductAditionalGroupRequestDto request);
        
        // Atualizar configuração de grupo de adicionais em um produto
        Task<ResponseDTO<ProductAditionalGroupResponseDto>> UpdateProductAditionalGroupAsync(Guid productId, Guid aditionalGroupId, UpdateProductAditionalGroupRequestDto request);
        
        // Remover grupo de adicionais de um produto
        Task<ResponseDTO<object>> RemoveAditionalGroupFromProductAsync(Guid productId, Guid aditionalGroupId);
        
        // Reordenar grupos de adicionais de um produto
        //Task<ResponseDTO<object>> ReorderProductAditionalGroupsAsync(Guid productId, Dictionary<int, int> groupOrders);

        // Operações em lote
        Task<ResponseDTO<IEnumerable<ProductAditionalGroupResponseDto>>> BulkAddAditionalGroupsToProductAsync(Guid productId, IEnumerable<AddProductAditionalGroupRequestDto> requests);
        Task<ResponseDTO<bool>> BulkRemoveAditionalGroupsFromProductAsync(Guid productId, IEnumerable<Guid> aditionalGroupIds);
        
        // Validações
        Task<ResponseDTO<bool>> IsAditionalGroupAssignedToProductAsync(Guid productId, Guid aditionalGroupId);
        Task<ResponseDTO<bool>> CanRemoveAditionalGroupFromProductAsync(Guid productId, Guid aditionalGroupId);
        
        // Busca e filtros
        Task<ResponseDTO<IEnumerable<ProductDto>>> GetProductsWithAditionalGroupAsync(Guid aditionalGroupId);
        Task<ResponseDTO<IEnumerable<ProductWithAditionalsResponseDto>>> GetAllProductsWithAditionalsAsync();
    }
}
