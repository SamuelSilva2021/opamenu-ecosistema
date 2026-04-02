using System;
using System.ComponentModel.DataAnnotations;

namespace OpaMenu.Desktop.Models;

/// <summary>
/// Representa a tabela LocalOrders no SQLite para funcionamento offline.
/// Guarda o JSON do payload do pedido até conseguir enviar para a API.
/// </summary>
public class LocalOrder : OfflineEntity
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// O ID do Restaurante / Tenant
    /// </summary>
    public int TenantId { get; set; }

    /// <summary>
    /// O tipo do pedido (Mesa, Balcão, Delivery)
    /// </summary>
    [Required]
    public string OrderType { get; set; } = string.Empty;

    /// <summary>
    /// Total do pedido no momento da criação local.
    /// </summary>
    public decimal TotalAmount { get; set; }

    /// <summary>
    /// O corpo do pedido em formato JSON (Payload que será enviado no POST para api/orders).
    /// </summary>
    [Required]
    public string PayloadJson { get; set; } = string.Empty;
}