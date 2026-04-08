using OpaMenu.Desktop.Models.Enums;
using System.Text.Json.Serialization;

namespace OpaMenu.Desktop.Models.DTOs.Tables;

public class TabCheckoutRequestDto
{
    [JsonPropertyName("paymentMethod")]
    public EPaymentMethod PaymentMethod { get; set; }
}
