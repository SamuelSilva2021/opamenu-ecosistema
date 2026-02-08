using System.ComponentModel.DataAnnotations;
using OpaMenu.Infrastructure.Shared.Enums.Opamenu;
using System.Text.Json.Serialization;

namespace OpaMenu.Domain.DTOs
{
    /// <summary>
    /// DTO para criaÃ§Ã£o de pedidos
    /// </summary>
    public class CreateOrderRequestDto
    {
        [StringLength(100, MinimumLength = 2)]
        public string? CustomerName { get; set; }

        [StringLength(20, MinimumLength = 10)]
        public string? CustomerPhone { get; set; }

        [EmailAddress]
        public string? CustomerEmail { get; set; }

        public AddressDto? DeliveryAddress { get; set; }

        public bool IsDelivery { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public EOrderType OrderType { get; set; } = EOrderType.Delivery;

        public Guid? TableId { get; set; }

        [StringLength(1000)]
        public string? Notes { get; set; }

        [StringLength(50)]
        public string? CouponCode { get; set; }

        public decimal? DeliveryFee { get; set; }

        [Required]
        [MinLength(1, ErrorMessage = "At least one item is required")]
        public List<CreateOrderItemRequestDto> Items { get; set; } = new();
    }
}

