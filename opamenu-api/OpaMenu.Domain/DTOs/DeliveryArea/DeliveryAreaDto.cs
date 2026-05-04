namespace OpaMenu.Domain.DTOs.DeliveryArea;

public class DeliveryAreaResponseDto
{
    public Guid Id { get; set; }
    public string City { get; set; } = string.Empty;
    public string? Neighborhood { get; set; }
    public decimal Fee { get; set; }
    public bool IsActive { get; set; }
}

public class CreateDeliveryAreaRequestDto
{
    public string City { get; set; } = string.Empty;
    public string? Neighborhood { get; set; }
    public decimal Fee { get; set; }
    public bool IsActive { get; set; } = true;
}
