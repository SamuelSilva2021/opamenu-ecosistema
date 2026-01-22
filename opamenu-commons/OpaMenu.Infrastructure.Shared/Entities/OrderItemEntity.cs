using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OpaMenu.Infrastructure.Shared.Entities;

[Table("order_items")]
public class OrderItemEntity : BaseEntity
{
    [Column("order_id")]
    public Guid OrderId { get; set; }

    [Column("product_id")]
    public Guid ProductId { get; set; }

    [Required]
    [MaxLength(100)]
    [Column("product_name")]
    public string ProductName { get; set; } = string.Empty;

    [Column("unit_price", TypeName = "decimal(10,2)")]
    public decimal UnitPrice { get; set; }

    [Column("quantity")]
    public int Quantity { get; set; }

    [Column("subtotal", TypeName = "decimal(10,2)")]
    public decimal Subtotal { get; set; }

    [MaxLength(500)]
    [Column("notes")]
    public string? Notes { get; set; }

    public virtual OrderEntity Order { get; set; } = null!;
    public virtual ProductEntity Product { get; set; } = null!;
    public virtual ICollection<OrderItemAddonEntity> Addons { get; set; } = new List<OrderItemAddonEntity>();
}

