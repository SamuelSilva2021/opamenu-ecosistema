using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpaMenu.Infrastructure.Shared.Enums.Opamenu
{
    public enum EPaymentProvider
    {
        MercadoPago,
        PagarMe,
        Gerencianet,
        None
    }
    public static class EPaymentProviderHelper
    {
        public static string GetDescription(this EPaymentProvider provider) =>
            provider switch 
            { 
                EPaymentProvider.MercadoPago => "Mercado Pago",
                EPaymentProvider.PagarMe => "Pagar me",
                EPaymentProvider.Gerencianet => "Gerencia net",
                EPaymentProvider.None => "None",
                _ => "Sem provedor de pagamento"
            };
    }
}
