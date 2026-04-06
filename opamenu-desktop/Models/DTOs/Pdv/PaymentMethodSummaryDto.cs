using System.Text.Json.Serialization;
using OpaMenu.Desktop.Models.Enums;

namespace OpaMenu.Desktop.Models.DTOs.Pdv;

public class PaymentMethodSummaryDto
{
    [JsonPropertyName("paymentMethod")]
    public EPaymentMethod PaymentMethod { get; set; }

    [JsonPropertyName("paymentMethodName")]
    public string PaymentMethodName { get; set; } = string.Empty;

    [JsonPropertyName("totalAmount")]
    public decimal TotalAmount { get; set; }

    [JsonPropertyName("count")]
    public int Count { get; set; }
}
