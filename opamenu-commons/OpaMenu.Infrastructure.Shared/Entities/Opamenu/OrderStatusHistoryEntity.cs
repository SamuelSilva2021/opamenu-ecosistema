using OpaMenu.Infrastructure.Shared.Enums.Opamenu;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OpaMenu.Infrastructure.Shared.Entities.Opamenu;

[Table("order_status_histories")]
public class OrderStatusHistoryEntity : BaseEntity
{
    [Required]
    [Column("order_id")]
    public Guid OrderId { get; set; }

    [Required]
    [Column("status")]
    public EOrderStatus Status { get; set; }

    [Required]
    [Column("timestamp")]
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    [MaxLength(500)]
    [Column("notes")]
    public string? Notes { get; set; }

    [Required]
    [MaxLength(50)]
    [Column("user_id")]
    public Guid UserId { get; set; }

    [ForeignKey("OrderId")]
    public virtual OrderEntity Order { get; set; } = null!;
}

