using System;
using OpaMenu.Domain.DTOs.Loyalty;

namespace OpaMenu.Domain.DTOs.Tenant
{
    public record TenantBusinessResponseDto(
        Guid Id,
        string Name,
        string Slug,
        string? Description,
        string? LogoUrl,
        string? BannerUrl,
        string? InstagramUrl,
        string? FacebookUrl,
        string? WhatsappNumber,
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
        LoyaltyProgramDto? LoyaltyProgram
    );
}
