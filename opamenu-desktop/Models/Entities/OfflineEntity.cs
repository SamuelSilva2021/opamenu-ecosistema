using System;
using OpaMenu.Desktop.Models.Enums;

namespace OpaMenu.Desktop.Models.Entities;

/// <summary>
/// Modelo base para tabelas que precisam de suporte Offline-First.
/// </summary>
public abstract class OfflineEntity
{
    /// <summary>
    /// ID Local (Gerado via Guid na máquina para garantir unicidade offline).
    /// </summary>
    public Guid LocalId { get; set; } = Guid.NewGuid();

    /// <summary>
    /// ID retornado pela API da Nuvem (opamenu-api).
    /// Ficará nulo até que o registro seja sincronizado.
    /// </summary>
    public Guid? CloudId { get; set; }

    /// <summary>
    /// Status atual da sincronização deste registro.
    /// </summary>
    public ESyncStatus SyncStatus { get; set; } = ESyncStatus.PendingSync;

    /// <summary>
    /// Última vez que tentou sincronizar.
    /// </summary>
    public DateTime? LastSyncAttempt { get; set; }

    /// <summary>
    /// Mensagem de erro caso o SyncStatus seja Error.
    /// </summary>
    public string? SyncErrorMessage { get; set; }

    /// <summary>
    /// Data de criação local.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}