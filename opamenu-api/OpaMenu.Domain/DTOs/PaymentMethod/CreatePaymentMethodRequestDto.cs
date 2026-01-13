using System.ComponentModel.DataAnnotations;

namespace OpaMenu.Domain.DTOs.PaymentMethod
{
    public record CreatePaymentMethodRequestDto(
        [Required] [MaxLength(100)] string Name,
        [Required] [MaxLength(100)] string Slug,
        [MaxLength(500)] string? Description,
        [MaxLength(500)] string? IconUrl,
        bool IsOnline,
        int DisplayOrder = 0
    );
}
