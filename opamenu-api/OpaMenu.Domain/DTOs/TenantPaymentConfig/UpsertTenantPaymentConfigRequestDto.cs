using OpaMenu.Infrastructure.Shared.Enums.Opamenu;
using System.ComponentModel.DataAnnotations;

namespace OpaMenu.Domain.DTOs.TenantPaymentConfig
{
    public record UpsertTenantPaymentConfigRequestDto(
        [Required] EPaymentProvider Provider,
        [Required] EPaymentMethod PaymentMethod,
        [Required] string PixKey,
        [Required] string ClientId,
        [Required] string ClientSecret,
        string? PublicKey,
        string? AccessToken,
        bool? IsSandbox,
        bool IsActive
    );
}
