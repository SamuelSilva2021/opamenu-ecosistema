using AutoMapper;
using OpaMenu.Domain.DTOs.Plan;
using OpaMenu.Domain.DTOs.Product;
using OpaMenu.Domain.DTOs.Subscription;
using OpaMenu.Domain.DTOs.Tenant;
using OpaMenu.Infrastructure.Shared.Entities.MultiTenant.Plan;
using OpaMenu.Infrastructure.Shared.Entities.MultiTenant.Subscription;
using OpaMenu.Infrastructure.Shared.Entities.MultiTenant.Tenant;
using OpaMenu.Infrastructure.Shared.Entities.MultiTenant.TenantProduct;
using System;

namespace OpaMenu.Infrastructure.Mapper
{
    public class SubscriptionMappingProfile : Profile
    {
        public SubscriptionMappingProfile()
        {
            CreateMap<SubscriptionEntity, SubscriptionStatusResponseDto>()
                .ForMember(dest => dest.Tenant, opt => opt.MapFrom(src => src.Tenant))
                .ForMember(dest => dest.Product, opt => opt.MapFrom(src => src.Product))
                .ForMember(dest => dest.Plan, opt => opt.MapFrom(src => src.Plan))
                .ForMember(dest => dest.PlanName, opt => opt.MapFrom(src => src.Plan != null ? src.Plan.Name : "Plano nÃ£o identificado"))
                .ForMember(dest => dest.DaysRemaining,
                    opt => opt.MapFrom((src, _, _, ctx) =>
                    {
                        var now = DateTime.UtcNow;
                        var days = (int)(src.CurrentPeriodEnd - now).TotalDays;
                        return days < 0 ? 0 : days;
                    }))
                .ForMember(dest => dest.IsTrial,
                    opt => opt.MapFrom((src, _, _, ctx) =>
                    {
                        var now = DateTime.UtcNow;
                        return src.TrialEndsAt.HasValue && src.TrialEndsAt.Value > now;
                    }));

            CreateMap<TenantEntity, TenantSummaryDto>();

            CreateMap<TenantProductEntity, ProductSummaryDto>();

            CreateMap<PlanEntity, PlanSummaryDto>();
        }
    }
}

