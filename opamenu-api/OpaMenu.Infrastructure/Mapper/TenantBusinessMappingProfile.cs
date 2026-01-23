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
                    src.BusinessInfo?.Description,
                    src.BusinessInfo?.LogoUrl,
                    src.BusinessInfo?.BannerUrl,
                    src.BusinessInfo?.InstagramUrl,
                    src.BusinessInfo?.FacebookUrl,
                    src.BusinessInfo?.WhatsappNumber,
                    src.Phone,
                    src.Email,
                    src.AddressStreet,
                    src.AddressNumber,
                    src.AddressComplement,
                    src.AddressNeighborhood,
                    src.AddressCity,
                    src.AddressState,
                    src.AddressZipcode,
                    Deserialize(src.BusinessInfo?.OpeningHours),
                    Deserialize(src.BusinessInfo?.PaymentMethods),
                    src.BusinessInfo?.Latitude,
                    src.BusinessInfo?.Longitude,
                    null,
                    src.BankDetails?.FirstOrDefault(b => b.IsPixKeySelected)?.PixKey
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
                    src.Longitude,
                    null,
                    src.Tenant != null && src.Tenant.BankDetails != null ? src.Tenant.BankDetails.FirstOrDefault(b => b.IsPixKeySelected)?.PixKey : null
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
                var dict = JsonSerializer.Deserialize<Dictionary<string, object>>(json);
                if (dict != null && dict.ContainsKey("pixKey"))
                {
                    dict.Remove("pixKey");
                    return dict;
                }
                return dict ?? JsonSerializer.Deserialize<object>(json);
            }
            catch
            {
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
}

