namespace OpaMenu.Domain.DTOs.Coupon;

public class ValidateCouponRequestDto
{
    public string Code { get; set; } = string.Empty;
    public decimal OrderValue { get; set; }
}
