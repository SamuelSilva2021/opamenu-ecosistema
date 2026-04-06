using System.Text.Json.Serialization;

namespace OpaMenu.Desktop.Models.DTOs.AccessControl;

/// <summary>
/// Mapeamento da propriedade 'data' da resposta de Login da API de Autenticação.
/// </summary>
public class LoginResponseDto
{
    [JsonPropertyName("accessToken")]
    public string AccessToken { get; set; } = string.Empty;

    [JsonPropertyName("refreshToken")]
    public string RefreshToken { get; set; } = string.Empty;

    [JsonPropertyName("tokenType")]
    public string TokenType { get; set; } = string.Empty;

    [JsonPropertyName("expiresIn")]
    public int ExpiresIn { get; set; }

    [JsonPropertyName("tenantStatus")]
    public string? TenantStatus { get; set; }

    [JsonPropertyName("subscriptionStatus")]
    public string? SubscriptionStatus { get; set; }

    [JsonPropertyName("requiresPayment")]
    public bool RequiresPayment { get; set; }

    [JsonPropertyName("redirectToPlanSelection")]
    public bool RedirectToPlanSelection { get; set; }
}