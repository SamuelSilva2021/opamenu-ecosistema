using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpaMenu.Infrastructure.Shared.Enums.Opamenu;

namespace OpaMenu.Domain.DTOs.Order
{
    public class CreatePublicOrderRequestDto
    {
        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string CustomerName { get; set; } = string.Empty;

        [Required]
        [StringLength(20, MinimumLength = 10)]
        public string CustomerPhone { get; set; } = string.Empty;

        [EmailAddress]
        public string? CustomerEmail { get; set; }
        public AddressDto? DeliveryAddress { get; set; }

        public EOrderType OrderType { get; set; } = EOrderType.Delivery;

        public Guid? TableId { get; set; }

        [StringLength(1000)]
        public string? Notes { get; set; }

        [StringLength(50)]
        public string? CouponCode { get; set; }

        public int? LoyaltyPointsUsed { get; set; }

        public decimal? DeliveryFee { get; set; }

        [Required]
        [MinLength(1, ErrorMessage = "At least one item is required")]
        public List<CreateOrderItemRequestDto> Items { get; set; } = new();
    }
}

