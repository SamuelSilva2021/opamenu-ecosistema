using System;
using System.Text.Json.Serialization;

namespace OpaMenu.Desktop.Models.DTOs.Tables;

public class OrderItemAditionalDto
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    [JsonPropertyName("aditionalId")]
    public Guid AditionalId { get; set; }

    [JsonPropertyName("aditionalName")]
    public string AditionalName { get; set; } = string.Empty;

    [JsonPropertyName("unitPrice")]
    public decimal UnitPrice { get; set; }

    [JsonPropertyName("quantity")]
    public int Quantity { get; set; }

    [JsonPropertyName("subtotal")]
    public decimal Subtotal { get; set; }
}
