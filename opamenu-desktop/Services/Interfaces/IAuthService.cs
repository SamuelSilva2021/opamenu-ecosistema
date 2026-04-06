using System.Threading.Tasks;

namespace OpaMenu.Desktop.Services.Interfaces;

public interface IAuthService
{
    /// <summary>
    /// Faz login na API de Autenticação (opamenu-authentication) e salva o token.
    /// Retorna verdadeiro se o login for bem-sucedido.
    /// </summary>
    Task<bool> LoginAsync(string email, string password);

    /// <summary>
    /// Retorna o Token JWT atual, se existir.
    /// </summary>
    string? GetCurrentToken();

    /// <summary>
    /// Desloga o usuário limpando o token local.
    /// </summary>
    void Logout();
}