using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpaMenu.Domain.DTOs.CashRegister
{
    public class CashShiftCloseSummaryResponseDto : CashShiftSummaryResponseDto
    {
        public decimal ClosingBalance { get; set; }
        public decimal ExpectedCashBalance { get; set; }
        public decimal Discrepancy { get; set; }
        public string? DiscrepancyJustification { get; set; }
    }
}
