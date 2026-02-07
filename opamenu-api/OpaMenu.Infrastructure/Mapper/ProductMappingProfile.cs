using AutoMapper;
using OpaMenu.Domain.DTOs;
using OpaMenu.Domain.DTOs.Product;
using OpaMenu.Infrastructure.Shared.Entities;
using OpaMenu.Infrastructure.Shared.Entities.Opamenu;

namespace OpaMenu.Application.Mappings;

/// <summary>
/// Profile de mapeamento do AutoMapper para entidades Product
/// Utiliza recursos modernos do C# 13 e .NET 9
/// </summary>
public class ProductMappingProfile : Profile
{
    public ProductMappingProfile()
    {
        // Mapeamento de Product para ProductDto
        CreateMap<ProductEntity, ProductDto>()
            .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category != null ? src.Category.Name : string.Empty))
            //.ForMember(dest => dest.ImageUrl, opt => opt.Ignore()) // Removido para permitir o mapeamento direto da entidade
            .ForMember(dest => dest.AditionalGroups, opt => opt.MapFrom(src => src.AditionalGroups));

        // Mapeamento de ProductAditionalGroup para ProductAditionalGroupResponseDto
        CreateMap<ProductAditionalGroupEntity, ProductAditionalGroupResponseDto>()
            .ForMember(dest => dest.AditionalGroup, opt => opt.MapFrom(src => src.AditionalGroup));

        // Mapeamento de CreateProductRequestDto para Product
        CreateMap<CreateProductRequestDto, ProductEntity>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Category, opt => opt.Ignore()) // Ignorar propriedade de navegação
            .ForMember(dest => dest.Images, opt => opt.Ignore()) // Ignorar propriedade de navegação
            .ForMember(dest => dest.AditionalGroups, opt => opt.Ignore()) // Ignorar propriedade de navegação
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow));

        // Mapeamento de UpdateProductRequest para Product (para atualização)
        CreateMap<UpdateProductRequest, ProductEntity>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Category, opt => opt.Ignore()) // Ignorar propriedade de navegação
            .ForMember(dest => dest.Images, opt => opt.Ignore()) // Ignorar propriedade de navegação
            .ForMember(dest => dest.AditionalGroups, opt => opt.Ignore()) // Ignorar propriedade de navegação
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow));

        // Mapeamento reverso para casos especÃ­ficos
        CreateMap<ProductDto, ProductEntity>()
            .ForMember(dest => dest.Category, opt => opt.Ignore());
    }
}

