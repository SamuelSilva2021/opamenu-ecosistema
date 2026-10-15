using AutoMapper;
using Opamenu.Domain.DTOs;
using Opamenu.Domain.DTOs.Aditionals;
using Opamenu.Infrastructure.Shared.Entities;
using Opamenu.Infrastructure.Shared.Entities.Opamenu;

namespace Opamenu.Infrastructure.Mapper;

/// <summary>
/// Profile de mapeamento do AutoMapper para entidades Aditional
/// </summary>
public class AditionalMappingProfile : Profile
{
    public AditionalMappingProfile()
    {
        // Mapeamento de Aditional para AditionalResponseDto
        CreateMap<AditionalEntity, AditionalResponseDto>();

        // Mapeamento de CreateAditionalRequestDto para Aditional
        CreateMap<CreateAditionalRequestDto, AditionalEntity>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.TenantId, opt => opt.Ignore())
            .ForMember(dest => dest.AditionalGroup, opt => opt.Ignore())
            .ForMember(dest => dest.OrderItemAditionals, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow));

        // Mapeamento de UpdateAditionalRequestDto para Aditional
        CreateMap<UpdateAditionalRequestDto, AditionalEntity>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.TenantId, opt => opt.Ignore())
            .ForMember(dest => dest.AditionalGroup, opt => opt.Ignore())
            .ForMember(dest => dest.OrderItemAditionals, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow));
    }
}
