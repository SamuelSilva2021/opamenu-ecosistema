using AutoMapper;
using OpaMenu.Domain.DTOs;
using OpaMenu.Domain.DTOs.Product;
using OpaMenu.Infrastructure.Shared.Entities;
using OpaMenu.Infrastructure.Shared.Entities.Opamenu;

namespace OpaMenu.Application.Mappings;

/// <summary>
/// Profile de mapeamento do AutoMapper para entidades ProductAditionalGroup
/// </summary>
public class ProductAditionalGroupMappingProfile : Profile
{
    public ProductAditionalGroupMappingProfile()
    {
        // Mapeamento de ProductAditionalGroup para ProductAditionalGroupResponseDto
        CreateMap<ProductAditionalGroupEntity, ProductAditionalGroupResponseDto>()
            .ForMember(dest => dest.AditionalGroup, opt => opt.MapFrom(src => src.AditionalGroup));

        // Mapeamento de AddProductAditionalGroupRequestDto para ProductAditionalGroup
        CreateMap<AddProductAditionalGroupRequestDto, ProductAditionalGroupEntity>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.ProductId, opt => opt.Ignore()) // ProductId deve ser definido manualmente
            .ForMember(dest => dest.Product, opt => opt.Ignore())
            .ForMember(dest => dest.AditionalGroup, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow));

        // Mapeamento de UpdateProductAditionalGroupRequestDto para ProductAditionalGroup
        CreateMap<UpdateProductAditionalGroupRequestDto, ProductAditionalGroupEntity>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.ProductId, opt => opt.Ignore())
            .ForMember(dest => dest.AditionalGroupId, opt => opt.Ignore())
            .ForMember(dest => dest.Product, opt => opt.Ignore())
            .ForMember(dest => dest.AditionalGroup, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow));
    }
}
