using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OpaMenu.Infrastructure.Shared.Entities.Opamenu;

[Table("loyalty_program_filters")]
public class LoyaltyProgramFilterEntity : BaseEntity
{
    [Required]
    [Column("loyalty_program_id")]
    public Guid LoyaltyProgramId { get; set; }

    [Column("product_id")]
    public Guid? ProductId { get; set; }

    [Column("category_id")]
    public Guid? CategoryId { get; set; }

    // Navigation properties
    [ForeignKey("LoyaltyProgramId")]
    public virtual LoyaltyProgramEntity LoyaltyProgram { get; set; } = null!;

    [ForeignKey("ProductId")]
    public virtual ProductEntity? Product { get; set; }

    [ForeignKey("CategoryId")]
    public virtual CategoryEntity? Category { get; set; }
}
