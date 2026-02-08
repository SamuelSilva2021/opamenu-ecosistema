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
                    Id: src.Id,
                    Name: src.Name,
                    Slug: src.Slug,
                    Description: src.BusinessInfo?.Description,
                    LogoUrl: src.BusinessInfo?.LogoUrl,
                    BannerUrl: src.BusinessInfo?.BannerUrl,
                    InstagramUrl: src.BusinessInfo?.InstagramUrl,
                    FacebookUrl: src.BusinessInfo?.FacebookUrl,
                    WhatsappNumber: src.BusinessInfo?.WhatsappNumber,
                    Phone: src.Phone,
                    Email: src.Email,
                    AddressStreet: src.AddressStreet,
                    AddressNumber: src.AddressNumber,
                    AddressComplement: src.AddressComplement,
                    AddressNeighborhood: src.AddressNeighborhood,
                    AddressCity: src.AddressCity,
                    AddressState: src.AddressState,
                    AddressZipcode: src.AddressZipcode,
                    OpeningHours: Deserialize(src.BusinessInfo?.OpeningHours),
                    PaymentMethods: Deserialize(src.BusinessInfo?.PaymentMethods),
                    Latitude: src.BusinessInfo?.Latitude,
                    Longitude: src.BusinessInfo?.Longitude,
                    LoyaltyProgram: null,
                    PixKey: src.BankDetails?.FirstOrDefault(b => b.IsPixKeySelected)?.PixKey,
                    PixIntegration: false
                ));

            CreateMap<TenantBusinessEntity, TenantBusinessResponseDto>()
                .ConstructUsing((src, context) => new TenantBusinessResponseDto(
                    Id: src.TenantId,
                    Name: src.Tenant != null ? src.Tenant.Name : string.Empty,
                    Slug: src.Tenant != null ? src.Tenant.Slug : string.Empty,
                    Description: src.Description,
                    LogoUrl: src.LogoUrl,
                    BannerUrl: src.BannerUrl,
                    InstagramUrl: src.InstagramUrl,
                    FacebookUrl: src.FacebookUrl,
                    WhatsappNumber: src.WhatsappNumber,
                    Phone: src.Tenant != null ? src.Tenant.Phone : null,
                    Email: src.Tenant != null ? src.Tenant.Email : null,
                    AddressStreet: src.Tenant != null ? src.Tenant.AddressStreet : null,
                    AddressNumber: src.Tenant != null ? src.Tenant.AddressNumber : null,
                    AddressComplement: src.Tenant != null ? src.Tenant.AddressComplement : null,
                    AddressNeighborhood: src.Tenant != null ? src.Tenant.AddressNeighborhood : null,
                    AddressCity: src.Tenant != null ? src.Tenant.AddressCity : null,
                    AddressState: src.Tenant != null ? src.Tenant.AddressState : null,
                    AddressZipcode: src.Tenant != null ? src.Tenant.AddressZipcode : null,
                    OpeningHours: Deserialize(src.OpeningHours),
                    PaymentMethods: Deserialize(src.PaymentMethods),
                    Latitude: src.Latitude,
                    Longitude: src.Longitude,
                    LoyaltyProgram: null,
                    PixKey: src.Tenant != null && src.Tenant.BankDetails != null ? src.Tenant.BankDetails.FirstOrDefault(b => b.IsPixKeySelected)?.PixKey : null,
                    PixIntegration: false
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

