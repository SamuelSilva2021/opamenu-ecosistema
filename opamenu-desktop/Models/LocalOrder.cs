using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OpaMenu.Desktop.Models;

/// <summary>
/// Representa a tabela LocalOrders no SQLite para funcionamento offline.
/// Guarda os dados básicos do pedido.
/// </summary>
public class LocalOrder : OfflineEntity
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// O ID do Restaurante / Tenant
    /// </summary>
    public Guid TenantId { get; set; }

    /// <summary>
    /// O tipo do pedido (Mesa, Balcão, Delivery)
    /// </summary>
    [Required]
    public string OrderType { get; set; } = string.Empty;

    public string? CustomerName { get; set; }
    public string? CustomerDocument { get; set; }
    public string? TableNumber { get; set; }
    public string? OrderObservation { get; set; }

    /// <summary>
    /// Total do pedido no momento da criação local.
    /// </summary>
    public decimal TotalAmount { get; set; }

    public decimal AmountPaid { get; set; }
    public decimal ChangeAmount { get; set; }
    public string? PaymentMethod { get; set; }

    /// <summary>
    /// O corpo do pedido em formato JSON (Payload que será enviado no POST para api/orders).
    /// Guardar o JSON garante que a estrutura exata do pedido seja enviada mesmo se o DTO mudar localmente.
    /// </summary>
    [Required]
    public string PayloadJson { get; set; } = string.Empty;
}