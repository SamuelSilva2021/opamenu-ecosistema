using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OpaMenu.Infrastructure.Shared.Entities.Opamenu;

[Table("order_rejections")]
public class OrderRejectionEntity : BaseEntity    
{
    [Required]
    [Column("order_id")]
    public Guid OrderId { get; set; }

    [Required]
    [MaxLength(100)]
    [Column("reason")]
    public string Reason { get; set; } = string.Empty;

    [MaxLength(500)]
    [Column("notes")]
    public string? Notes { get; set; }

    [Required]
    [Column("rejected_at")]
    public DateTime RejectedAt { get; set; } = DateTime.UtcNow;

    [Required]
    [MaxLength(50)]
    [Column("rejected_by")]
    public string RejectedBy { get; set; } = string.Empty;

    [ForeignKey("OrderId")]
    public virtual OrderEntity Order { get; set; } = null!;
}

