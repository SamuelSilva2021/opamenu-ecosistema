using AutoMapper;
using Opamenu.Domain.DTOs.Table;
using Opamenu.Infrastructure.Shared.Entities;
using Opamenu.Infrastructure.Shared.Entities.Opamenu;

namespace Opamenu.Infrastructure.Mapper;

public class TableMappingProfile : Profile
{
    public TableMappingProfile()
    {
        CreateMap<TableEntity, TableResponseDto>();
        CreateMap<TableEntity, TableFullResponseDto>()
            .ForMember(dest => dest.Tabs, opt => opt.MapFrom(src => src.Tabs));
        CreateMap<CreateTableRequestDto, TableEntity>()
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true));
        CreateMap<UpdateTableRequestDto, TableEntity>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
    }
}

