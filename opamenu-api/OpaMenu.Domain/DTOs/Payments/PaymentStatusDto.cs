using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpaMenu.Domain.DTOs.Payments
{
    public class PaymentStatusDto
    {
        public Guid PaymentId { get; set; }
        public string Status { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Method { get; set; } = string.Empty;
        public DateTime? ProcessedAt { get; set; }
        public string? GatewayTransactionId { get; set; }
    }
}
