using System;
using System.Text.Json.Serialization;
using OpaMenu.Desktop.Models.Enums;

namespace OpaMenu.Desktop.Models.DTOs.Pdv;

public class CashMovementDto
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    [JsonPropertyName("type")]
    public ECashMovementType Type { get; set; }

    [JsonPropertyName("amount")]
    public decimal Amount { get; set; }

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("paymentMethod")]
    public EPaymentMethod? PaymentMethod { get; set; }

    [JsonPropertyName("orderId")]
    public Guid? OrderId { get; set; }

    [JsonPropertyName("orderNumber")]
    public int? OrderNumber { get; set; }

    [JsonPropertyName("createdAt")]
    public DateTime CreatedAt { get; set; }
}
