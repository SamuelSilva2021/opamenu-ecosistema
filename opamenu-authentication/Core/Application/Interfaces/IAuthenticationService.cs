using Authenticator.API.Core.Domain.Api;

namespace Authenticator.API.Core.Application.Interfaces;

/// <summary>
/// Interface para o serviço de autenticação
/// </summary>
public interface IAuthenticationService
{
    /// <summary>
    /// Autentica um usuário com username/email e senha
    /// </summary>
    /// <param name="usernameOrEmail">Username ou email do usuário</param>
    /// <param name="password">Senha do usuário</param>
    /// <returns>Resposta de login com tokens e informações do usuário</returns>
    Task<ResponseDTO<LoginResponse>> LoginAsync(string usernameOrEmail, string password);

    /// <summary>
    /// Renova o token de acesso usando o refresh token
    /// </summary>
    /// <param name="refreshToken">Token de renovação</param>
    /// <returns>Nova resposta de login com tokens atualizados</returns>
    Task<ResponseDTO<LoginResponse>> RefreshTokenAsync(string refreshToken);

    /// <summary>
    /// Invalida o refresh token (logout)
    /// </summary>
    /// <param name="refreshToken">Token de renovação a ser invalidado</param>
    /// <returns>Resultado da operação</returns>
    Task<ResponseDTO<bool>> RevokeTokenAsync(string refreshToken);

    /// <summary>
    /// Obtém informações do usuário pelo token JWT
    /// </summary>
    /// <param name="userId">ID do usuário</param>
    /// <param name="tenantSlug">Slug do tenant (opcional)</param>
    /// <returns>Informações do usuário</returns>
    Task<ResponseDTO<UserInfo>> GetUserInfoAsync(Guid userId, string? tenantSlug = null);

    /// <summary>
    /// Valida se o token JWT é válido
    /// </summary>
    /// <param name="token">Token JWT</param>
    /// <returns>Se o token é válido</returns>
    Task<bool> ValidateTokenAsync(string token);
}