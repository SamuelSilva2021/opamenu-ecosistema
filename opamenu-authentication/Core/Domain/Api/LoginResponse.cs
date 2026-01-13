using OpaMenu.Infrastructure.Shared.Entities.MultiTenant.Subscription;

namespace Authenticator.API.Core.Domain.Api;

/// <summary>
/// Modelo para resposta de login
/// </summary>
public class LoginResponse
{
    /// <summary>
    /// Token JWT de acesso
    /// </summary>
    public string AccessToken { get; set; } = string.Empty;

    /// <summary>
    /// Token de renovaÃ§Ã£o
    /// </summary>
    public string RefreshToken { get; set; } = string.Empty;

    /// <summary>
    /// Tipo do token (normalmente "Bearer")
    /// </summary>
    public string TokenType { get; set; } = "Bearer";

    /// <summary>
    /// Tempo de expiraÃ§Ã£o do token em segundos
    /// </summary>
    public int ExpiresIn { get; set; }

    /// <summary>
    /// Status atual do tenant
    /// </summary>
    public string? TenantStatus { get; set; }

    /// <summary>
    /// Status da assinatura
    /// </summary>
    public ESubscriptionStatus SubscriptionStatus { get; set; }

    /// <summary>
    /// Indica se Ã© necessÃ¡rio realizar pagamento/assinatura
    /// </summary>
    public bool RequiresPayment { get; set; }
}
