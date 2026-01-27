using AutoMapper;
using OpaMenu.Domain.DTOs;
using OpaMenu.Domain.DTOs.Addons;
using OpaMenu.Infrastructure.Shared.Entities;
using OpaMenu.Infrastructure.Shared.Entities.Opamenu;

namespace OpaMenu.Infrastructure.Mapper;

/// <summary>
/// Profile de mapeamento do AutoMapper para entidades Addon
/// </summary>
public class AddonMappingProfile : Profile
{
    public AddonMappingProfile()
    {
        // Mapeamento de Addon para AddonResponseDto
        CreateMap<AddonEntity, AddonResponseDto>();

        // Mapeamento de CreateAddonRequestDto para Addon
        CreateMap<CreateAddonRequestDto, AddonEntity>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.TenantId, opt => opt.Ignore())
            .ForMember(dest => dest.AddonGroup, opt => opt.Ignore())
            .ForMember(dest => dest.OrderItemAddons, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow));

        // Mapeamento de UpdateAddonRequestDto para Addon
        CreateMap<UpdateAddonRequestDto, AddonEntity>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.TenantId, opt => opt.Ignore())
            .ForMember(dest => dest.AddonGroup, opt => opt.Ignore())
            .ForMember(dest => dest.OrderItemAddons, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow));
    }
}

