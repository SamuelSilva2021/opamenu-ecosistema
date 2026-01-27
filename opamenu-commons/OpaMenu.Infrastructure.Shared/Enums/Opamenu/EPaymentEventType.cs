using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpaMenu.Infrastructure.Shared.Enums.Opamenu
{
    public enum EPaymentEventType
    {
        Created,
        ProviderNotified,
        Confirmed,
        Failed,
        Refunded
    }
    public static class PaymentEventTypeHelper
    {
        public static string GetDescriptio(this EPaymentEventType type) =>
            type switch
            {
                EPaymentEventType.Created => "Criado",
                EPaymentEventType.ProviderNotified => "Provedor notificado",
                EPaymentEventType.Confirmed => "Confirmado",
                EPaymentEventType.Failed => "Falhou",
                EPaymentEventType.Refunded => "Estornado",
                _ => "Tipo de evento desconhecido"
            };
    }
}
