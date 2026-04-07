using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using OpaMenu.Infrastructure.Shared.Enums.Opamenu;

namespace OpaMenu.Infrastructure.Shared.Entities.Opamenu;

/// <summary>
/// Tabela de comandas
/// </summary>
[Table("tabs")]
public class TabEntity : BaseEntity
{
    [Required]
    [Column("table_id")]
    public Guid TableId { get; set; }

    [ForeignKey(nameof(TableId))]
    public virtual TableEntity Table { get; set; } = null!;

    [MaxLength(50)]
    [Column("name")]
    public string? Name { get; set; }

    [Column("status")]
    public ETabStatus Status { get; set; } = ETabStatus.Open;

    [Column("opened_at")]
    public DateTime OpenedAt { get; set; } = DateTime.UtcNow;

    [Column("closed_at")]
    public DateTime? ClosedAt { get; set; }

    public virtual ICollection<OrderEntity> Orders { get; set; } = new List<OrderEntity>();
}
