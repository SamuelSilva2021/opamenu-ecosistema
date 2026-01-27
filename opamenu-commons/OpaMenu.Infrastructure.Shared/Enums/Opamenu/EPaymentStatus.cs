using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpaMenu.Infrastructure.Shared.Enums.Opamenu
{
    public enum EPaymentStatus
    {
        Pending,        // Criado, aguardando pagamento
        Authorized,     // (futuro cartão)
        Paid,           // Confirmado via webhook
        Expired,        // QR Code expirou
        Failed,         // Erro no provedor
        Refunded,       // Estornado
        Cancelled       // Pedido cancelado
    }
    public static class PaymentStatusHelper
    {
        public static string GetDescription(this EPaymentStatus status)
        {
            return status switch
            {
                EPaymentStatus.Pending => "Pendente",
                EPaymentStatus.Authorized => "Autorizado",
                EPaymentStatus.Paid => "Pago",
                EPaymentStatus.Expired => "Expirado",
                EPaymentStatus.Failed => "Falhou",
                EPaymentStatus.Refunded => "Estornado",
                EPaymentStatus.Cancelled => "Cancelado",
                _ => "Unknown"
            };
        }

    }
}
