using OpaMenu.Domain.DTOs;
using OpaMenu.Infrastructure.Shared.Entities;
using OpaMenu.Infrastructure.Shared.Enums.Opamenu;

namespace OpaMenu.Web.Models.DTOs
{
    public class OrderResponseDto
    {
        public int Id { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerPhone { get; set; } = string.Empty;
        public string? CustomerEmail { get; set; }
        public string DeliveryAddress { get; set; } = string.Empty;
        public decimal Subtotal { get; set; }
        public decimal DeliveryFee { get; set; }
        public decimal Total { get; set; }
        public EOrderStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsDelivery { get; set; }
        public string? Notes { get; set; }
        public List<OrderItemResponseDto> Items { get; set; } = new();
    }
}

