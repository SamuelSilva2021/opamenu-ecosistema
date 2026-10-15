using Opamenu.Domain.DTOs.PaymentMethod;

namespace Opamenu.Domain.DTOs.TenantPaymentMethod
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
