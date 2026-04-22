namespace OpaMenu.Domain.DTOs.Auth;

public sealed class LoginResponseDto
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public string TokenType { get; set; } = "Bearer";
    public int ExpiresIn { get; set; }
    public string? TenantStatus { get; set; }
    public string? SubscriptionStatus { get; set; }
    public bool? RequiresPayment { get; set; }
    public bool? RedirectToPlanSelection { get; set; }
}

