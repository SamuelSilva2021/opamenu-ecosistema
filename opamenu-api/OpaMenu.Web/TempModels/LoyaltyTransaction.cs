using System;

namespace OpaMenu.Web.TempModels;

public enum LoyaltyTransactionType
{
    Earn = 1,
    Redeem = 2,
    Adjustment = 3,
    Expired = 4
}

public class LoyaltyTransaction
{
    public int Id { get; set; }
    
    public int CustomerLoyaltyBalanceId { get; set; }
    
    public int? OrderId { get; set; } // Opcional, link para o pedido que gerou/consumiu
    
    public int Points { get; set; } // Positivo para ganho, negativo para uso
    
    public LoyaltyTransactionType Type { get; set; }
    
    public string? Description { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime? ExpiresAt { get; set; } // Data de expiração desses pontos específicos
    
    public virtual CustomerLoyaltyBalance CustomerLoyaltyBalance { get; set; } = null!;
    
    // Navegação opcional para Order se necessário configurar no DbContext
    // public virtual Order? Order { get; set; } 
}
