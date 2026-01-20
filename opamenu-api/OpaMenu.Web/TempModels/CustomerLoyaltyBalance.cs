using System;
using System.Collections.Generic;

namespace OpaMenu.Web.TempModels;

public class CustomerLoyaltyBalance
{
    public int Id { get; set; }
    
    public string TenantId { get; set; } = null!;
    
    // Identificador do cliente (vínculo com Order.CustomerPhone)
    public string CustomerPhone { get; set; } = null!;
    
    public string? CustomerName { get; set; } // Cache do nome para facilidade
    
    public int Balance { get; set; } = 0;
    
    public int TotalEarned { get; set; } = 0;
    
    public DateTime LastActivityAt { get; set; } = DateTime.UtcNow;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public virtual ICollection<LoyaltyTransaction> Transactions { get; set; } = new List<LoyaltyTransaction>();
}
