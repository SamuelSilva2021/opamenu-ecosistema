using System.ComponentModel.DataAnnotations;

namespace Authenticator.API.Core.Domain.Api;

/// <summary>
/// Modelo para requisição de login
/// </summary>
public class LoginRequest
{
    /// <summary>
    /// Nome de usuário ou email
    /// </summary>
    [Required(ErrorMessage = "Username ou email é obrigatório")]
    public string UsernameOrEmail { get; set; } = string.Empty;

    /// <summary>
    /// Senha do usuário
    /// </summary>
    [Required(ErrorMessage = "Senha é obrigatória")]
    public string Password { get; set; } = string.Empty;
}