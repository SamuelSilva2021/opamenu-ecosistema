using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace OpaMenu.Desktop.Models.DTOs.Pdv;

public class CashShiftSummaryDto
{
    [JsonPropertyName("shift")]
    public CashShiftDto Shift { get; set; } = new();

    [JsonPropertyName("totalSales")]
    public decimal TotalSales { get; set; }

    [JsonPropertyName("totalInflows")]
    public decimal TotalInflows { get; set; }

    [JsonPropertyName("totalOutflows")]
    public decimal TotalOutflows { get; set; }

    [JsonPropertyName("salesByPaymentMethod")]
    public List<PaymentMethodSummaryDto> SalesByPaymentMethod { get; set; } = new();
}

public class CashShiftCloseSummaryDto : CashShiftSummaryDto
{
    [JsonPropertyName("closingBalance")]
    public decimal ClosingBalance { get; set; }

    [JsonPropertyName("expectedCashBalance")]
    public decimal ExpectedCashBalance { get; set; }

    [JsonPropertyName("discrepancy")]
    public decimal Discrepancy { get; set; }

    [JsonPropertyName("discrepancyJustification")]
    public string? DiscrepancyJustification { get; set; }
}
