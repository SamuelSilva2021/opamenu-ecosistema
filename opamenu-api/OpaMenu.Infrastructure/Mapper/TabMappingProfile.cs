using AutoMapper;
using OpaMenu.Domain.DTOs.Tab;
using OpaMenu.Infrastructure.Shared.Entities.Opamenu;

namespace OpaMenu.Infrastructure.Mapper;

public class TabMappingProfile : Profile
{
    public TabMappingProfile()
    {
        CreateMap<TabEntity, TabResponseDto>()
            .ForMember(dest => dest.Orders, opt => opt.MapFrom(src => src.Orders));
    }
}
