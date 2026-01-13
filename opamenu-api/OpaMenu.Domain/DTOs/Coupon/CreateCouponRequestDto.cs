using System.ComponentModel.DataAnnotations;
using OpaMenu.Infrastructure.Shared.Enums;

namespace OpaMenu.Domain.DTOs.Coupon;

public class CreateCouponRequestDto
{
    [Required]
    [MaxLength(50)]
    public string Code { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? Description { get; set; }

    [Required]
    public EDiscountType EDiscountType { get; set; }

    [Required]
    public decimal DiscountValue { get; set; }

    public decimal? MinOrderValue { get; set; }
    public decimal? MaxDiscountValue { get; set; }
    public int? UsageLimit { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? ExpirationDate { get; set; }
    public bool FirstOrderOnly { get; set; }
}

