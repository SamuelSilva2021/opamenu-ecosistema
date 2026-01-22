using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpaMenu.Infrastructure.Shared.Enums.Opamenu
{
    public enum EOrderStatus
    {
        Pending,
        Confirmed,
        Preparing,
        Ready,
        OutForDelivery,
        Delivered,
        Cancelled,
        Rejected
    }
    public static class EOrderStatusHelper
    {
        public static string GetDescription(this EOrderStatus status) =>
            status switch
            {
                EOrderStatus.Pending => "Pendente",
                EOrderStatus.Confirmed => "Confirmado",
                EOrderStatus.Preparing => "Preparando",
                EOrderStatus.Ready => "Pronto",
                EOrderStatus.OutForDelivery => "Saiu para Entrega",
                EOrderStatus.Delivered => "Entregue",
                EOrderStatus.Cancelled => "Cancelado",
                EOrderStatus.Rejected => "Rejeitado",
                _ => "Desconhecido"
            };
    }
}
