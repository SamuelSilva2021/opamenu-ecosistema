using System.ComponentModel.DataAnnotations;

namespace Opamenu.Domain.DTOs
{
    public class UpdateOrderDeliveryTypeRequestDto
    {
        [Required]
        public bool IsDelivery { get; set; }
        
        [MaxLength(500)]
        public string? DeliveryAddress { get; set; }
    }
}
