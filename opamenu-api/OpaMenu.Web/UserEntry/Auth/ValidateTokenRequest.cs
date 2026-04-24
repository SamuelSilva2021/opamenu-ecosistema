namespace OpaMenu.Web.UserEntry.Auth;

/// <summary>
/// Request para validação de token
/// </summary>
public class ValidateTokenRequest
{
    public string Token { get; set; } = string.Empty;
}
