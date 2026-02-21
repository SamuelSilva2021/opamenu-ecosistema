using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using OpaMenu.Infrastructure.Shared.Enums.Opamenu;

namespace OpaMenu.Infrastructure.Shared.Entities.Opamenu;

[Table("cash_movements")]
public class CashMovementEntity : BaseEntity
{
    [Column("shift_id")]
    public Guid ShiftId { get; set; }

    [ForeignKey("ShiftId")]
    public virtual CashShiftEntity Shift { get; set; } = null!;

    [Column("type")]
    public ECashMovementType Type { get; set; }

    [Column("amount", TypeName = "decimal(18,2)")]
    public decimal Amount { get; set; }

    [MaxLength(500)]
    [Column("description")]
    public string? Description { get; set; }

    [Column("payment_method")]
    public EPaymentMethod? PaymentMethod { get; set; }

    [Column("order_id")]
    public Guid? OrderId { get; set; }

    [ForeignKey("OrderId")]
    public virtual OrderEntity? Order { get; set; }
}
