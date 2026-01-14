using Authenticator.API.Core.Domain.MultiTenant.Tenant.DTOs;
using OpaMenu.Infrastructure.Shared.Entities.MultiTenant.Tenant;
using AutoMapper;

namespace Authenticator.API.Infrastructure.Mapper.Tenant
{
    public class TenantProfile : Profile
    {
        public TenantProfile()
        {
            // TenantEntity -> TenantDto
            CreateMap<TenantEntity, TenantDTO>();

            // TenantEntity -> TenantSummaryDto
            CreateMap<TenantEntity, TenantSummaryDTO>();

            // CreateTenantDto -> TenantEntity
            CreateMap<CreateTenantDTO, TenantEntity>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.CompanyName))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => ETenantStatus.Ativo))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.ActiveSubscriptionId, opt => opt.Ignore())
                .ForMember(dest => dest.ActiveSubscription, opt => opt.Ignore())
                .ForMember(dest => dest.Subscriptions, opt => opt.Ignore());

            // UpdateTenantDto -> TenantEntity
            CreateMap<UpdateTenantDTO, TenantEntity>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.ActiveSubscriptionId, opt => opt.Ignore())
                .ForMember(dest => dest.ActiveSubscription, opt => opt.Ignore())
                .ForMember(dest => dest.Subscriptions, opt => opt.Ignore())
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) =>
                    srcMember != null && !(srcMember is string str && string.IsNullOrWhiteSpace(str))));
        }
    }
}

