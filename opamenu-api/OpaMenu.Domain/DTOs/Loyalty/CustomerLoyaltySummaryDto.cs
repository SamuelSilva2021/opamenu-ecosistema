namespace OpaMenu.Domain.DTOs.Loyalty;

public class CustomerLoyaltySummaryDto
{
    public int Balance { get; set; }
    public int TotalEarned { get; set; }
    public LoyaltyProgramDto? Program { get; set; }
    public List<CustomerLoyaltyBalanceDto> Balances { get; set; } = new();
}
