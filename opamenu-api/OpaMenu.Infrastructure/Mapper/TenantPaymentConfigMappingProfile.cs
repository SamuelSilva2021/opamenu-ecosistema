using AutoMapper;
using OpaMenu.Domain.DTOs.TenantPaymentConfig;
using OpaMenu.Infrastructure.Shared.Entities.Opamenu;

namespace OpaMenu.Infrastructure.Mapper
{
    public class TenantPaymentConfigMappingProfile : Profile
    {
        public TenantPaymentConfigMappingProfile()
        {
            CreateMap<TenantPaymentConfigEntity, TenantPaymentConfigResponseDto>();
            CreateMap<UpsertTenantPaymentConfigRequestDto, TenantPaymentConfigEntity>();
        }
    }
}
