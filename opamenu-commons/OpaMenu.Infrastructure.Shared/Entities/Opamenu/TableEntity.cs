using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OpaMenu.Infrastructure.Shared.Entities.Opamenu;

[Table("tables")]
public class TableEntity : BaseEntity
{
    [Required]
    [MaxLength(50)]
    [Column("name")]
    public string Name { get; set; } = string.Empty;

    [Column("capacity")]
    public int Capacity { get; set; }

    [Column("is_active")]
    public bool IsActive { get; set; } = true;

    [Column("qr_code_url")]
    [MaxLength(500)]
    public string? QrCodeUrl { get; set; }

    [Column("layout_x")]
    public double LayoutX { get; set; } = 50.0;

    [Column("layout_y")]
    public double LayoutY { get; set; } = 50.0;

    [Column("layout_width")]
    public double LayoutWidth { get; set; } = 80.0;

    [Column("layout_height")]
    public double LayoutHeight { get; set; } = 80.0;

    [Column("floor")]
    [MaxLength(50)]
    public string? Floor { get; set; }

    public virtual ICollection<TabEntity> Tabs { get; set; } = new List<TabEntity>();

    public virtual ICollection<OrderEntity> Orders { get; set; } = new List<OrderEntity>();
}
