namespace OpaMenu.Desktop.Models.Enums;

/// <summary>
/// Representa o status de sincronização de um registro local.
/// </summary>
public enum ESyncStatus
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