using AutoMapper;
using OpaMenu.Domain.DTOs.PaymentMethod;
using OpaMenu.Infrastructure.Shared.Entities;
using OpaMenu.Infrastructure.Shared.Entities.Opamenu;

namespace OpaMenu.Infrastructure.Mapper
{
    public class PaymentMethodMappingProfile : Profile
    {
        public PaymentMethodMappingProfile()
        {
            CreateMap<PaymentMethodEntity, PaymentMethodResponseDto>();

            CreateMap<CreatePaymentMethodRequestDto, PaymentMethodEntity>()
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true));

            CreateMap<UpdatePaymentMethodRequestDto, PaymentMethodEntity>()
                .ForMember(dest => dest.Name, opt => opt.Condition(src => src.Name != null))
                .ForMember(dest => dest.Slug, opt => opt.Condition(src => src.Slug != null))
                .ForMember(dest => dest.Description, opt => opt.Condition(src => src.Description != null))
                .ForMember(dest => dest.IconUrl, opt => opt.Condition(src => src.IconUrl != null))
                .ForMember(dest => dest.IsActive, opt => opt.Condition(src => src.IsActive.HasValue))
                .ForMember(dest => dest.IsOnline, opt => opt.Condition(src => src.IsOnline.HasValue))
                .ForMember(dest => dest.DisplayOrder, opt => opt.Condition(src => src.DisplayOrder.HasValue));
        }
    }
}

