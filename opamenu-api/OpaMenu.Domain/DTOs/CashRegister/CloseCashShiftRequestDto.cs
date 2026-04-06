namespace OpaMenu.Domain.DTOs.CashRegister;

public class CloseCashShiftRequestDto
{
    public decimal ClosingBalance { get; set; }

    public string? DiscrepancyJustification { get; set; }
}
