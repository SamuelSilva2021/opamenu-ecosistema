using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpaMenu.Infrastructure.Shared.Enums.Opamenu
{
    public enum EPaymentStatus
    {
        Pending = 0,
        Processing = 1,
        Approved = 2,
        Declined = 3,
        Cancelled = 4,
        Refunded = 5
    }
    public static class PaymentStatusHelper
    {
        public static string GetDescription(this EPaymentStatus status)
        {
            return status switch
            {
                EPaymentStatus.Pending => "Pendente",
                EPaymentStatus.Processing => "Em Processamento",
                EPaymentStatus.Approved => "Aprovado",
                EPaymentStatus.Declined => "Recusado",
                EPaymentStatus.Cancelled => "Cancelado",
                EPaymentStatus.Refunded => "Estornado",
                _ => "Unknown"
            };
        }

    }
}
