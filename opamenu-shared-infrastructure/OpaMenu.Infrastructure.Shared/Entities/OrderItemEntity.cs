using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OpaMenu.Infrastructure.Shared.Entities;

[Table("order_items")]
public class OrderItemEntity
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("order_id")]
    public int OrderId { get; set; }

    [Column("product_id")]
    public int ProductId { get; set; }

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

    // Navigation properties
    public virtual OrderEntity Order { get; set; } = null!;
    public virtual ProductEntity Product { get; set; } = null!;
    public virtual ICollection<OrderItemAddon> Addons { get; set; } = new List<OrderItemAddon>();
}

