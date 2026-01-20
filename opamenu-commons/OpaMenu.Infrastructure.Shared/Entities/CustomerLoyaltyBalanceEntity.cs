using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OpaMenu.Infrastructure.Shared.Entities;

[Table("customer_loyalty_balances")]
public class CustomerLoyaltyBalanceEntity : BaseEntity
{
    [Required]
    [Column("customer_id")]
    public Guid CustomerId { get; set; }

    [Required]
    [Column("balance")]
    public int Balance { get; set; } = 0;

    [Required]
    [Column("total_earned")]
    public int TotalEarned { get; set; } = 0;

    [Required]
    [Column("last_activity_at")]
    public DateTime LastActivityAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public virtual CustomerEntity Customer { get; set; } = null!;
    public virtual ICollection<LoyaltyTransactionEntity> Transactions { get; set; } = new List<LoyaltyTransactionEntity>();
}
