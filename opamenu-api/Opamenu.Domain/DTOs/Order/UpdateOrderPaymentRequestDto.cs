using System.ComponentModel.DataAnnotations;
using Opamenu.Infrastructure.Shared.Enums.Opamenu;

namespace Opamenu.Domain.DTOs
{
    public class UpdateOrderPaymentRequestDto
    {
        [Required]
        public EPaymentMethod PaymentMethod { get; set; }
    }
}
