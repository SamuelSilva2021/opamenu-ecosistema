using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OpaMenu.Infrastructure.Shared.Entities
{
    [Table("product_activity_logs")]
    public class ProductActivityLogEntity : BaseEntity
    {
        [Required]
        [Column("product_id")]
        public int ProductId { get; set; }

        [Required]
        [MaxLength(50)]
        [Column("action")]
        public string Action { get; set; } = string.Empty; // 'price_change', 'toggle_availability', 'bulk_update'

        [MaxLength(200)]
        [Column("previous_value")]
        public string? PreviousValue { get; set; }

        [MaxLength(200)]
        [Column("new_value")]
        public string? NewValue { get; set; }

        [Required]
        [Column("timestamp")]
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        [MaxLength(100)]
        [Column("user_id")]
        public string? UserId { get; set; }

        // Navigation Properties
        [ForeignKey("ProductId")]
        public virtual ProductEntity Product { get; set; } = null!;
    }
}

