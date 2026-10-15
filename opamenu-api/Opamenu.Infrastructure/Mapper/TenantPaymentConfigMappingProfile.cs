using AutoMapper;
using Opamenu.Domain.DTOs.TenantPaymentConfig;
using Opamenu.Infrastructure.Shared.Entities.Opamenu;

namespace Opamenu.Infrastructure.Mapper
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
