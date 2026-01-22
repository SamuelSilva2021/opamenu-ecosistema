using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OpaMenu.Infrastructure.Shared.Entities;

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

    public virtual ICollection<OrderEntity> Orders { get; set; } = new List<OrderEntity>();
}

