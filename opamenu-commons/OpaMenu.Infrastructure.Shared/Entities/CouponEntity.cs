using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using OpaMenu.Infrastructure.Shared.Enums.Opamenu;

namespace OpaMenu.Infrastructure.Shared.Entities;

[Table("coupons")]
public class CouponEntity : BaseEntity
{
    [Required]
    [MaxLength(50)]
    [Column("code")]
    public string Code { get; set; } = string.Empty;

    [MaxLength(200)]
    [Column("description")]
    public string? Description { get; set; }

    [Column("discount_type")]
    public EDiscountType DiscountType { get; set; }

    [Column("discount_value")]
    public decimal DiscountValue { get; set; }

    [Column("min_order_value")]
    public decimal? MinOrderValue { get; set; }

    [Column("max_discount_value")]
    public decimal? MaxDiscountValue { get; set; }

    [Column("usage_limit")]
    public int? UsageLimit { get; set; }

    [Column("usage_count")]
    public int UsageCount { get; set; } = 0;

    [Column("start_date")]
    public DateTime? StartDate { get; set; }

    [Column("expiration_date")]
    public DateTime? ExpirationDate { get; set; }

    [Column("is_active")]
    public bool IsActive { get; set; } = true;

    [Column("first_order_only")]
    public bool FirstOrderOnly { get; set; } = false;
}

