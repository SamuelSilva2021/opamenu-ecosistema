using System.ComponentModel.DataAnnotations;

namespace OpaMenu.Domain.DTOs
{
    /// <summary>
    /// DTO para criação de itens de pedido
    /// </summary>
    public class CreateOrderItemRequestDto
    {
        [Required]
        public Guid ProductId { get; set; }

        [Required]
        [Range(1, 99, ErrorMessage = "Quantidade deve estar entre 1 e 99")]
        public int Quantity { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }
        
        public List<CreateOrderItemAditionalRequestDto> Aditionals { get; set; } = new();
    }
}
