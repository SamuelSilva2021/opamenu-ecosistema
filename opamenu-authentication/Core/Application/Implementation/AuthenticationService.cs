using OpaMenu.Infrastructure.Shared.Entities.AccessControl.UserAccounts.Enum;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using BCrypt.Net;
using Authenticator.API.Core.Domain.Api;
using OpaMenu.Infrastructure.Shared.Entities.MultiTenant.Tenant;
using Authenticator.API.Core.Application.Interfaces;
using Authenticator.API.Infrastructure.Data;
using Authenticator.API.Core.Domain.MultiTenant.Tenant.DTOs;
using Authenticator.API.Infrastructure.Data.Context;
using Authenticator.API.Core.Application.Interfaces.AccessControl.Module;
using Authenticator.API.Core.Application.Interfaces.Auth;
using OpaMenu.Infrastructure.Shared.Entities.AccessControl;
using Authenticator.API.Core.Domain.AccessControl.Modules.DTOs;
using Authenticator.API.Core.Domain.AccessControl.UserAccounts.DTOs;
using OpaMenu.Infrastructure.Shared.Entities.MultiTenant.Subscription;
using Authenticator.API.Core.Application.Interfaces.MultiTenant;

namespace Authenticator.API.Core.Application.Implementation;

/// <summary>
/// ImplementaÃ§Ã£o do serviÃ§o de autenticaÃ§Ã£o
/// </summary>
public class AuthenticationService(
    AccessControlDbContext accessControlContext,
    MultiTenantDbContext multiTenantContext,
    IJwtTokenService jwtTokenService,
    IMemoryCache cache,
    ILogger<AuthenticationService> logger,
    IUserAccountsRepository userAccountsRepository,
    IModuleRepository moduleRepository,
    ISubscriptionRepository subscriptionRepository,
    IUserValidation loginValidation
        ) : IAuthenticationService
{
    private readonly AccessControlDbContext _accessControlContext = accessControlContext;
    private readonly MultiTenantDbContext _multiTenantContext = multiTenantContext;
    private readonly IJwtTokenService _jwtTokenService = jwtTokenService;
    private readonly IMemoryCache _cache = cache;
    private readonly ILogger<AuthenticationService> _logger = logger;
    private readonly IUserAccountsRepository _userAccountsRepository = userAccountsRepository;
    private readonly IModuleRepository _moduleRepository = moduleRepository;
    private readonly ISubscriptionRepository _subscriptionRepository = subscriptionRepository;
    private readonly IUserValidation _loginValidation = loginValidation;

    private const string REFRESH_TOKENS_CACHE_KEY = "refresh_tokens";
    private const string USER_CACHE_KEY_PREFIX = "user_";

    /// <summary>
    /// Autentica um usuário com username/email e senha
    /// </summary>
    /// <param name="usernameOrEmail"></param>
    /// <param name="password"></param>
    /// <returns></returns>
    public async Task<ResponseDTO<LoginResponse>> LoginAsync(string usernameOrEmail, string password)
    {
        try
        {
            _logger.LogInformation("Tentativa de login para: {UsernameOrEmail}", usernameOrEmail);
            var user = await _userAccountsRepository.FirstOrDefaultAsync(u =>
                (u.Username.Equals(usernameOrEmail) || u.Email.Equals(usernameOrEmail)) && u.DeletedAt == null);


            if (user == null)
            {
                _logger.LogWarning("Usuário não encontrado: {UsernameOrEmail}", usernameOrEmail);
                return ResponseBuilder<LoginResponse>
                    .Fail(new ErrorDTO { Message = "Credenciais inválidas" }).WithCode(401).Build();
            }

            var validationResult =  _loginValidation.LoginValidation(user, password);
            if (validationResult.Any())
                return ResponseBuilder<LoginResponse>.Fail(validationResult.ToArray()).WithCode(401).Build();

            var tenant = _multiTenantContext.Tenants.FirstOrDefault(t => t.Id == user.TenantId);

            var accessGroups = await GetUserAccessGroupsAsync(user.Id);
            var roles = await GetUserRolesAsync(accessGroups);
            var permissions = await GetUserPermissionsAsync(roles);

            var accessToken = _jwtTokenService.GenerateAccessToken(user, tenant, roles);
            var refreshToken = _jwtTokenService.GenerateRefreshToken();

            await StoreRefreshTokenAsync(refreshToken, user.Id, tenant?.Id);

            user.LastLoginAt = DateTime.UtcNow;
            user.UpdatedAt = DateTime.UtcNow;
            await _accessControlContext.SaveChangesAsync();

            // Verificar assinatura
            SubscriptionEntity? subscription = null;
            if (tenant != null)
                subscription = await _subscriptionRepository.GetActiveByTenantIdAsync(tenant.Id);

            var loginResponse = new LoginResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiresIn = _jwtTokenService.GetTokenExpirationTime(),
                TenantStatus = tenant?.Status.ToString(),
                //SubscriptionStatus = subscription!.Status,
                //// Requer pagamento se status for pendente ou suspenso, ou se assinatura nÃ£o estiver ativa/trial
                //RequiresPayment = tenant?.Status == ETenantStatus.Pendente || 
                //                  tenant?.Status == ETenantStatus.Suspenso ||
                //                  (subscription != null && subscription.Status != ESubscriptionStatus.Ativo && subscription.Status != ESubscriptionStatus.Trial)
            };

            _logger.LogInformation("Login bem-sucedido parausuário: {UserId}", user.Id);
            return ResponseBuilder<LoginResponse>.Ok(loginResponse).Build();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro durante o login para: {UsernameOrEmail}", usernameOrEmail);
            return ResponseBuilder<LoginResponse>
                .Fail(new ErrorDTO { Message = "Erro interno do servidor" }).WithCode(500).Build();
        }
    }

    public async Task<ResponseDTO<LoginResponse>> LoginClient(string usernameOrEmail, string password)  
    {
        try
        {
            _logger.LogInformation("Tentativa de login para: {UsernameOrEmail}", usernameOrEmail);
            var user = await _userAccountsRepository.FirstOrDefaultAsync(u =>
                (u.Username.Equals(usernameOrEmail) || u.Email.Equals(usernameOrEmail)) && u.DeletedAt == null);
            if (user == null)
            {
                _logger.LogWarning("Usuário não encontrado: {UsernameOrEmail}", usernameOrEmail);
                return ResponseBuilder<LoginResponse>
                    .Fail(new ErrorDTO { Message = "Credenciais inválidas" }).WithCode(401).Build();
            }

            if (user!.Status != EUserAccountStatus.Ativo)
            {
                _logger.LogWarning("Usuário inativo ou suspenso: {UsernameOrEmail}", usernameOrEmail);
                return ResponseBuilder<LoginResponse>
                    .Fail(new ErrorDTO { Message = "Usuário inativo", Code = "USUARIO_INATIVO" }).WithCode(401).Build();
            }

            if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            {
                _logger.LogWarning("Senha incorreta para usuário: {UserId}", user.Id);
                return ResponseBuilder<LoginResponse>.Fail(new ErrorDTO { Message = "Credenciais inválidas" })
                    .WithCode(401).Build();
            }

            var tenant = _multiTenantContext.Tenants.FirstOrDefault(t => t.Id == user.TenantId);

            var accessGroups = await GetUserAccessGroupsAsync(user.Id);
            var roles = await GetUserRolesAsync(accessGroups);
            var permissions = await GetUserPermissionsAsync(roles);

            var accessToken = _jwtTokenService.GenerateAccessToken(user, tenant, roles);
            var refreshToken = _jwtTokenService.GenerateRefreshToken();

            await StoreRefreshTokenAsync(refreshToken, user.Id, tenant?.Id);

            user.LastLoginAt = DateTime.UtcNow;
            user.UpdatedAt = DateTime.UtcNow;
            await _accessControlContext.SaveChangesAsync();

            // Verificar assinatura
            SubscriptionEntity? subscription = null;
            if (tenant != null)
                subscription = await _subscriptionRepository.GetActiveByTenantIdAsync(tenant.Id);

            var loginResponse = new LoginResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiresIn = _jwtTokenService.GetTokenExpirationTime(),
                TenantStatus = tenant?.Status.ToString(),
                //SubscriptionStatus = subscription!.Status,
                //// Requer pagamento se status for pendente ou suspenso, ou se assinatura nÃ£o estiver ativa/trial
                //RequiresPayment = tenant?.Status == ETenantStatus.Pendente || 
                //                  tenant?.Status == ETenantStatus.Suspenso ||
                //                  (subscription != null && subscription.Status != ESubscriptionStatus.Ativo && subscription.Status != ESubscriptionStatus.Trial)
            };

            _logger.LogInformation("Login bem-sucedido parausuário: {UserId}", user.Id);
            return ResponseBuilder<LoginResponse>.Ok(loginResponse).Build();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro durante o login para: {UsernameOrEmail}", usernameOrEmail);
            return ResponseBuilder<LoginResponse>
                .Fail(new ErrorDTO { Message = "Erro interno do servidor" }).WithCode(500).Build();
        }
    }

    /// <summary>
    /// Renova o token de acesso usando o refresh token
    /// </summary>
    /// <param name="refreshToken"></param>
    /// <returns></returns>
    public async Task<ResponseDTO<LoginResponse>> RefreshTokenAsync(string refreshToken)
    {
        try
        {
            _logger.LogInformation("Tentativa de renovação de token");

            var tokenData = await GetRefreshTokenDataAsync(refreshToken);
            if (tokenData == null)
            {
                _logger.LogWarning("Refresh token invalido ou expirado");
                return ResponseBuilder<LoginResponse>.Fail(new ErrorDTO { Message = "Token de renovação inválido" }).WithCode(401).Build();
            }

            var user = await _accessControlContext.UserAccounts
                .Where(u => u.Id == tokenData.UserId && u.Status == EUserAccountStatus.Ativo && u.DeletedAt == null)
                .FirstOrDefaultAsync();

            if (user == null)
            {
                _logger.LogWarning("Usuário não encontrado para refresh token: {UserId}", tokenData.UserId);
                return ResponseBuilder<LoginResponse>.Fail(new ErrorDTO { Message = "Usuário não encontrado." }).WithCode(401).Build();
            }

            TenantEntity? tenant = null;
            if (tokenData.TenantId.HasValue)
            {
                // Permite renovar token mesmo se status for pendente, para permitir acesso Ã  Ã¡rea de pagamento
                tenant = await _multiTenantContext.Tenants
                    .Where(t => t.Id == tokenData.TenantId)
                    .FirstOrDefaultAsync();
            }

            var permissions = await GetUserPermissionsAsync(user.Id);

            var accessGroups = await GetUserAccessGroupsAsync(user.Id);
            var roles = await GetUserRolesAsync(accessGroups);

            var newAccessToken = _jwtTokenService.GenerateAccessToken(user, tenant, roles);
            var newRefreshToken = _jwtTokenService.GenerateRefreshToken();

            await RevokeRefreshTokenAsync(refreshToken);
            await StoreRefreshTokenAsync(newRefreshToken, user.Id, tenant?.Id);

            // Verificar assinatura
            SubscriptionEntity? subscription = null;
            if (tenant != null)
            {
                subscription = await _subscriptionRepository.GetActiveByTenantIdAsync(tenant.Id);
            }

            var loginResponse = new LoginResponse
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken,
                ExpiresIn = _jwtTokenService.GetTokenExpirationTime(),
                TenantStatus = tenant?.Status.ToString(),
                SubscriptionStatus = (ESubscriptionStatus)(subscription!.Status),
                RequiresPayment = tenant?.Status == ETenantStatus.Pendente || 
                                  tenant?.Status == ETenantStatus.Suspenso ||
                                  (subscription != null && subscription.Status != ESubscriptionStatus.Ativo && subscription.Status != ESubscriptionStatus.Trial)
            };

            _logger.LogInformation("Token renovado com sucesso para usuário: {UserId}", user.Id);
            return ResponseBuilder<LoginResponse>.Ok(loginResponse).Build();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro durante renovação de token");
            return ResponseBuilder<LoginResponse>.Fail(new ErrorDTO { Message = "Erro interno do servidor" }).WithCode(401).Build();
        }
    }

    /// <summary>
    /// Revoga o refresh token (logout)
    /// </summary>
    /// <param name="refreshToken"></param>
    /// <returns></returns>
    public async Task<ResponseDTO<bool>> RevokeTokenAsync(string refreshToken)
    {
        try
        {
            await RevokeRefreshTokenAsync(refreshToken);
            _logger.LogInformation("Refresh token revogado com sucesso");
            return ResponseBuilder<bool>.Ok(true).Build();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao revogar token");
            return ResponseBuilder<bool>.Fail(new ErrorDTO { Message = "Erro interno do servidor" }).WithCode(500).Build();
        }
    }

    /// <summary>
    /// ObtÃ©m informaÃ§Ãµes do usuário pelo token JWT
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="tenantSlug"></param>
    /// <returns></returns>
    public async Task<ResponseDTO<UserInfo>> GetUserInfoAsync(Guid userId, string? tenantSlug = null)
    {
        try
        {
            // Busca no cache primeiro
            var cacheKey = $"{USER_CACHE_KEY_PREFIX}{userId}_{tenantSlug ?? "no_tenant"}";
            if (_cache.TryGetValue(cacheKey, out UserInfo? cachedUserInfo))
                return ResponseBuilder<UserInfo>.Ok(cachedUserInfo!).Build();

            var user = await _accessControlContext.UserAccounts
                .Where(u => u.Id == userId && u.Status == EUserAccountStatus.Ativo && u.DeletedAt == null)
                .FirstOrDefaultAsync();

            if (user == null)
            {
                _logger.LogWarning("Usuário não encontrado: {UserId}", userId);
                return ResponseBuilder<UserInfo>.Fail(new ErrorDTO { Message = "Usuário não encontrado." }).WithCode(404).Build();
            }

            TenantEntity? tenant = null;
            if (!string.IsNullOrEmpty(tenantSlug))
            {
                tenant = await _multiTenantContext.Tenants
                    .Where(t => t.Slug == tenantSlug)
                    .FirstOrDefaultAsync();
            }

            var userPermissions = await GetUserPermissionsAsync(user.Id);

            var userInfo = new UserInfo
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                FullName = $"{user.FirstName} {user.LastName}".Trim(),
                Permissions = userPermissions,
                Tenant = tenant != null ? new TenantInfoDTO
                {
                    Id = tenant.Id,
                    Name = tenant.Name,
                    Slug = tenant.Slug,
                    CustomDomain = tenant.Domain
                } : null
            };

            _cache.Set(cacheKey, userInfo, TimeSpan.FromMinutes(15));

            return ResponseBuilder<UserInfo>.Ok(userInfo).Build();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter informações do usuário: {UserId}", userId);
            return ResponseBuilder<UserInfo>.Fail(new ErrorDTO { Message = "Erro interno do servidor" }).WithCode(500).Build();
        }
    }

    /// <summary>
    /// Valida se o token JWT Ã© vÃ¡lido
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    public async Task<bool> ValidateTokenAsync(string token) =>
        await Task.FromResult(_jwtTokenService.ValidateToken(token));

    #region MÃ©todos auxiliares privados

    /// <summary>
    /// ObtÃ©m os grupos de acesso do usuário
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    private async Task<List<string>> GetUserAccessGroupsAsync(Guid userId)
    {
        return await _accessControlContext.AccountAccessGroups
            .Where(aag => aag.UserAccountId == userId && aag.IsActive)
            .Include(aag => aag.AccessGroup)
            .Where(aag => aag.AccessGroup.IsActive)
            .Select(aag => aag.AccessGroup.Name)
            .ToListAsync();
    }

    /// <summary>
    /// ObtÃ©m as roles do usuário com base nos grupos de acesso
    /// </summary>
    /// <param name="accessGroups"></param>
    /// <returns></returns>
    private async Task<List<string>> GetUserRolesAsync(List<string> accessGroups)
    {
        return await _accessControlContext.RoleAccessGroups
            .Where(rag => accessGroups.Contains(rag.AccessGroup.Name) && rag.IsActive)
            .Include(rag => rag.Role)
            .Where(rag => rag.Role.IsActive)
            .Select(rag => rag.Role.Code)
            .Distinct()
            .ToListAsync();
    }

    /// <summary>
    /// Obtém as permissões do usuário com base nas roles
    /// </summary>
    /// <param name="roles"></param>
    /// <returns></returns>
    private async Task<List<string>> GetUserPermissionsAsync(List<string> roles)
    {
        // Resolver permissÃµes via relaÃ§Ãµes RolePermission (N:N), evitando dependÃªncia de Permission.RoleId
        return await _accessControlContext.RolePermissions
            .Where(rp => roles.Contains(rp.Role!.Name) && rp.IsActive)
            .Include(rp => rp.Role)
            .Include(rp => rp.Permission)
            .Where(rp => rp.Role!.IsActive && rp.Permission!.IsActive)
            .Select(rp => rp.Permission!.Name)
            .Distinct()
            .ToListAsync();
    }

    /// <summary>
    /// Obtém permissões detalhadas do usuário
    /// </summary>
    /// <param name="UserId"></param>
    /// <returns></returns>
    private async Task<UserPermissionsDTO> GetUserPermissionsAsync(Guid UserId) => 
        await _moduleRepository.GetModulesByUserAsync(UserId);

    /// <summary>
    /// Armazena o refresh token em cache
    /// </summary>
    /// <param name="refreshToken"></param>
    /// <param name="userId"></param>
    /// <param name="tenantId"></param>
    /// <returns></returns>
    private async Task StoreRefreshTokenAsync(string refreshToken, Guid userId, Guid? tenantId)
    {
        var tokenData = new RefreshTokenData
        {
            UserId = userId,
            TenantId = tenantId,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddDays(7)
        };

        if (!_cache.TryGetValue(REFRESH_TOKENS_CACHE_KEY, out Dictionary<string, RefreshTokenData>? tokens))
        {
            tokens = new Dictionary<string, RefreshTokenData>();
        }

        tokens![refreshToken] = tokenData;
        _cache.Set(REFRESH_TOKENS_CACHE_KEY, tokens, TimeSpan.FromDays(7));

        await Task.CompletedTask;
    }

    /// <summary>
    /// Obtém dados do refresh token do cache
    /// </summary>
    /// <param name="refreshToken"></param>
    /// <returns></returns>
    private async Task<RefreshTokenData?> GetRefreshTokenDataAsync(string refreshToken)
    {
        if (_cache.TryGetValue(REFRESH_TOKENS_CACHE_KEY, out Dictionary<string, RefreshTokenData>? tokens))
        {
            if (tokens!.TryGetValue(refreshToken, out RefreshTokenData? tokenData))
            {
                if (tokenData.ExpiresAt > DateTime.UtcNow)
                {
                    return await Task.FromResult(tokenData);
                }

                tokens.Remove(refreshToken);
                _cache.Set(REFRESH_TOKENS_CACHE_KEY, tokens, TimeSpan.FromDays(7));
            }
        }

        return await Task.FromResult<RefreshTokenData?>(null);
    }

    /// <summary>
    /// Revoga o refresh token removendo do cache
    /// </summary>
    /// <param name="refreshToken"></param>
    /// <returns></returns>
    private async Task RevokeRefreshTokenAsync(string refreshToken)
    {
        if (_cache.TryGetValue(REFRESH_TOKENS_CACHE_KEY, out Dictionary<string, RefreshTokenData>? tokens))
        {
            tokens!.Remove(refreshToken);
            _cache.Set(REFRESH_TOKENS_CACHE_KEY, tokens, TimeSpan.FromDays(7));
        }

        await Task.CompletedTask;
    }

    #endregion
}


