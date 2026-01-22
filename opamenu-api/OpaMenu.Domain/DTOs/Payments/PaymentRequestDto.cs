using OpaMenu.Infrastructure.Shared.Enums.Opamenu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpaMenu.Domain.DTOs.Payments
{
    public class PaymentRequestDto
    {
        public Guid OrderId { get; set; }
        public decimal Amount { get; set; }
        public EPaymentMethod Method { get; set; }
        public string? CardToken { get; set; }
        public string? CardHolderName { get; set; }
        public string? Notes { get; set; }
    }
}
