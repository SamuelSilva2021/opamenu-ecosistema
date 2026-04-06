using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpaMenu.Domain.DTOs.CashRegister
{
    public class CashShiftSummaryResponseDto
    {
        public CashShiftResponseDto Shift { get; set; } = new();
        public decimal TotalSales { get; set; }
        public decimal TotalInflows { get; set; }
        public decimal TotalOutflows { get; set; }
        public List<PaymentMethodSummaryDto> SalesByPaymentMethod { get; set; } = new();
    }
}
