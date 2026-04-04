using System.Text.Json.Serialization;

namespace OpaMenu.Desktop.Models.DTOs.Requests;

public class CloseCashShiftRequestDto
{
    [JsonPropertyName("closingBalance")]
    public decimal ClosingBalance { get; set; }
}
