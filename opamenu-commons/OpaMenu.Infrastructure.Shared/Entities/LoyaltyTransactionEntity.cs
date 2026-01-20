using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OpaMenu.Infrastructure.Shared.Entities;

public enum LoyaltyTransactionType
{
    Earn = 1,
    Redeem = 2,
    Adjustment = 3,
    Expired = 4
}

[Table("loyalty_transactions")]
public class LoyaltyTransactionEntity : BaseEntity
{
    [Required]
    [Column("customer_loyalty_balance_id")]
    public int CustomerLoyaltyBalanceId { get; set; }

    [Column("order_id")]
    public int? OrderId { get; set; }

    [Required]
    [Column("points")]
    public int Points { get; set; }

    [Required]
    [Column("type")]
    public LoyaltyTransactionType Type { get; set; }

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
