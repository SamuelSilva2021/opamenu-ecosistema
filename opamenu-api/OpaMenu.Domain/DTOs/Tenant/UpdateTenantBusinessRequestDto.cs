using System.ComponentModel.DataAnnotations;

namespace OpaMenu.Domain.DTOs.Tenant
{
    public record UpdateTenantBusinessRequestDto(
        [MaxLength(100)] string? Name,
        [MaxLength(500)] string? LogoUrl,
        [MaxLength(500)] string? BannerUrl,
        string? Description,
        [MaxLength(255)] string? InstagramUrl,
        [MaxLength(255)] string? FacebookUrl,
        [MaxLength(20)] string? WhatsappNumber,
        string? Phone,
        string? Email,
        string? AddressStreet,
        string? AddressNumber,
        string? AddressComplement,
        string? AddressNeighborhood,
        string? AddressCity,
        string? AddressState,
        string? AddressZipcode,
        object? OpeningHours,
        object? PaymentMethods,
        double? Latitude,
        double? Longitude,
        string? PixKey
    );
}
