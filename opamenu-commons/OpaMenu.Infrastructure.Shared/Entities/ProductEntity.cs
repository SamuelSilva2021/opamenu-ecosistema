using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OpaMenu.Infrastructure.Shared.Entities;

[Table("products")]
public class ProductEntity : BaseEntity
{
    [Required]
    [MaxLength(100)]
    [Column("name")]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500)]
    [Column("description")]
    public string? Description { get; set; }

    [Column("price", TypeName = "decimal(18,2)")]
    public decimal Price { get; set; }

    [Column("category_id")]
    public Guid CategoryId { get; set; }

    [Column("display_order")]
    public int DisplayOrder { get; set; }

    [Column("is_active")]
    public bool IsActive { get; set; } = true;

    [MaxLength(500)]
    [Column("image_url")]
    public string? ImageUrl { get; set; }

    // Navigation property
    public virtual CategoryEntity Category { get; set; } = null!;  
    public virtual ICollection<ProductImageEntity> Images { get; set; } = new List<ProductImageEntity>();
    public virtual ICollection<ProductAddonGroupEntity> AddonGroups { get; set; } = new List<ProductAddonGroupEntity>();
}

