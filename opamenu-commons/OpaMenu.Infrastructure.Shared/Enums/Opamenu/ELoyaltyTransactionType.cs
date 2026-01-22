using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpaMenu.Infrastructure.Shared.Enums.Opamenu
{
    public enum ELoyaltyTransactionType
    {
        Earn,
        Redeem,
        Adjustment,
        Expired
    }

    public static class LoyaltyTransactionTypeHelpers
    {
        public static string GetDescription(this ELoyaltyTransactionType type)
        {
            return type switch
            {
                ELoyaltyTransactionType.Earn => "Pontos Ganhos",
                ELoyaltyTransactionType.Redeem => "Pontos Resgatados",
                ELoyaltyTransactionType.Adjustment => "Ajuste de Pontos",
                ELoyaltyTransactionType.Expired => "Pontos Expirados",
            };
        }
    }
}
