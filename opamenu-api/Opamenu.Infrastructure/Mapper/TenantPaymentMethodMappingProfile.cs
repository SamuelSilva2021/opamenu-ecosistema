using AutoMapper;
using Opamenu.Domain.DTOs.TenantPaymentMethod;
using Opamenu.Infrastructure.Shared.Entities;
using Opamenu.Infrastructure.Shared.Entities.Opamenu;

namespace Opamenu.Infrastructure.Mapper
{
    public class TenantPaymentMethodMappingProfile : Profile
    {
        public TenantPaymentMethodMappingProfile()
        {
            CreateMap<TenantPaymentMethodEntity, TenantPaymentMethodResponseDto>();

            CreateMap<CreateTenantPaymentMethodRequestDto, TenantPaymentMethodEntity>();

            CreateMap<UpdateTenantPaymentMethodRequestDto, TenantPaymentMethodEntity>()
                .ForMember(dest => dest.Alias, opt => opt.Condition(src => src.Alias != null))
                .ForMember(dest => dest.IsActive, opt => opt.Condition(src => src.IsActive.HasValue))
                .ForMember(dest => dest.Configuration, opt => opt.Condition(src => src.Configuration != null))
                .ForMember(dest => dest.DisplayOrder, opt => opt.Condition(src => src.DisplayOrder.HasValue));
        }
    }
}

