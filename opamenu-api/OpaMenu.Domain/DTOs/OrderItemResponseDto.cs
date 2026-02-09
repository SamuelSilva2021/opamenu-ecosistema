using OpaMenu.Domain.DTOs;

namespace OpaMenu.Domain.DTOs
{
    /// <summary>
    /// DTO de resposta para itens de pedido
    /// </summary>
    public class OrderItemResponseDto
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal Subtotal { get; set; }
        public string? Notes { get; set; }
        public string? ImageUrl { get; set; }
        public int Status { get; set; }
        public List<OrderItemAditionalResponseDto> Aditionals { get; set; } = new();
    }
}
