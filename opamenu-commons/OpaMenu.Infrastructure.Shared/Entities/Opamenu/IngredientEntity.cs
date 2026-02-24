using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OpaMenu.Infrastructure.Shared.Entities.Opamenu;

[Table("ingredients")]
public class IngredientEntity : BaseEntity
{
    [Required]
    [MaxLength(100)]
    [Column("name")]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(20)]
    [Column("unit_of_measure")]
    public string UnitOfMeasure { get; set; } = string.Empty;

    [Column("cost_price", TypeName = "decimal(18,4)")]
    public decimal CostPrice { get; set; }

    [Column("stock_quantity", TypeName = "decimal(18,4)")]
    public decimal StockQuantity { get; set; }

    [Column("minimum_stock", TypeName = "decimal(18,4)")]
    public decimal MinimumStock { get; set; }

    [Column("is_active")]
    public bool IsActive { get; set; } = true;

    // Navigation property for compositions where this ingredient is used
    public virtual ICollection<ProductCompositionEntity> Compositions { get; set; } = new List<ProductCompositionEntity>();
}
