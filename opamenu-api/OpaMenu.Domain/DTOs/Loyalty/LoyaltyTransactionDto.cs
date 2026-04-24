namespace OpaMenu.Domain.DTOs.Loyalty;

public class LoyaltyTransactionDto
{
    public Guid Id { get; set; }
    public int Points { get; set; }
    public string Type { get; set; } = null!;
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
}
