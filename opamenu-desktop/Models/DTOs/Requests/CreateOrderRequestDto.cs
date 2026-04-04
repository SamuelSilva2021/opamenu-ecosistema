using OpaMenu.Desktop.Models.Enums;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace OpaMenu.Desktop.Models.DTOs.Requests;

public class CreateOrderRequestDto
{
    [JsonPropertyName("customerName")]
    public string? CustomerName { get; set; }

    [JsonPropertyName("customerPhone")]
    public string? CustomerPhone { get; set; }

    [JsonPropertyName("customerEmail")]
    public string? CustomerEmail { get; set; }

    [JsonPropertyName("deliveryAddress")]
    public AddressDto? DeliveryAddress { get; set; }

    [JsonPropertyName("isDelivery")]
    public bool IsDelivery { get; set; }

    [JsonPropertyName("orderType")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public EOrderType OrderType { get; set; } = EOrderType.Counter;

    [JsonPropertyName("tableId")]
    public string? TableId { get; set; }

    [JsonPropertyName("notes")]
    public string? Notes { get; set; }

    [JsonPropertyName("couponCode")]
    public string? CouponCode { get; set; }

    [JsonPropertyName("loyaltyPointsUsed")]
    public int? LoyaltyPointsUsed { get; set; }

    [JsonPropertyName("loyaltyProgramId")]
    public Guid? LoyaltyProgramId { get; set; }

    [JsonPropertyName("loyaltyDiscount")]
    public decimal? LoyaltyDiscount { get; set; }

    [JsonPropertyName("deliveryFee")]
    public decimal? DeliveryFee { get; set; }

    [JsonPropertyName("items")]
    public List<CreateOrderItemRequestDto> Items { get; set; } = new();

    [JsonPropertyName("paymentMethod")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public EPaymentMethod? PaymentMethod { get; set; }
}