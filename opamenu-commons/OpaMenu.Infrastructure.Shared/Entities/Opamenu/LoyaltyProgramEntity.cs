using OpaMenu.Infrastructure.Shared.Enums.Opamenu;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OpaMenu.Infrastructure.Shared.Entities.Opamenu;

[Table("loyalty_programs")]
public class LoyaltyProgramEntity : BaseEntity
{
    [Required]
    [MaxLength(100)]
    [Column("name")]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500)]
    [Column("description")]
    public string? Description { get; set; }

    [Required]
    [Column("points_per_currency", TypeName = "decimal(10,2)")]
    public decimal PointsPerCurrency { get; set; } = 1.0m;
    [Required]
    [Column("currency_value", TypeName = "decimal(10,2)")]
    public decimal CurrencyValue { get; set; } = 1.0m;

    [Required]
    [Column("min_order_value", TypeName = "decimal(10,2)")]
    public decimal MinOrderValue { get; set; } = 0m;

    [Column("points_validity_days")]
    public int? PointsValidityDays { get; set; }

    [Required]
    [Column("is_active")]
    public bool IsActive { get; set; } = true;

    [Required]
    [Column("type")]
    public ELoyaltyProgramType Type { get; set; } = ELoyaltyProgramType.PointsPerValue;

    [Column("target_count")]
    public int? TargetCount { get; set; }

    [Column("reward_type")]
    public ELoyaltyRewardType? RewardType { get; set; }

    [Column("reward_value", TypeName = "decimal(10,2)")]
    public decimal? RewardValue { get; set; }

    // Navigation properties
    public virtual ICollection<LoyaltyProgramFilterEntity> Filters { get; set; } = new List<LoyaltyProgramFilterEntity>();
}
