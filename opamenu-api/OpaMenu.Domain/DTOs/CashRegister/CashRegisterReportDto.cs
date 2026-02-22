namespace OpaMenu.Domain.DTOs.CashRegister;

public class CashRegisterReportDto
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal TotalGrossSales { get; set; }
    public decimal TotalNetSales { get; set; }
    public decimal TotalInflows { get; set; }
    public decimal TotalOutflows { get; set; }
    public decimal TotalDiscrepancy { get; set; }
    public List<PaymentMethodSummaryDto> SalesByPaymentMethod { get; set; } = new();
}
