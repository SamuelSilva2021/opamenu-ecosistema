using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpaMenu.Domain.DTOs.Payments
{
    public class RefundResponseDto
    {
        public Guid RefundId { get; set; }
        public Guid PaymentId { get; set; }
        public decimal Amount { get; set; }
        public string Reason { get; set; } = string.Empty;
        public DateTime RefundedAt { get; set; }
        public string RefundedBy { get; set; } = string.Empty;
        public string? GatewayRefundId { get; set; }
    }
}
