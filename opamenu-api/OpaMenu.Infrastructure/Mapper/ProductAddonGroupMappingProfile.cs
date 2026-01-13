using AutoMapper;
using OpaMenu.Domain.DTOs;
using OpaMenu.Domain.DTOs.Product;
using OpaMenu.Infrastructure.Shared.Entities;

namespace OpaMenu.Application.Mappings;

/// <summary>
/// Profile de mapeamento do AutoMapper para entidades ProductAddonGroup
/// </summary>
public class ProductAddonGroupMappingProfile : Profile
{
    public ProductAddonGroupMappingProfile()
    {
        // Mapeamento de ProductAddonGroup para ProductAddonGroupResponseDto
        CreateMap<ProductAddonGroupEntity, ProductAddonGroupResponseDto>()
            .ForMember(dest => dest.AddonGroup, opt => opt.MapFrom(src => src.AddonGroup));

        // Mapeamento de AddProductAddonGroupRequestDto para ProductAddonGroup
        CreateMap<AddProductAddonGroupRequestDto, ProductAddonGroupEntity>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.ProductId, opt => opt.Ignore()) // ProductId deve ser definido manualmente
            .ForMember(dest => dest.Product, opt => opt.Ignore())
            .ForMember(dest => dest.AddonGroup, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow));

        // Mapeamento de UpdateProductAddonGroupRequestDto para ProductAddonGroup
        CreateMap<UpdateProductAddonGroupRequestDto, ProductAddonGroupEntity>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.ProductId, opt => opt.Ignore())
            .ForMember(dest => dest.AddonGroupId, opt => opt.Ignore())
            .ForMember(dest => dest.Product, opt => opt.Ignore())
            .ForMember(dest => dest.AddonGroup, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow));
    }
}

