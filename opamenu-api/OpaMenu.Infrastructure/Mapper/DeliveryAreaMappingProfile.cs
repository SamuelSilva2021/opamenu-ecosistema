using AutoMapper;
using OpaMenu.Domain.DTOs.DeliveryArea;
using OpaMenu.Infrastructure.Shared.Entities.Opamenu;

namespace OpaMenu.Infrastructure.Mapper;

public class DeliveryAreaMappingProfile : Profile
{
    public DeliveryAreaMappingProfile()
    {
        CreateMap<DeliveryAreaEntity, DeliveryAreaResponseDto>();
        CreateMap<CreateDeliveryAreaRequestDto, DeliveryAreaEntity>();
    }
}
