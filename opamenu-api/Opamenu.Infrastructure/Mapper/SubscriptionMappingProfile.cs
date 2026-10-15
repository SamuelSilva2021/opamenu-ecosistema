using AutoMapper;
using Opamenu.Domain.DTOs.Plan;
using Opamenu.Domain.DTOs.Product;
using Opamenu.Domain.DTOs.Subscription;
using Opamenu.Domain.DTOs.Tenant;
using Opamenu.Infrastructure.Shared.Entities.MultiTenant.Plan;
using Opamenu.Infrastructure.Shared.Entities.MultiTenant.Subscription;
using Opamenu.Infrastructure.Shared.Entities.MultiTenant.Tenant;
using Opamenu.Infrastructure.Shared.Entities.MultiTenant.TenantProduct;
using System;

namespace Opamenu.Infrastructure.Mapper
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

