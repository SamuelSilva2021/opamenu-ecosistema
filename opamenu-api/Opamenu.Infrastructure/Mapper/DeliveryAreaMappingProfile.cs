using AutoMapper;
using Opamenu.Domain.DTOs.DeliveryArea;
using Opamenu.Infrastructure.Shared.Entities.Opamenu;

namespace Opamenu.Infrastructure.Mapper;

public class DeliveryAreaMappingProfile : Profile
{
    public DeliveryAreaMappingProfile()
    {
        CreateMap<DeliveryAreaEntity, DeliveryAreaResponseDto>();
        CreateMap<CreateDeliveryAreaRequestDto, DeliveryAreaEntity>();
    }
}
