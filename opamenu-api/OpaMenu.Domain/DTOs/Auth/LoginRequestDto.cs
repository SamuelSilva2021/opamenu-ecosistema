namespace OpaMenu.Domain.DTOs.Auth;

public sealed class LoginRequestDto
{
    public string UsernameOrEmail { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

