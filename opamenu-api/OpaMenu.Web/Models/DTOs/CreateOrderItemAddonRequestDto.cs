using System.ComponentModel.DataAnnotations;

namespace OpaMenu.Web.Models.DTOs
{
    public class CreateOrderItemAddonRequestDto
    {
        [Required]
        public Guid AddonId { get; set; }

        [Required]
        [Range(1, 10, ErrorMessage = "Quantidade do adicional deve estar entre 1 e 10")]
        public int Quantity { get; set; } = 1;
    }
}
