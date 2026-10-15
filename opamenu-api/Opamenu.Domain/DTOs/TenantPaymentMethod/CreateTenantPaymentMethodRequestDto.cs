using System.ComponentModel.DataAnnotations;

namespace Opamenu.Domain.DTOs.TenantPaymentMethod
{
    public record CreateTenantPaymentMethodRequestDto(
        [Required] int PaymentMethodId,
        [MaxLength(100)] string? Alias,
        bool IsActive = true,
        string? Configuration = null,
        int DisplayOrder = 0
    );
}
