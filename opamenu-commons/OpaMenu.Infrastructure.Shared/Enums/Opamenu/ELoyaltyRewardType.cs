namespace OpaMenu.Infrastructure.Shared.Enums.Opamenu
{
    public enum ELoyaltyRewardType
    {
        DiscountPercentage = 0,
        DiscountValue = 1,
        FreeProduct = 2
    }

    public static class LoyaltyRewardTypeHelpers
    {
        public static string GetDescription(this ELoyaltyRewardType type)
        {
            return type switch
            {
                ELoyaltyRewardType.DiscountPercentage => "Desconto em Porcentagem",
                ELoyaltyRewardType.DiscountValue => "Desconto em Valor Fixo",
                ELoyaltyRewardType.FreeProduct => "Produto Grátis",
                _ => "Desconhecido"
            };
        }
    }
}
