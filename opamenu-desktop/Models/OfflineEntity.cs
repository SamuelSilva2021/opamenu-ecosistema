using System;

namespace OpaMenu.Desktop.Models;

/// <summary>
/// Representa o status de sincronização de um registro local.
/// Semelhante à estratégia usada no App Flutter.
/// </summary>
public enum SyncStatus
{
    /// <summary>
    /// Criado/Alterado localmente e precisa ser enviado para a nuvem.
    /// </summary>
    PendingSync = 0,

    /// <summary>
    /// Sincronizado com a nuvem (O CloudId foi preenchido).
    /// </summary>
    Synced = 1,

    /// <summary>
    /// Ocorreu um erro ao tentar sincronizar.
    /// </summary>
    Error = 2
}

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
    public int? CloudId { get; set; }

    /// <summary>
    /// Status atual da sincronização deste registro.
    /// </summary>
    public SyncStatus SyncStatus { get; set; } = SyncStatus.PendingSync;

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
}