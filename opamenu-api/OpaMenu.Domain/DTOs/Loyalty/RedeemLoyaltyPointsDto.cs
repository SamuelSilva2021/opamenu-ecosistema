namespace OpaMenu.Domain.DTOs.Loyalty;

public class RedeemLoyaltyPointsDto
{
    public Guid ProgramId { get; set; }
    public string CustomerPhone { get; set; } = null!;
    public int Points { get; set; }
    public Guid? OrderId { get; set; }
}
