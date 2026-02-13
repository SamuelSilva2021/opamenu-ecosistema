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

public class LoyaltyProgramFilterDto
{
    public Guid? ProductId { get; set; }
    public Guid? CategoryId { get; set; }
}

public class CustomerLoyaltySummaryDto
{
    public int Balance { get; set; }
    public int TotalEarned { get; set; }
    public LoyaltyProgramDto? Program { get; set; }
}

public class LoyaltyTransactionDto
{
    public Guid Id { get; set; }
    public int Points { get; set; }
    public string Type { get; set; } = null!;
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
}
