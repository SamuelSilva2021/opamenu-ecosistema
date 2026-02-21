using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using OpaMenu.Infrastructure.Shared.Enums.Opamenu;

namespace OpaMenu.Infrastructure.Shared.Entities.Opamenu;

[Table("cash_shifts")]
public class CashShiftEntity : BaseEntity
{
    [Column("user_id")]
    public Guid UserId { get; set; }

    [Column("opened_at")]
    public DateTime OpenedAt { get; set; }

    [Column("closed_at")]
    public DateTime? ClosedAt { get; set; }

    [Column("opening_balance", TypeName = "decimal(18,2)")]
    public decimal OpeningBalance { get; set; }

    [Column("closing_balance", TypeName = "decimal(18,2)")]
    public decimal? ClosingBalance { get; set; }

    [Column("expected_balance", TypeName = "decimal(18,2)")]
    public decimal ExpectedBalance { get; set; }

    [Column("status")]
    public ECashShiftStatus Status { get; set; } = ECashShiftStatus.Open;

    public virtual ICollection<CashMovementEntity> Movements { get; set; } = new List<CashMovementEntity>();
}
