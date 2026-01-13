using AutoMapper;
using OpaMenu.Domain.DTOs;
using OpaMenu.Domain.DTOs.AddonGroup;
using OpaMenu.Infrastructure.Shared.Entities;

namespace OpaMenu.Application.Mappings;

/// <summary>
/// Profile de mapeamento do AutoMapper para entidades AddonGroup
/// </summary>
public class AddonGroupMappingProfile : Profile
{
    public AddonGroupMappingProfile()
    {
        // Mapeamento de AddonGroup para AddonGroupResponseDto
        CreateMap<AddonGroupEntity, AddonGroupResponseDto>()
            .ForMember(dest => dest.Addons, opt => opt.MapFrom(src => src.Addons));

        // Mapeamento de CreateAddonGroupRequestDto para AddonGroup
        CreateMap<CreateAddonGroupRequestDto, AddonGroupEntity>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.TenantId, opt => opt.Ignore())
            .ForMember(dest => dest.Addons, opt => opt.Ignore())
            .ForMember(dest => dest.ProductAddonGroups, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow));

        // Mapeamento de UpdateAddonGroupRequestDto para AddonGroup
        CreateMap<UpdateAddonGroupRequestDto, AddonGroupEntity>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.TenantId, opt => opt.Ignore())
            .ForMember(dest => dest.Addons, opt => opt.Ignore())
            .ForMember(dest => dest.ProductAddonGroups, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow));
    }
}

