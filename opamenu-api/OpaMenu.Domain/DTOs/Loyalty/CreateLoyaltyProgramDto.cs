using OpaMenu.Infrastructure.Shared.Enums.Opamenu;

namespace OpaMenu.Domain.DTOs.Loyalty;

public class CreateLoyaltyProgramDto
{
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public decimal PointsPerCurrency { get; set; } = 1.0m;
    public decimal CurrencyValue { get; set; } = 1.0m;
    public decimal MinOrderValue { get; set; } = 0m;
    public int? PointsValidityDays { get; set; }
    public bool IsActive { get; set; } = true;
    public ELoyaltyProgramType Type { get; set; } = ELoyaltyProgramType.PointsPerValue;
    public int? TargetCount { get; set; }
    public ELoyaltyRewardType? RewardType { get; set; }
    public decimal? RewardValue { get; set; }
    public List<LoyaltyProgramFilterDto> Filters { get; set; } = new();
}
