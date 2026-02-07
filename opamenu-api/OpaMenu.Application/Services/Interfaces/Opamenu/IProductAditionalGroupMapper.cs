using OpaMenu.Domain.DTOs;
using OpaMenu.Domain.DTOs.AditionalGroup;
using OpaMenu.Domain.DTOs.Aditionals;
using OpaMenu.Domain.DTOs.Product;
using OpaMenu.Infrastructure.Shared.Entities.Opamenu;

namespace OpaMenu.Application.Services.Interfaces.Opamenu;

/// <summary>
/// Interface para mapeamento entre entidades ProductAditionalGroup e DTOs
/// </summary>
public interface IProductAditionalGroupMapper
{
    /// <summary>
    /// Mapeia uma entidade ProductAditionalGroup para ProductAditionalGroupResponseDto
    /// </summary>
    /// <param name="productAditionalGroup">Entidade ProductAditionalGroup</param>
    /// <returns>ProductAditionalGroupResponseDto mapeado</returns>
    ProductAditionalGroupResponseDto MapToDto(ProductAditionalGroupEntity productAditionalGroup);   
    IEnumerable<ProductAditionalGroupResponseDto> MapToDtos(IEnumerable<ProductAditionalGroupEntity> productAditionalGroups);
    
    ProductAditionalGroupEntity MapToEntity(AddProductAditionalGroupRequestDto request, Guid productId);

    void MapToEntity(UpdateProductAditionalGroupRequestDto request, ProductAditionalGroupEntity productAditionalGroup);
    
    /// <summary>
    /// Mapeia uma entidade Product com seus grupos de adicionais para ProductWithAditionalsResponseDto
    /// </summary>
    /// <param name="product">Entidade Product com grupos de adicionais carregados</param>
    /// <returns>ProductWithAditionalsResponseDto mapeado</returns>
    ProductWithAditionalsResponseDto MapToProductWithAditionalsDto(ProductEntity product);
    
    /// <summary>
    /// Mapeia uma entidade AditionalGroup para AditionalGroupResponseDto
    /// </summary>
    /// <param name="aditionalGroup">Entidade AditionalGroup</param>
    /// <returns>AditionalGroupResponseDto mapeado</returns>
    AditionalGroupResponseDto MapToAditionalGroupDto(AditionalGroupEntity aditionalGroup);
    
    /// <summary>
    /// Mapeia uma entidade Aditional para AditionalResponseDto
    /// </summary>
    /// <param name="aditional">Entidade Aditional</param>
    /// <returns>AditionalResponseDto mapeado</returns>
    AditionalResponseDto MapToAditionalDto(AditionalEntity aditional);
}
