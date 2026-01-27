using OpaMenu.Infrastructure.Shared.Enums.Opamenu;

namespace OpaMenu.Domain.DTOs.TenantPaymentConfig
{
    public record TenantPaymentConfigResponseDto(
        Guid Id,
        EPaymentProvider Provider,
        EPaymentMethod PaymentMethod,
        string PixKey,
        string ClientId,
        string ClientSecret,
        string? PublicKey,
        string? AccessToken,
        bool? IsSandbox,
        bool IsActive
    );
}
