using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OpaMenu.Infrastructure.Shared.Entities.Opamenu;

[Table("product_compositions")]
public class ProductCompositionEntity : BaseEntity
{
    [Required]
    [Column("product_id")]
    public Guid ProductId { get; set; }

    [Required]
    [Column("ingredient_id")]
    public Guid IngredientId { get; set; }

    [Required]
    [Column("quantity_required", TypeName = "decimal(18,4)")]
    public decimal QuantityRequired { get; set; }

    // Navigation properties
    public virtual ProductEntity Product { get; set; } = null!;
    public virtual IngredientEntity Ingredient { get; set; } = null!;
}
