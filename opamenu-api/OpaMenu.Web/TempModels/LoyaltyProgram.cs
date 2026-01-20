using System;
using System.Collections.Generic;

namespace OpaMenu.Web.TempModels;

public class LoyaltyProgram
{
    public int Id { get; set; }
    
    // Identificador do Tenant dono do programa
    public string TenantId { get; set; } = null!; // Guid as string ou identificador único

    public string Name { get; set; } = null!;
    
    public string? Description { get; set; }
    
    // Regra: Pontos por unidade monetária (ex: 1.0 = 1 ponto por real)
    public decimal PointsPerCurrency { get; set; } = 1.0m;
    
    // Valor mínimo do pedido para pontuar
    public decimal MinOrderValue { get; set; } = 0m;
    
    // Validade dos pontos em dias (opcional, null = sem expiração)
    public int? PointsValidityDays { get; set; }
    
    public bool IsActive { get; set; } = true;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
