using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using OpaMenu.Infrastructure.Shared.Enums;

namespace OpaMenu.Infrastructure.Shared.Entities;

[Table("orders")]
public class OrderEntity : BaseEntity
{
    [Required]
    [MaxLength(100)]
    [Column("customer_name")]
    public string CustomerName { get; set; } = string.Empty;

    [Required]
    [MaxLength(20)]
    [Column("customer_phone")]
    public string CustomerPhone { get; set; } = string.Empty;

    [MaxLength(100)]
    [Column("customer_email")]
    public string? CustomerEmail { get; set; }

    [Required]
    [MaxLength(500)]
    [Column("delivery_address")]
    public string DeliveryAddress { get; set; } = string.Empty;

    [Column("subtotal", TypeName = "decimal(10,2)")]
    public decimal Subtotal { get; set; }

    [Column("delivery_fee", TypeName = "decimal(10,2)")]
    public decimal DeliveryFee { get; set; }

    [Column("discount_amount", TypeName = "decimal(10,2)")]
    public decimal DiscountAmount { get; set; } = 0;

    [MaxLength(50)]
    [Column("coupon_code")]
    public string? CouponCode { get; set; }

    [Column("total", TypeName = "decimal(10,2)")]
    public decimal Total { get; set; }

    [Column("status")]
    public OrderStatus Status { get; set; } = OrderStatus.Pending;

    [MaxLength(1000)]
    [Column("notes")]
    public string? Notes { get; set; }

    [Column("is_delivery")]
    public bool IsDelivery { get; set; } = true;

    [Column("order_type")]
    public EOrderType OrderType { get; set; } = EOrderType.Delivery;

    [Column("table_id")]
    public int? TableId { get; set; }

    public virtual TableEntity? Table { get; set; }

    [Column("estimated_preparation_minutes")]
    public int? EstimatedPreparationMinutes { get; set; }
    
    [Column("estimated_delivery_time")]
    public DateTime? EstimatedDeliveryTime { get; set; }

    [Column("queue_position")]
    public int QueuePosition { get; set; } = 0;

    [Required]
    [Column("customer_id")]
    public Guid CustomerId { get; set; }

    public CustomerEntity Customer { get; set; } = null!;

    public virtual ICollection<OrderItemEntity> Items { get; set; } = new List<OrderItemEntity>();
    
    public virtual ICollection<OrderStatusHistoryEntity> StatusHistory { get; set; } = new List<OrderStatusHistoryEntity>();
    
    public virtual OrderRejectionEntity? Rejection { get; set; }
    
    public virtual ICollection<PaymentEntity> Payments { get; set; } = new List<PaymentEntity>();
}

public enum OrderStatus
{
    Pending = 0,
    Confirmed = 1,
    Preparing = 2,
    Ready = 3,
    OutForDelivery = 4,
    Delivered = 5,
    Cancelled = 6,
    Rejected = 7
}

