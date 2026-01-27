using OpaMenu.Application.Services.Interfaces.Opamenu;
using OpaMenu.Domain.DTOs;
using OpaMenu.Domain.DTOs.AddonGroup;
using OpaMenu.Domain.DTOs.Addons;
using OpaMenu.Domain.DTOs.Product;
using OpaMenu.Infrastructure.Shared.Entities;
using OpaMenu.Infrastructure.Shared.Entities.Opamenu;

namespace OpaMenu.Application.Services.Opamenu;

/// <summary>
/// ImplementaÃ§Ã£o do mapeador de grupos de adicionais de produtos usando recursos modernos do C# 13
/// </summary>
public class ProductAddonGroupMapper(IUrlBuilderService urlBuilderService) : IProductAddonGroupMapper
{
    /// <summary>
    /// Mapeia uma entidade ProductAddonGroup para ProductAddonGroupResponseDto
    /// </summary>
    public ProductAddonGroupResponseDto MapToDto(ProductAddonGroupEntity productAddonGroup)
    {
        ArgumentNullException.ThrowIfNull(productAddonGroup);
        
        return new ProductAddonGroupResponseDto
        {
            Id = productAddonGroup.Id,
            ProductId = productAddonGroup.ProductId,
            AddonGroupId = productAddonGroup.AddonGroupId,
            IsRequired = productAddonGroup.IsRequired,
            MaxSelectionsOverride = productAddonGroup.MaxSelectionsOverride,
            MinSelectionsOverride = productAddonGroup.MinSelectionsOverride,
            DisplayOrder = productAddonGroup.DisplayOrder,
            AddonGroup = productAddonGroup.AddonGroup
            != null ? MapToAddonGroupDto(productAddonGroup.AddonGroup) : null
        };
    }

    /// <summary>
    /// Mapeia uma coleÃ§Ã£o de entidades ProductAddonGroup para ProductAddonGroupResponseDto usando collection expressions
    /// </summary>
    public IEnumerable<ProductAddonGroupResponseDto> MapToDtos(IEnumerable<ProductAddonGroupEntity> productAddonGroups)
    {
        ArgumentNullException.ThrowIfNull(productAddonGroups);
        
        return productAddonGroups.Select(MapToDto);
    }

    /// <summary>
    /// Mapeia um AddProductAddonGroupRequestDto para entidade ProductAddonGroup
    /// </summary>
    public ProductAddonGroupEntity MapToEntity(AddProductAddonGroupRequestDto request, Guid productId)
    {
        ArgumentNullException.ThrowIfNull(request);
        
        return new ProductAddonGroupEntity
        {
            ProductId = productId,
            AddonGroupId = request.AddonGroupId,
            IsRequired = request.IsRequired,
            MaxSelectionsOverride = request.MaxSelectionsOverride,
            MinSelectionsOverride = request.MinSelectionsOverride,
            DisplayOrder = request.DisplayOrder
        };
    }

    /// <summary>
    /// Atualiza uma entidade ProductAddonGroup com dados do UpdateProductAddonGroupRequestDto
    /// </summary>
    public void MapToEntity(UpdateProductAddonGroupRequestDto request, ProductAddonGroupEntity productAddonGroup)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(productAddonGroup);
        
        productAddonGroup.IsRequired = request.IsRequired;
        productAddonGroup.MaxSelectionsOverride = request.MaxSelectionsOverride;
        productAddonGroup.MinSelectionsOverride = request.MinSelectionsOverride;
        productAddonGroup.DisplayOrder = request.DisplayOrder;
    }

    /// <summary>
    /// Mapeia uma entidade Product com seus grupos de adicionais para ProductWithAddonsResponseDto
    /// </summary>
    public ProductWithAddonsResponseDto MapToProductWithAddonsDto(ProductEntity product)
    {
        ArgumentNullException.ThrowIfNull(product);
        
        return new ProductWithAddonsResponseDto
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            ImageUrl = product.ImageUrl,
            AddonGroups = product.AddonGroups?.Select(pag => new AddonGroupResponseDto
            {
                Id = pag.AddonGroup.Id,
                Name = pag.AddonGroup?.Name ?? string.Empty,
                Description = pag.AddonGroup?.Description,
                IsRequired = pag.IsRequired,
                MaxSelections = pag.MaxSelectionsOverride ?? pag.AddonGroup?.MaxSelections ?? 0,
                MinSelections = pag.MinSelectionsOverride ?? pag.AddonGroup?.MinSelections ?? 0,
                DisplayOrder = pag.DisplayOrder,
                Addons = pag.AddonGroup?.Addons?.Select(MapToAddonDto).ToList() ?? []
            }).ToList() ?? []
        };
    }

    /// <summary>
    /// Mapeia uma entidade AddonGroup para AddonGroupResponseDto
    /// </summary>
    public AddonGroupResponseDto MapToAddonGroupDto(AddonGroupEntity addonGroup)
    {
        ArgumentNullException.ThrowIfNull(addonGroup);
        
        return new AddonGroupResponseDto
        {
            Id = addonGroup.Id,
            Name = addonGroup.Name,
            Description = addonGroup.Description,
            IsRequired = false, // SerÃ¡ definido pelo ProductAddonGroup
            MaxSelections = 0,  // SerÃ¡ definido pelo ProductAddonGroup
            MinSelections = 0,  // SerÃ¡ definido pelo ProductAddonGroup
            DisplayOrder = 0,   // SerÃ¡ definido pelo ProductAddonGroup
            Addons = addonGroup.Addons?.Select(MapToAddonDto).ToList() ?? []
        };
    }

    /// <summary>
    /// Mapeia uma entidade Addon para AddonResponseDto
    /// </summary>
    public AddonResponseDto MapToAddonDto(AddonEntity addon)
    {
        ArgumentNullException.ThrowIfNull(addon);
        
        return new AddonResponseDto
        {
            Id = addon.Id,
            Name = addon.Name,
            Description = addon.Description,
            Price = addon.Price,
            IsActive = addon.IsActive,
            DisplayOrder = addon.DisplayOrder,
            ImageUrl = urlBuilderService.BuildImageUrl(addon.ImageUrl)
        };
    }
}
