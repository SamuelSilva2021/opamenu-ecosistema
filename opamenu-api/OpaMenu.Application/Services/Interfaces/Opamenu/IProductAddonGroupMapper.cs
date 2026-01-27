using OpaMenu.Domain.DTOs;
using OpaMenu.Domain.DTOs.AddonGroup;
using OpaMenu.Domain.DTOs.Addons;
using OpaMenu.Domain.DTOs.Product;
using OpaMenu.Infrastructure.Shared.Entities.Opamenu;

namespace OpaMenu.Application.Services.Interfaces.Opamenu;

/// <summary>
/// Interface para mapeamento entre entidades ProductAddonGroup e DTOs
/// </summary>
public interface IProductAddonGroupMapper
{
    /// <summary>
    /// Mapeia uma entidade ProductAddonGroup para ProductAddonGroupResponseDto
    /// </summary>
    /// <param name="productAddonGroup">Entidade ProductAddonGroup</param>
    /// <returns>ProductAddonGroupResponseDto mapeado</returns>
    ProductAddonGroupResponseDto MapToDto(ProductAddonGroupEntity productAddonGroup);   
    IEnumerable<ProductAddonGroupResponseDto> MapToDtos(IEnumerable<ProductAddonGroupEntity> productAddonGroups);
    
    ProductAddonGroupEntity MapToEntity(AddProductAddonGroupRequestDto request, Guid productId);

    void MapToEntity(UpdateProductAddonGroupRequestDto request, ProductAddonGroupEntity productAddonGroup);
    
    /// <summary>
    /// Mapeia uma entidade Product com seus grupos de adicionais para ProductWithAddonsResponseDto
    /// </summary>
    /// <param name="product">Entidade Product com grupos de adicionais carregados</param>
    /// <returns>ProductWithAddonsResponseDto mapeado</returns>
    ProductWithAddonsResponseDto MapToProductWithAddonsDto(ProductEntity product);
    
    /// <summary>
    /// Mapeia uma entidade AddonGroup para AddonGroupResponseDto
    /// </summary>
    /// <param name="addonGroup">Entidade AddonGroup</param>
    /// <returns>AddonGroupResponseDto mapeado</returns>
    AddonGroupResponseDto MapToAddonGroupDto(AddonGroupEntity addonGroup);
    
    /// <summary>
    /// Mapeia uma entidade Addon para AddonResponseDto
    /// </summary>
    /// <param name="addon">Entidade Addon</param>
    /// <returns>AddonResponseDto mapeado</returns>
    AddonResponseDto MapToAddonDto(AddonEntity addon);
}
