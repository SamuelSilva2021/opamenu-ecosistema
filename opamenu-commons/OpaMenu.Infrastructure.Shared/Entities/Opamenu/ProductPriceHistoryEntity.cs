using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OpaMenu.Infrastructure.Shared.Entities.Opamenu
{
    [Table("product_price_histories")]
    public class ProductPriceHistoryEntity : BaseEntity
    {

        [Required]
        [Column("product_id")]
        public int ProductId { get; set; }

        [Required]
        [Column("previous_price", TypeName = "decimal(10,2)")]
        public decimal PreviousPrice { get; set; }

        [Required]
        [Column("new_price", TypeName = "decimal(10,2)")]
        public decimal NewPrice { get; set; }

        [Required]
        [Column("changed_at")]
        public DateTime ChangedAt { get; set; } = DateTime.UtcNow;

        [MaxLength(100)]
        [Column("changed_by")]
        public string? ChangedBy { get; set; }

        [MaxLength(200)]
        [Column("reason")]
        public string? Reason { get; set; }

        // Navigation Properties
        [ForeignKey("ProductId")]
        public virtual ProductEntity Product { get; set; } = null!;
    }
}

