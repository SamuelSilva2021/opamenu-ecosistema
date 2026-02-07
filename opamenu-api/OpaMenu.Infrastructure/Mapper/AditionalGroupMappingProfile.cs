using AutoMapper;
using OpaMenu.Domain.DTOs;
using OpaMenu.Domain.DTOs.AditionalGroup;
using OpaMenu.Infrastructure.Shared.Entities;
using OpaMenu.Infrastructure.Shared.Entities.Opamenu;

namespace OpaMenu.Application.Mappings;

/// <summary>
/// Profile de mapeamento do AutoMapper para entidades AditionalGroup
/// </summary>
public class AditionalGroupMappingProfile : Profile
{
    public AditionalGroupMappingProfile()
    {
        // Mapeamento de AditionalGroup para AditionalGroupResponseDto
        CreateMap<AditionalGroupEntity, AditionalGroupResponseDto>()
            .ForMember(dest => dest.Aditionals, opt => opt.MapFrom(src => src.Aditionals));

        // Mapeamento de CreateAditionalGroupRequestDto para AditionalGroup
        CreateMap<CreateAditionalGroupRequestDto, AditionalGroupEntity>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.TenantId, opt => opt.Ignore())
            .ForMember(dest => dest.Aditionals, opt => opt.Ignore())
            .ForMember(dest => dest.ProductAditionalGroups, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow));

        // Mapeamento de UpdateAditionalGroupRequestDto para AditionalGroup
        CreateMap<UpdateAditionalGroupRequestDto, AditionalGroupEntity>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.TenantId, opt => opt.Ignore())
            .ForMember(dest => dest.Aditionals, opt => opt.Ignore())
            .ForMember(dest => dest.ProductAditionalGroups, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow));
    }
}
