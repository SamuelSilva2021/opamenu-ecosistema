using System.ComponentModel.DataAnnotations;
using OpaMenu.Infrastructure.Shared.Entities;
using OpaMenu.Infrastructure.Shared.Enums.Opamenu;

namespace OpaMenu.Domain.DTOs
{
    public class CancelOrderRequestDto
    {
        [Required]
        [MaxLength(500)]
        public string Reason { get; set; } = string.Empty;
    }

    public class UpdateOrderPaymentRequestDto
    {
        [Required]
        public EPaymentMethod PaymentMethod { get; set; }
    }

    public class UpdateOrderDeliveryTypeRequestDto
    {
        [Required]
        public bool IsDelivery { get; set; }
        
        [MaxLength(500)]
        public string? DeliveryAddress { get; set; }
    }
}


