using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OpaMenu.Infrastructure.Shared.Entities.Opamenu;

[Table("delivery_areas")]
public class DeliveryAreaEntity : BaseEntity
{
    [Required]
    [MaxLength(100)]
    [Column("city")]
    public string City { get; set; } = string.Empty;

    [MaxLength(100)]
    [Column("neighborhood")]
    public string? Neighborhood { get; set; }

    [Column("fee", TypeName = "decimal(18,2)")]
    public decimal Fee { get; set; }

    [Column("is_active")]
    public bool IsActive { get; set; } = true;
}
