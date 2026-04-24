using System.ComponentModel.DataAnnotations;

namespace OpaMenu.Domain.DTOs
{
    public class UpdateOrderDeliveryTypeRequestDto
    {
        [Required]
        public bool IsDelivery { get; set; }
        
        [MaxLength(500)]
        public string? DeliveryAddress { get; set; }
    }
}
