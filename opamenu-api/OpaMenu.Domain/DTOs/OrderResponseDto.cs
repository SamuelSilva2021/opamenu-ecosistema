
using OpaMenu.Infrastructure.Shared.Entities;
using OpaMenu.Infrastructure.Shared.Enums.Opamenu;

namespace OpaMenu.Domain.DTOs
{
    /// <summary>
    /// DTO de resposta para pedidos
    /// </summary>
    public class OrderResponseDto
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerPhone { get; set; } = string.Empty;
        public string? CustomerEmail { get; set; }
        public string DeliveryAddress { get; set; } = string.Empty;
        public decimal Subtotal { get; set; }
        public decimal DeliveryFee { get; set; }
        public decimal DiscountAmount { get; set; }
        public string? CouponCode { get; set; }
        public decimal Total { get; set; }
        public EOrderStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool IsDelivery { get; set; }
        public EOrderType OrderType { get; set; }
        public Guid? TableId { get; set; }
        public string? Notes { get; set; }
        public int? EstimatedPreparationMinutes { get; set; }
        public DateTime? EstimatedDeliveryTime { get; set; }
        public int? QueuePosition { get; set; }
        public List<OrderItemResponseDto> Items { get; set; } = new();
    }
}

