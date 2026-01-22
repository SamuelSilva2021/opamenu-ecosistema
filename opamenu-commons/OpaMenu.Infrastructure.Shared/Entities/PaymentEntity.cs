using OpaMenu.Infrastructure.Shared.Enums.Opamenu;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OpaMenu.Infrastructure.Shared.Entities;

[Table("payments")]
public class PaymentEntity : BaseEntity
{
    [Required]
    [Column("order_id")]
    public Guid OrderId { get; set; }
    
    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
    [Column("amount", TypeName = "decimal(10,2)")]
    public decimal Amount { get; set; }
    
    [Required]
    [Column("method")]
    public EPaymentMethod Method { get; set; }
    
    [Required]
    [Column("status")]
    public EPaymentStatus Status { get; set; }
    
    [MaxLength(200)]
    [Column("gateway_transaction_id")]
    public string? GatewayTransactionId { get; set; }
    
    [MaxLength(1000)]
    [Column("gateway_response")]
    public string? GatewayResponse { get; set; }
    
    [Column("processed_at")]
    public DateTime? ProcessedAt { get; set; }
    
    [MaxLength(500)]
    [Column("notes")]
    public string? Notes { get; set; }
    
    // Navigation properties
    public virtual OrderEntity Order { get; set; } = null!;
    public virtual ICollection<PaymentRefundEntity> Refunds { get; set; } = new List<PaymentRefundEntity>();
}
