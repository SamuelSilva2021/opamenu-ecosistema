using System.Text.Json.Serialization;

namespace OpaMenu.Desktop.Models.DTOs.Requests;

public class AddCashMovementRequestDto
{
    [JsonPropertyName("type")]
    public int Type { get; set; }

    [JsonPropertyName("amount")]
    public decimal Amount { get; set; }

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("paymentMethod")]
    public int? PaymentMethod { get; set; }
}
