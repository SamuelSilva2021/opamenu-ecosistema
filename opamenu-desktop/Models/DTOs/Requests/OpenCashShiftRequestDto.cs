using System.Text.Json.Serialization;

namespace Opamenu.Desktop.Models.DTOs.Requests;

public class OpenCashShiftRequestDto
{
    [JsonPropertyName("openingBalance")]
    public decimal OpeningBalance { get; set; }
}
