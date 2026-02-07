using System.ComponentModel.DataAnnotations;

namespace OpaMenu.Domain.DTOs
{
    /// <summary>
    /// DTO para criação de adicionais de itens de pedido
    /// </summary>
    public class CreateOrderItemAditionalRequestDto
    {
        [Required]
        public Guid AditionalId { get; set; }

        [Required]
        [Range(1, 99, ErrorMessage = "Quantidade deve estar entre 1 e 99")]
        public int Quantity { get; set; }
    }
}
