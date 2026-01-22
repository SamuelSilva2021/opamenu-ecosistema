using OpaMenu.Infrastructure.Shared.Enums.Opamenu;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OpaMenu.Infrastructure.Shared.Entities;

[Table("loyalty_transactions")]
public class LoyaltyTransactionEntity : BaseEntity
{
    [Required]
    [Column("customer_loyalty_balance_id")]
    public Guid CustomerLoyaltyBalanceId { get; set; }

    [Column("order_id")]
    public Guid OrderId { get; set; }

    [Required]
    [Column("points")]
    public int Points { get; set; }

    [Required]
    [Column("type")]
    public ELoyaltyTransactionType Type { get; set; }

    [MaxLength(200)]
    [Column("description")]
    public string? Description { get; set; }

    [Column("expires_at")]
    public DateTime? ExpiresAt { get; set; }

    // Navigation properties
    [ForeignKey("CustomerLoyaltyBalanceId")]
    public virtual CustomerLoyaltyBalanceEntity CustomerLoyaltyBalance { get; set; } = null!;

    [ForeignKey("OrderId")]
    public virtual OrderEntity? Order { get; set; }
}
