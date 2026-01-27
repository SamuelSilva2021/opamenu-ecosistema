using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OpaMenu.Infrastructure.Shared.Entities.Opamenu;

[Table("payment_refunds")]
public class PaymentRefundEntity : BaseEntity
{
        
        [Required]
        [Column("payment_id")]
        public Guid PaymentId { get; set; }
        
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Refund amount must be greater than 0")]
        [Column("amount", TypeName = "decimal(10,2)")]
        public decimal Amount { get; set; }
        
        [Required]
        [MaxLength(500)]
        [Column("reason")]
        public string Reason { get; set; } = string.Empty;
        
        [Column("refunded_at")]
        public DateTime RefundedAt { get; set; } = DateTime.UtcNow;
        
        [Required]
        [MaxLength(100)]
        [Column("refunded_by")]
        public string RefundedBy { get; set; } = string.Empty;
        
        [MaxLength(200)]
        [Column("gateway_refund_id")]
        public string? GatewayRefundId { get; set; }
        
        [MaxLength(1000)]
        [Column("gateway_response")]
        public string? GatewayResponse { get; set; }
        
        public virtual PaymentEntity Payment { get; set; } = null!;
}

