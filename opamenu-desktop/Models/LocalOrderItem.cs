using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OpaMenu.Desktop.Models;

/// <summary>
/// Representa a tabela LocalOrderItems no SQLite.
/// Relaciona os produtos ao LocalOrder (Pedido offline).
/// </summary>
public class LocalOrderItem
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    public Guid LocalOrderId { get; set; }

    [ForeignKey(nameof(LocalOrderId))]
    public LocalOrder? LocalOrder { get; set; }

    [Required]
    public Guid ProductId { get; set; }

    [Required]
    public string ProductName { get; set; } = string.Empty;

    public int Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    public decimal TotalPrice { get; set; }
}