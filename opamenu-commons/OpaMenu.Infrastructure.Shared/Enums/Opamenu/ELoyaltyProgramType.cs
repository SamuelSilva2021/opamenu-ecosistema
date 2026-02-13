namespace OpaMenu.Infrastructure.Shared.Enums.Opamenu
{
    public enum ELoyaltyProgramType
    {
        PointsPerValue = 0,
        OrderCount = 1,
        ItemCount = 2
    }

    public static class LoyaltyProgramTypeHelpers
    {
        public static string GetDescription(this ELoyaltyProgramType type)
        {
            return type switch
            {
                ELoyaltyProgramType.PointsPerValue => "Pontos por Valor Gasto",
                ELoyaltyProgramType.OrderCount => "Quantidade de Pedidos",
                ELoyaltyProgramType.ItemCount => "Quantidade de Itens Específicos",
                _ => "Desconhecido"
            };
        }
    }
}
