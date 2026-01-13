using OpaMenu.Infrastructure.Shared.Entities.AccessControl.UserAccounts;
using OpaMenu.Infrastructure.Shared.Entities.MultiTenant.Tenant;
using System.Security.Claims;

namespace Authenticator.API.Core.Application.Interfaces;

/// <summary>
/// Interface para o serviÃ§o de tokens JWT
/// </summary>
public interface IJwtTokenService
{
    /// <summary>
    /// Gera um token JWT para o usuário
    /// </summary>
    /// <param name="user">usuário autenticado</param>
    /// <param name="tenant">Tenant (opcional)</param>
    /// <param name="roles">Lista de roles do usuário</param>
    /// <returns>Token JWT gerado</returns>
    string GenerateAccessToken(UserAccountEntity user, TenantEntity? tenant, IList<string> roles);

    /// <summary>
    /// Gera um refresh token
    /// </summary>
    /// <returns>Refresh token gerado</returns>
    string GenerateRefreshToken();

    /// <summary>
    /// ObtÃ©m os claims de um token JWT vÃ¡lido
    /// </summary>
    /// <param name="token">Token JWT</param>
    /// <returns>Claims do token</returns>
    ClaimsPrincipal? GetPrincipalFromToken(string token);

    /// <summary>
    /// Valida um token JWT
    /// </summary>
    /// <param name="token">Token JWT</param>
    /// <returns>Se o token Ã© vÃ¡lido</returns>
    bool ValidateToken(string token);

    /// <summary>
    /// ObtÃ©m o tempo de expiraÃ§Ã£o configurado para tokens (em segundos)
    /// </summary>
    /// <returns>Tempo de expiraÃ§Ã£o em segundos</returns>
    int GetTokenExpirationTime();
}

