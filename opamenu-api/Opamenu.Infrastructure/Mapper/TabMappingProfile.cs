using AutoMapper;
using Opamenu.Domain.DTOs.Tab;
using Opamenu.Infrastructure.Shared.Entities.Opamenu;

namespace Opamenu.Infrastructure.Mapper;

public class TabMappingProfile : Profile
{
    public TabMappingProfile()
    {
        CreateMap<TabEntity, TabResponseDto>()
            .ForMember(dest => dest.Orders, opt => opt.MapFrom(src => src.Orders));
    }
}
