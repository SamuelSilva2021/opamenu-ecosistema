using OpaMenu.Infrastructure.Shared.Enums.Opamenu;

namespace OpaMenu.Domain.DTOs.Loyalty;

public class LoyaltyProgramDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public decimal PointsPerCurrency { get; set; }
    public decimal CurrencyValue { get; set; }
    public decimal MinOrderValue { get; set; }
    public int? PointsValidityDays { get; set; }
    public bool IsActive { get; set; }
    public ELoyaltyProgramType Type { get; set; }
    public int? TargetCount { get; set; }
    public ELoyaltyRewardType? RewardType { get; set; }
    public decimal? RewardValue { get; set; }
    public List<LoyaltyProgramFilterDto> Filters { get; set; } = new();
}
