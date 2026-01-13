using System.ComponentModel.DataAnnotations;

namespace OpaMenu.Domain.DTOs.PaymentMethod
{
    public record UpdatePaymentMethodRequestDto(
        [MaxLength(100)] string? Name,
        [MaxLength(100)] string? Slug,
        [MaxLength(500)] string? Description,
        [MaxLength(500)] string? IconUrl,
        bool? IsActive,
        bool? IsOnline,
        int? DisplayOrder
    );
}
