using OpaMenu.Application.Services.Interfaces.Opamenu;
using OpaMenu.Domain.DTOs;
using OpaMenu.Domain.DTOs.AditionalGroup;
using OpaMenu.Domain.DTOs.Aditionals;
using OpaMenu.Domain.DTOs.Product;
using OpaMenu.Infrastructure.Shared.Entities;
using OpaMenu.Infrastructure.Shared.Entities.Opamenu;

namespace OpaMenu.Application.Services.Opamenu;

/// <summary>
/// Implementação do mapeador de grupos de adicionais de produtos
/// </summary>
public class ProductAditionalGroupMapper(IUrlBuilderService urlBuilderService) : IProductAditionalGroupMapper
{
    private readonly IUrlBuilderService _urlBuilderService = urlBuilderService;

    /// <summary>
    /// Mapeia uma entidade ProductAditionalGroup para ProductAditionalGroupResponseDto
    /// </summary>
    public ProductAditionalGroupResponseDto MapToDto(ProductAditionalGroupEntity productAditionalGroup)
    {
        ArgumentNullException.ThrowIfNull(productAditionalGroup);
        
        return new ProductAditionalGroupResponseDto
        {
            Id = productAditionalGroup.Id,
            ProductId = productAditionalGroup.ProductId,
            AditionalGroupId = productAditionalGroup.AditionalGroupId,
            IsRequired = productAditionalGroup.IsRequired,
            MaxSelectionsOverride = productAditionalGroup.MaxSelectionsOverride,
            MinSelectionsOverride = productAditionalGroup.MinSelectionsOverride,
            DisplayOrder = productAditionalGroup.DisplayOrder,
            AditionalGroup = productAditionalGroup.AditionalGroup
            != null ? MapToAditionalGroupDto(productAditionalGroup.AditionalGroup) : null
        };
    }

    /// <summary>
    /// Mapeia uma coleção de entidades ProductAditionalGroup para ProductAditionalGroupResponseDto
    /// </summary>
    public IEnumerable<ProductAditionalGroupResponseDto> MapToDtos(IEnumerable<ProductAditionalGroupEntity> productAditionalGroups)
    {
        ArgumentNullException.ThrowIfNull(productAditionalGroups);
        
        return productAditionalGroups.Select(MapToDto);
    }

    /// <summary>
    /// Mapeia um AddProductAditionalGroupRequestDto para entidade ProductAditionalGroup
    /// </summary>
    public ProductAditionalGroupEntity MapToEntity(AddProductAditionalGroupRequestDto request, Guid productId)
    {
        ArgumentNullException.ThrowIfNull(request);
        
        return new ProductAditionalGroupEntity
        {
            ProductId = productId,
            AditionalGroupId = request.AditionalGroupId,
            IsRequired = request.IsRequired,
            MaxSelectionsOverride = request.MaxSelectionsOverride,
            MinSelectionsOverride = request.MinSelectionsOverride,
            DisplayOrder = request.DisplayOrder
        };
    }

    /// <summary>
    /// Atualiza uma entidade ProductAditionalGroup com dados do UpdateProductAditionalGroupRequestDto
    /// </summary>
    public void MapToEntity(UpdateProductAditionalGroupRequestDto request, ProductAditionalGroupEntity productAditionalGroup)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(productAditionalGroup);
        
        productAditionalGroup.IsRequired = request.IsRequired;
        productAditionalGroup.MaxSelectionsOverride = request.MaxSelectionsOverride;
        productAditionalGroup.MinSelectionsOverride = request.MinSelectionsOverride;
        productAditionalGroup.DisplayOrder = request.DisplayOrder;
    }

    /// <summary>
    /// Mapeia uma entidade Product com seus grupos de adicionais para ProductWithAditionalsResponseDto
    /// </summary>
    public ProductWithAditionalsResponseDto MapToProductWithAditionalsDto(ProductEntity product)
    {
        ArgumentNullException.ThrowIfNull(product);
        
        return new ProductWithAditionalsResponseDto
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            ImageUrl = product.ImageUrl,
            AditionalGroups = product.AditionalGroups?.Select(pag => new AditionalGroupResponseDto
            {
                Id = pag.AditionalGroup.Id,
                Name = pag.AditionalGroup?.Name ?? string.Empty,
                Description = pag.AditionalGroup?.Description,
                IsRequired = pag.IsRequired,
                MaxSelections = pag.MaxSelectionsOverride ?? pag.AditionalGroup?.MaxSelections ?? 0,
                MinSelections = pag.MinSelectionsOverride ?? pag.AditionalGroup?.MinSelections ?? 0,
                DisplayOrder = pag.DisplayOrder,
                Aditionals = pag.AditionalGroup?.Aditionals?.Select(MapToAditionalDto).ToList() ?? []
            }).ToList() ?? []
        };
    }

    /// <summary>
    /// Mapeia uma entidade AditionalGroup para AditionalGroupResponseDto
    /// </summary>
    public AditionalGroupResponseDto MapToAditionalGroupDto(AditionalGroupEntity aditionalGroup)
    {
        ArgumentNullException.ThrowIfNull(aditionalGroup);
        
        return new AditionalGroupResponseDto
        {
            Id = aditionalGroup.Id,
            Name = aditionalGroup.Name,
            Description = aditionalGroup.Description,
            IsRequired = false, // Será definido pelo ProductAditionalGroup
            MaxSelections = 0,  // Será definido pelo ProductAditionalGroup
            MinSelections = 0,  // Será definido pelo ProductAditionalGroup
            DisplayOrder = 0,   // Será definido pelo ProductAditionalGroup
            Aditionals = aditionalGroup.Aditionals?.Select(MapToAditionalDto).ToList() ?? []
        };
    }

    /// <summary>
    /// Mapeia uma entidade Aditional para AditionalResponseDto
    /// </summary>
    public AditionalResponseDto MapToAditionalDto(AditionalEntity aditional)
    {
        ArgumentNullException.ThrowIfNull(aditional);
        
        return new AditionalResponseDto
        {
            Id = aditional.Id,
            Name = aditional.Name,
            Description = aditional.Description,
            Price = aditional.Price,
            IsActive = aditional.IsActive,
            DisplayOrder = aditional.DisplayOrder,
            ImageUrl = _urlBuilderService.BuildImageUrl(aditional.ImageUrl)
        };
    }
}
