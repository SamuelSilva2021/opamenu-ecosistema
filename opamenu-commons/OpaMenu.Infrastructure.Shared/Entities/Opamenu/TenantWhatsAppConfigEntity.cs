using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using OpaMenu.Infrastructure.Shared.Enums.Opamenu;

namespace OpaMenu.Infrastructure.Shared.Entities.Opamenu;

/// <summary>
/// Configuração de integração com WhatsApp para o Tenant
/// </summary>
public class TenantWhatsAppConfigEntity : BaseEntity
{
    [Required]
    public EWhatsAppProvider Provider { get; set; } = EWhatsAppProvider.EvolutionApi;

    /// <summary>
    /// ID da instância no provedor (Ex: Nome da instância na Evolution API)
    /// </summary>
    [MaxLength(100)]
    public string InstanceId { get; set; } = string.Empty;

    /// <summary>
    /// Token de autenticação/API Key para a instância
    /// </summary>
    [MaxLength(500)]
    public string ApiKey { get; set; } = string.Empty;

    /// <summary>
    /// URL base da API do provedor (caso não seja a global)
    /// </summary>
    [MaxLength(500)]
    public string? BaseUrl { get; set; }

    /// <summary>
    /// Se o bot de boas-vindas está ativado para este tenant
    /// </summary>
    public bool WelcomeBotEnabled { get; set; } = false;

    /// <summary>
    /// Se a consulta de status automática está ativada
    /// </summary>
    public bool OrderStatusLookupEnabled { get; set; } = true;

    /// <summary>
    /// Mensagem personalizada de boas-vindas
    /// </summary>
    [MaxLength(1000)]
    public string? WelcomeMessage { get; set; }

    /// <summary>
    /// Ativo ou Inativo
    /// </summary>
    public bool IsActive { get; set; } = true;
}
