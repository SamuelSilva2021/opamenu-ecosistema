using AutoMapper;
using OpaMenu.Domain.DTOs.Tenant;
using OpaMenu.Infrastructure.Shared.Entities.MultiTenant.Tenant;
using System.Text.Json;

namespace OpaMenu.Infrastructure.Mapper
{
    public class TenantBusinessMappingProfile : Profile
    {
        public TenantBusinessMappingProfile()
        {
            CreateMap<TenantEntity, TenantBusinessResponseDto>()
                .ConstructUsing((src, context) => new TenantBusinessResponseDto(
                    src.Id,
                    src.Name,
                    src.Slug,
                    src.BusinessInfo != null ? src.BusinessInfo.Description : null,
                    src.BusinessInfo != null ? src.BusinessInfo.LogoUrl : null,
                    src.BusinessInfo != null ? src.BusinessInfo.BannerUrl : null,
                    src.BusinessInfo != null ? src.BusinessInfo.InstagramUrl : null,
                    src.BusinessInfo != null ? src.BusinessInfo.FacebookUrl : null,
                    src.BusinessInfo != null ? src.BusinessInfo.WhatsappNumber : null,
                    src.Phone,
                    src.Email,
                    src.AddressStreet,
                    src.AddressNumber,
                    src.AddressComplement,
                    src.AddressNeighborhood,
                    src.AddressCity,
                    src.AddressState,
                    src.AddressZipcode,
                    Deserialize(src.BusinessInfo != null ? src.BusinessInfo.OpeningHours : null),
                    Deserialize(src.BusinessInfo != null ? src.BusinessInfo.PaymentMethods : null),
                    src.BusinessInfo != null ? src.BusinessInfo.Latitude : null,
                    src.BusinessInfo != null ? src.BusinessInfo.Longitude : null
                ));

            CreateMap<TenantBusinessEntity, TenantBusinessResponseDto>()
                .ConstructUsing((src, context) => new TenantBusinessResponseDto(
                    src.TenantId,
                    src.Tenant != null ? src.Tenant.Name : string.Empty,
                    src.Tenant != null ? src.Tenant.Slug : string.Empty,
                    src.Description,
                    src.LogoUrl,
                    src.BannerUrl,
                    src.InstagramUrl,
                    src.FacebookUrl,
                    src.WhatsappNumber,
                    src.Tenant != null ? src.Tenant.Phone : null,
                    src.Tenant != null ? src.Tenant.Email : null,
                    src.Tenant != null ? src.Tenant.AddressStreet : null,
                    src.Tenant != null ? src.Tenant.AddressNumber : null,
                    src.Tenant != null ? src.Tenant.AddressComplement : null,
                    src.Tenant != null ? src.Tenant.AddressNeighborhood : null,
                    src.Tenant != null ? src.Tenant.AddressCity : null,
                    src.Tenant != null ? src.Tenant.AddressState : null,
                    src.Tenant != null ? src.Tenant.AddressZipcode : null,
                    Deserialize(src.OpeningHours),
                    Deserialize(src.PaymentMethods),
                    src.Latitude,
                    src.Longitude
                ));

            CreateMap<UpdateTenantBusinessRequestDto, TenantBusinessEntity>()
                .ForMember(dest => dest.OpeningHours, opt => opt.MapFrom(src => Serialize(src.OpeningHours)))
                .ForMember(dest => dest.PaymentMethods, opt => opt.MapFrom(src => Serialize(src.PaymentMethods)));
        }

        private static string? Serialize(object? obj)
        {
            if (obj == null) return null;
            return JsonSerializer.Serialize(obj);
        }

        private static object? Deserialize(string? json)
        {
            if (string.IsNullOrEmpty(json)) return null;
            try
            {
                return JsonSerializer.Deserialize<object>(json);
            }
            catch
            {
                return null;
            }
        }
    }
}

