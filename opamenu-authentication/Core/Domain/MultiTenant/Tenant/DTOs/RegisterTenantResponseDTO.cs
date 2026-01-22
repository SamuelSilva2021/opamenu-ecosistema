namespace Authenticator.API.Core.Domain.MultiTenant.Tenant.DTOs;

/// <summary>
/// Resposta do cadastro de novo tenant
/// </summary>
public class RegisterTenantResponseDTO
{
    /// <summary>
    /// ID do tenant criado
    /// </summary>
    public Guid TenantId { get; set; }

    /// <summary>
    /// ID do usuário administrador criado
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Nome da empresa
    /// </summary>
    public string CompanyName { get; set; } = string.Empty;

    /// <summary>
    /// Slug gerado para o tenant
    /// </summary>
    public string Slug { get; set; } = string.Empty;

    /// <summary>
    /// Email do usuário administrador
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Nome completo do usuário administrador
    /// </summary>
    public string FullName { get; set; } = string.Empty;

    /// <summary>
    /// Token JWT para acesso imediato
    /// </summary>
    public string AccessToken { get; set; } = string.Empty;

    /// <summary>
    /// Token de refresh
    /// </summary>
    public string RefreshToken { get; set; } = string.Empty;

    /// <summary>
    /// Tempo de expiração do token (em segundos)
    /// </summary>
    public int ExpiresIn { get; set; }

    /// <summary>
    /// Data de criação
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Mensagem de sucesso
    /// </summary>
    public string Message { get; set; } = "Tenant cadastrado com sucesso!";

    /// <summary>
    /// Indica se o usuário deve ser redirecionado para seleção de plano
    /// </summary>
    public bool RedirectToPlanSelection { get; set; }
}