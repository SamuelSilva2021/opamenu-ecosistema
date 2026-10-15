using System.Text.Json.Serialization;

namespace Opamenu.Desktop.Models.DTOs.Requests;

public class CloseCashShiftRequestDto
{
    [JsonPropertyName("closingBalance")]
    public decimal ClosingBalance { get; set; }

    [JsonPropertyName("discrepancyJustification")]
    public string? DiscrepancyJustification { get; set; }
}
