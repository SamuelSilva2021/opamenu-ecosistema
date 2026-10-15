using System.ComponentModel.DataAnnotations;

namespace Opamenu.Domain.DTOs.TenantPaymentMethod
{
    public record UpdateTenantPaymentMethodRequestDto(
        [MaxLength(100)] string? Alias,
        bool? IsActive,
        string? Configuration,
        int? DisplayOrder
    );
}
