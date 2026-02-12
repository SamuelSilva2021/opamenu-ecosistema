using OpaMenu.Domain.DTOs.PaymentMethod;

namespace OpaMenu.Domain.DTOs.TenantPaymentMethod
{
    public record TenantPaymentMethodResponseDto(
        Guid Id,
        Guid PaymentMethodId,
        PaymentMethodResponseDto PaymentMethod,
        string? Alias,
        bool IsActive,
        string? Configuration,
        int DisplayOrder
    );
}
