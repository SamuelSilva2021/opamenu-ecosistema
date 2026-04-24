namespace OpaMenu.Domain.DTOs.Loyalty;

public class CustomerLoyaltyBalanceDto
{
    public Guid? ProgramId { get; set; }
    public string ProgramName { get; set; } = string.Empty;
    public int Balance { get; set; }
}
