using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OpaMenu.Infrastructure.Shared.Entities
{
    [Table("payment_methods")]
    public class PaymentMethodEntity : BaseEntity
    {
        [Required]
        [MaxLength(100)]
        [Column("name")]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        [Column("slug")]
        public string Slug { get; set; } = string.Empty;

        [MaxLength(500)]
        [Column("description")]
        public string? Description { get; set; }

        [MaxLength(500)]
        [Column("icon_url")]
        public string? IconUrl { get; set; }

        [Column("is_active")]
        public bool IsActive { get; set; } = true;

        [Column("is_online")]
        public bool IsOnline { get; set; }

        [Column("display_order")]
        public int DisplayOrder { get; set; }

        public virtual ICollection<TenantPaymentMethodEntity> TenantConfiguredMethods { get; set; } = new List<TenantPaymentMethodEntity>();
    }
}

