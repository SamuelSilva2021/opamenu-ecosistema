using OpaMenu.Infrastructure.Shared.Enums.Opamenu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpaMenu.Domain.DTOs.Payments
{
    /// <summary>
    /// DTOs para respostas de pagamento
    /// </summary>
    public class PaymentResponseDto
    {
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }
        public decimal Amount { get; set; }
        public EPaymentMethod Method { get; set; }
        public EPaymentStatus Status { get; set; }
        public string? GatewayTransactionId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ProcessedAt { get; set; }
        public string? Notes { get; set; }
    }
}
