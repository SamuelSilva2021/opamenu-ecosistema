using Opamenu.Desktop.Models.Enums;
using System.Text.Json.Serialization;

namespace Opamenu.Desktop.Models.DTOs.Tables;

public class TabCheckoutRequestDto
{
    [JsonPropertyName("paymentMethod")]
    public EPaymentMethod PaymentMethod { get; set; }
}
