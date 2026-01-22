namespace OpaMenu.Infrastructure.Shared.Enums.Opamenu;

public enum EOrderType
{
    Delivery,
    Counter,
    Table
}

public static class OrderTypeHelper
{
    public static string GetDescription(this EOrderType orderType) =>
        orderType switch
        {
            EOrderType.Delivery => "Entrega",
            EOrderType.Counter => "Balcão",
            EOrderType.Table => "Mesa",
            _ => "Desconhecido"
        };
}

