using AutoMapper;
using OpaMenu.Domain.DTOs.Table;
using OpaMenu.Infrastructure.Shared.Entities;

namespace OpaMenu.Infrastructure.Mapper;

public class TableMappingProfile : Profile
{
    public TableMappingProfile()
    {
        CreateMap<TableEntity, TableResponseDto>();
        CreateMap<CreateTableRequestDto, TableEntity>()
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true));
        CreateMap<UpdateTableRequestDto, TableEntity>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
    }
}

