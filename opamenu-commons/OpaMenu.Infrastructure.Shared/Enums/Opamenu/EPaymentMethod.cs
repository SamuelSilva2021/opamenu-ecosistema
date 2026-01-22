using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpaMenu.Infrastructure.Shared.Enums.Opamenu
{
    public enum EPaymentMethod
    {
        CreditCard,
        DebitCard,
        Pix,
        Cash,
        BankTransfer,
        Ticket
    }
    public static class PaymentMethodHelper
    {
        public static string GetDescriptio(this EPaymentMethod method) =>
            method switch
            {
                EPaymentMethod.CreditCard => "Cartão de Crédito",
                EPaymentMethod.DebitCard => "Cartão de Débito",
                EPaymentMethod.Pix => "Pix",
                EPaymentMethod.Cash => "Dinheiro",
                EPaymentMethod.BankTransfer => "Transferência Bancária",
                EPaymentMethod.Ticket => "Boleto",
                _ => "Método Desconhecido"
            };
    }
}
