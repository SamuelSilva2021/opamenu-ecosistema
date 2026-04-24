using System.ComponentModel.DataAnnotations;
using OpaMenu.Infrastructure.Shared.Enums.Opamenu;

namespace OpaMenu.Domain.DTOs
{
    public class UpdateOrderPaymentRequestDto
    {
        [Required]
        public EPaymentMethod PaymentMethod { get; set; }
    }
}
