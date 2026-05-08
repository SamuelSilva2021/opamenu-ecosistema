using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using OpaMenu.Application.Services.Interfaces.Auth;
using OpaMenu.Commons.Api.Commons;
using OpaMenu.Commons.Api.DTOs;
using OpaMenu.Domain.DTOs.Auth;
using OpaMenu.Infrastructure.Shared.Data.Context.AccessControl;
using OpaMenu.Infrastructure.Shared.Data.Context.MultTenant;
using OpaMenu.Infrastructure.Shared.Entities.AccessControl;
using OpaMenu.Infrastructure.Shared.Entities.AccessControl.UserAccounts;
using OpaMenu.Infrastructure.Shared.Entities.AccessControl.UserAccounts.Enum;
using OpaMenu.Infrastructure.Shared.Entities.MultiTenant.Subscription;
using OpaMenu.Infrastructure.Shared.Entities.MultiTenant.Tenant;
using OpaMenu.Infrastructure.Shared.Entities.MultiTenant.TenantModule;
using OpaMenu.Infrastructure.Shared.Entities.MultiTenant.PlanModule;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace OpaMenu.Infrastructure.Authentication;

public sealed class AuthService(
    AccessControlDbContext accessControlDbContext,
    MultiTenantDbContext multiTenantDbContext,
    IDistributedCache cache,
    IConfiguration configuration,
    ILogger<AuthService> logger) : IAuthService
{
    private readonly AccessControlDbContext _accessControlDbContext = accessControlDbContext;
    private readonly MultiTenantDbContext _multiTenantDbContext = multiTenantDbContext;
    private readonly IDistributedCache _cache = cache;
    private readonly IConfiguration _configuration = configuration;
    private readonly ILogger<AuthService> _logger = logger;

    public async Task<ResponseDTO<LoginResponseDto>> LoginAsync(LoginRequestDto request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.UsernameOrEmail) || string.IsNullOrWhiteSpace(request.Password))
            {
                return StaticResponseBuilder<LoginResponseDto>.BuildError("Credenciais inválidas.");
            }

            var user = await _accessControlDbContext.UserAccounts
                .AsNoTracking()
                .FirstOrDefaultAsync(u =>
                    u.DeletedAt == null &&
                    (u.Username == request.UsernameOrEmail || u.Email == request.UsernameOrEmail));

            if (user == null || user.Status != EUserAccountStatus.Ativo)
            {
                return StaticResponseBuilder<LoginResponseDto>.BuildError("Credenciais inválidas.");
            }

            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                return StaticResponseBuilder<LoginResponseDto>.BuildError("Credenciais inválidas.");
            }

            var tenant = await GetTenantAsync(user.TenantId);
            var roles = await GetRoleCodesAsync(user);
            var refreshToken = GenerateRefreshToken();

            var refreshTokenExpiresAt = DateTime.UtcNow.AddDays(GetRefreshTokenExpirationDays());
            var refreshTokenCacheKey = GetRefreshTokenCacheKey(refreshToken);
            var refreshPayload = new RefreshTokenPayload
            {
                UserId = user.Id,
                TenantId = tenant?.Id,
                ExpiresAtUtc = refreshTokenExpiresAt
            };

            await _cache.SetStringAsync(
                refreshTokenCacheKey,
                JsonSerializer.Serialize(refreshPayload),
                new DistributedCacheEntryOptions { AbsoluteExpiration = refreshTokenExpiresAt });

            var accessToken = GenerateAccessToken(user, tenant, roles);

            var subscription = tenant?.Id != null
                ? await GetActiveSubscriptionAsync(tenant.Id)
                : null;

            var response = new LoginResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiresIn = GetAccessTokenExpirationMinutes() * 60,
                TenantStatus = tenant?.Status.ToString(),
                SubscriptionStatus = subscription?.Status.ToString(),
                RequiresPayment = tenant?.Status == ETenantStatus.Pendente ||
                                  tenant?.Status == ETenantStatus.Suspenso ||
                                  subscription == null ||
                                  (subscription.Status != ESubscriptionStatus.Ativo && subscription.Status != ESubscriptionStatus.Trial),
                RedirectToPlanSelection = tenant != null && (subscription == null || (subscription.Status != ESubscriptionStatus.Ativo && subscription.Status != ESubscriptionStatus.Trial))
            };

            return StaticResponseBuilder<LoginResponseDto>.BuildOk(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro no login");
            return StaticResponseBuilder<LoginResponseDto>.BuildErrorResponse(ex);
        }
    }

    public async Task<ResponseDTO<LoginResponseDto>> RefreshAsync(string refreshToken)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
            {
                return StaticResponseBuilder<LoginResponseDto>.BuildError("Refresh token inválido.");
            }

            var payload = await GetRefreshTokenPayloadAsync(refreshToken);
            if (payload == null || payload.ExpiresAtUtc <= DateTime.UtcNow)
            {
                return StaticResponseBuilder<LoginResponseDto>.BuildError("Refresh token inválido.");
            }

            var user = await _accessControlDbContext.UserAccounts
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == payload.UserId && u.DeletedAt == null);

            if (user == null || user.Status != EUserAccountStatus.Ativo)
            {
                return StaticResponseBuilder<LoginResponseDto>.BuildError("Usuário inválido.");
            }

            var tenant = payload.TenantId.HasValue
                ? await _multiTenantDbContext.Tenants.AsNoTracking().FirstOrDefaultAsync(t => t.Id == payload.TenantId.Value)
                : null;

            var roles = await GetRoleCodesAsync(user);

            var newRefreshToken = GenerateRefreshToken();
            var refreshTokenExpiresAt = DateTime.UtcNow.AddDays(GetRefreshTokenExpirationDays());
            var newPayload = new RefreshTokenPayload
            {
                UserId = user.Id,
                TenantId = tenant?.Id,
                ExpiresAtUtc = refreshTokenExpiresAt
            };

            await _cache.RemoveAsync(GetRefreshTokenCacheKey(refreshToken));
            await _cache.SetStringAsync(
                GetRefreshTokenCacheKey(newRefreshToken),
                JsonSerializer.Serialize(newPayload),
                new DistributedCacheEntryOptions { AbsoluteExpiration = refreshTokenExpiresAt });

            var accessToken = GenerateAccessToken(user, tenant, roles);
            var subscription = tenant?.Id != null
                ? await GetActiveSubscriptionAsync(tenant.Id)
                : null;

            var response = new LoginResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = newRefreshToken,
                ExpiresIn = GetAccessTokenExpirationMinutes() * 60,
                TenantStatus = tenant?.Status.ToString(),
                SubscriptionStatus = subscription?.Status.ToString(),
                RequiresPayment = tenant?.Status == ETenantStatus.Pendente ||
                                  tenant?.Status == ETenantStatus.Suspenso ||
                                  subscription == null ||
                                  (subscription.Status != ESubscriptionStatus.Ativo && subscription.Status != ESubscriptionStatus.Trial),
                RedirectToPlanSelection = tenant != null && (subscription == null || (subscription.Status != ESubscriptionStatus.Ativo && subscription.Status != ESubscriptionStatus.Trial))
            };

            return StaticResponseBuilder<LoginResponseDto>.BuildOk(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao renovar token");
            return StaticResponseBuilder<LoginResponseDto>.BuildErrorResponse(ex);
        }
    }

    public async Task<ResponseDTO<bool>> LogoutAsync(string refreshToken)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
            {
                return StaticResponseBuilder<bool>.BuildError("Refresh token inválido.");
            }

            await _cache.RemoveAsync(GetRefreshTokenCacheKey(refreshToken));
            return StaticResponseBuilder<bool>.BuildOk(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao fazer logout");
            return StaticResponseBuilder<bool>.BuildErrorResponse(ex);
        }
    }

    public async Task<ResponseDTO<UserInfoDto>> GetMeAsync(Guid userId, string? tenantSlug)
    {
        try
        {
            var user = await _accessControlDbContext.UserAccounts
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == userId && u.DeletedAt == null);

            if (user == null)
            {
                return StaticResponseBuilder<UserInfoDto>.BuildError("Usuário não encontrado.");
            }

            var tenant = await GetTenantBySlugOrUserTenantAsync(tenantSlug, user.TenantId);
            var effectivePermissions = await GetEffectivePermissionsAsync(user, tenant);
            var moduleKeyToId = await GetModuleKeyToIdMapAsync(effectivePermissions.Select(p => p.ModuleKey));

            var role = await GetPrimaryRoleAsync(user);
            var simplifiedRole = role == null
                ? null
                : new SimplifiedRoleDto
                {
                    Id = role.Id,
                    Name = role.Code ?? role.Name,
                    Permissions = effectivePermissions
                        .Select(p => new SimplifiedPermissionDto { Module = p.ModuleKey, Actions = p.Actions })
                        .ToList()
                };

            var accessGroups = await _accessControlDbContext.AccountAccessGroups
                .AsNoTracking()
                .Where(aag => aag.UserAccountId == user.Id && aag.IsActive)
                .Select(aag => aag.AccessGroup)
                .Where(ag => ag.IsActive)
                .ToListAsync();

            var rolePermissionsByRole = await GetRolePermissionsByRoleAsync(user);

            var permissions = new UserPermissionsDto
            {
                UserId = user.Id,
                AccessGroups = accessGroups.Select(ag => new AccessGroupBasicDto
                {
                    Id = ag.Id,
                    Code = ag.Code ?? ag.Name,
                    Roles = rolePermissionsByRole.Select(rp => new RoleBasicDto
                    {
                        Id = rp.RoleId,
                        Code = rp.RoleCode,
                        Modules = rp.Permissions
                            .Where(p => moduleKeyToId.ContainsKey(p.ModuleKey))
                            .Select(p => new ModuleBasicDto
                            {
                                Id = moduleKeyToId[p.ModuleKey],
                                Key = p.ModuleKey,
                                Operations = p.Actions
                            })
                            .ToList()
                    }).ToList()
                }).ToList()
            };

            var userInfo = new UserInfoDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                FullName = $"{user.FirstName} {user.LastName}".Trim(),
                Permissions = permissions,
                Role = simplifiedRole,
                Tenant = tenant == null
                    ? null
                    : new TenantInfoDto
                    {
                        Id = tenant.Id,
                        Name = tenant.Name,
                        Slug = tenant.Slug,
                        CustomDomain = tenant.Domain
                    }
            };

            return StaticResponseBuilder<UserInfoDto>.BuildOk(userInfo);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter dados do usuário");
            return StaticResponseBuilder<UserInfoDto>.BuildErrorResponse(ex);
        }
    }

    private async Task<TenantEntity?> GetTenantAsync(Guid? tenantId)
    {
        if (!tenantId.HasValue || tenantId.Value == Guid.Empty)
        {
            return null;
        }

        return await _multiTenantDbContext.Tenants.AsNoTracking().FirstOrDefaultAsync(t => t.Id == tenantId.Value);
    }

    private async Task<TenantEntity?> GetTenantBySlugOrUserTenantAsync(string? tenantSlug, Guid? userTenantId)
    {
        if (!string.IsNullOrWhiteSpace(tenantSlug))
        {
            return await _multiTenantDbContext.Tenants.AsNoTracking().FirstOrDefaultAsync(t => t.Slug == tenantSlug);
        }

        return await GetTenantAsync(userTenantId);
    }

    private async Task<SubscriptionEntity?> GetActiveSubscriptionAsync(Guid tenantId)
    {
        return await _multiTenantDbContext.Subscriptions
            .AsNoTracking()
            .Where(s => s.TenantId == tenantId && (
                s.Status == ESubscriptionStatus.Ativo || 
                (s.Status == ESubscriptionStatus.Trial && s.TrialEndsAt != null && s.TrialEndsAt > DateTime.UtcNow)
            ))
            .OrderByDescending(s => s.UpdatedAt)
            .FirstOrDefaultAsync();
    }

    private async Task<List<string>> GetRoleCodesAsync(UserAccountEntity user)
    {
        var roleIds = await GetUserRoleIdsAsync(user);

        var roleCodes = await _accessControlDbContext.Roles
            .IgnoreQueryFilters()
            .AsNoTracking()
            .Where(r => roleIds.Contains(r.Id) && r.IsActive)
            .Select(r => r.Code ?? r.Name)
            .Distinct()
            .ToListAsync();

        return roleCodes;
    }

    private async Task<List<Guid>> GetUserRoleIdsAsync(UserAccountEntity user)
    {
        var roleIds = new HashSet<Guid>();

        if (user.RoleId.HasValue && user.RoleId.Value != Guid.Empty)
        {
            roleIds.Add(user.RoleId.Value);
        }

        var groupRoleIds = await _accessControlDbContext.AccountAccessGroups
            .IgnoreQueryFilters()
            .AsNoTracking()
            .Where(aag => aag.UserAccountId == user.Id && aag.IsActive)
            .SelectMany(aag => aag.AccessGroup.RoleAccessGroups)
            .Where(rag => rag.IsActive)
            .Select(rag => rag.RoleId)
            .ToListAsync();

        foreach (var roleId in groupRoleIds)
        {
            if (roleId != Guid.Empty)
            {
                roleIds.Add(roleId);
            }
        }

        return roleIds.ToList();
    }

    private async Task<RoleEntity?> GetPrimaryRoleAsync(UserAccountEntity user)
    {
        if (user.RoleId.HasValue && user.RoleId.Value != Guid.Empty)
        {
            return await _accessControlDbContext.Roles
                .IgnoreQueryFilters()
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Id == user.RoleId.Value);
        }

        var roleId = await _accessControlDbContext.AccountAccessGroups
            .IgnoreQueryFilters()
            .AsNoTracking()
            .Where(aag => aag.UserAccountId == user.Id && aag.IsActive)
            .SelectMany(aag => aag.AccessGroup.RoleAccessGroups)
            .Where(rag => rag.IsActive)
            .Select(rag => rag.RoleId)
            .FirstOrDefaultAsync();

        if (roleId == Guid.Empty)
        {
            return null;
        }

        return await _accessControlDbContext.Roles
            .IgnoreQueryFilters()
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == roleId);
    }

    private async Task<List<PermissionAggregate>> GetRolePermissionsAsync(List<Guid> roleIds)
    {
        var permissions = await _accessControlDbContext.RolePermissions
            .IgnoreQueryFilters()
            .AsNoTracking()
            .Where(rp => roleIds.Contains(rp.RoleId) && rp.IsActive)
            .Select(rp => new PermissionAggregate
            {
                ModuleKey = rp.ModuleKey,
                Actions = rp.Actions
            })
            .ToListAsync();

        return permissions
            .GroupBy(p => p.ModuleKey, StringComparer.OrdinalIgnoreCase)
            .Select(g => new PermissionAggregate
            {
                ModuleKey = g.Key,
                Actions = g.SelectMany(x => x.Actions).Distinct(StringComparer.OrdinalIgnoreCase).ToList()
            })
            .ToList();
    }

    private async Task<List<PermissionAggregate>> GetEffectivePermissionsAsync(UserAccountEntity user, TenantEntity? tenant)
    {
        if (tenant == null)
        {
            return await GetRolePermissionsAsync(await GetUserRoleIdsAsync(user));
        }

        // Tenta buscar do cache primeiro
        var cacheKey = $"auth:permissions:effective:{user.Id}:{tenant.Id}";
        var cached = await _cache.GetStringAsync(cacheKey);
        if (!string.IsNullOrEmpty(cached))
        {
            return JsonSerializer.Deserialize<List<PermissionAggregate>>(cached) ?? new List<PermissionAggregate>();
        }

        var roleIds = await GetUserRoleIdsAsync(user);
        var rolePermissions = await GetRolePermissionsAsync(roleIds);

        // 1. Buscar IDs de planos das assinaturas ativas do tenant
        var activePlanIds = await _multiTenantDbContext.Subscriptions
            .AsNoTracking()
            .Where(s => s.TenantId == tenant.Id && (s.Status == ESubscriptionStatus.Ativo || (s.Status == ESubscriptionStatus.Trial && s.CurrentPeriodEnd > DateTime.UtcNow)))
            .Select(s => s.PlanId)
            .ToListAsync();

        // 2. Buscar módulos vinculados a esses planos (Dinâmico)
        var planModuleIds = await _multiTenantDbContext.Set<PlanModuleEntity>()
            .AsNoTracking()
            .Where(pm => activePlanIds.Contains(pm.PlanId))
            .Select(pm => pm.ModuleId)
            .ToListAsync();

        // 3. Buscar módulos habilitados explicitamente para o tenant (Extras/Overrides)
        var tenantModuleIds = await _multiTenantDbContext.Set<TenantModuleEntity>()
            .AsNoTracking()
            .Where(tm => tm.TenantId == tenant.Id && tm.IsEnabled)
            .Select(tm => tm.ModuleId)
            .ToListAsync();

        // 4. Unir todos os IDs de módulos habilitados
        var enabledModuleIds = planModuleIds.Union(tenantModuleIds).Distinct().ToList();

        if (enabledModuleIds.Count == 0) return [];

        var enabledModuleKeys = await _accessControlDbContext.Modules
            .AsNoTracking()
            .Where(m => enabledModuleIds.Contains(m.Id) && m.IsActive)
            .Select(m => m.Key!)
            .ToListAsync();

        var result = rolePermissions
            .Where(p => enabledModuleKeys.Contains(p.ModuleKey, StringComparer.OrdinalIgnoreCase))
            .ToList();

        // Salva no cache por 15 minutos
        await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(result), new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(15)
        });

        return result;
    }

    private async Task<Dictionary<string, Guid>> GetModuleKeyToIdMapAsync(IEnumerable<string> moduleKeys)
    {
        var keys = moduleKeys.Where(k => !string.IsNullOrWhiteSpace(k)).Distinct(StringComparer.OrdinalIgnoreCase).ToList();
        if (keys.Count == 0)
        {
            return new Dictionary<string, Guid>(StringComparer.OrdinalIgnoreCase);
        }

        var modules = await _accessControlDbContext.Modules
            .AsNoTracking()
            .Where(m => m.Key != null && keys.Contains(m.Key) && m.IsActive)
            .Select(m => new { m.Id, Key = m.Key! })
            .ToListAsync();

        return modules
            .GroupBy(x => x.Key, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(g => g.Key, g => g.First().Id, StringComparer.OrdinalIgnoreCase);
    }

    private async Task<List<RolePermissionsAggregate>> GetRolePermissionsByRoleAsync(UserAccountEntity user)
    {
        var roleIds = await GetUserRoleIdsAsync(user);
        if (roleIds.Count == 0)
        {
            return [];
        }

        var roles = await _accessControlDbContext.Roles
            .IgnoreQueryFilters()
            .AsNoTracking()
            .Where(r => roleIds.Contains(r.Id) && r.IsActive)
            .Select(r => new { r.Id, RoleCode = r.Code ?? r.Name })
            .ToListAsync();

        var permissions = await _accessControlDbContext.RolePermissions
            .IgnoreQueryFilters()
            .AsNoTracking()
            .Where(rp => roleIds.Contains(rp.RoleId) && rp.IsActive)
            .Select(rp => new { rp.RoleId, rp.ModuleKey, rp.Actions })
            .ToListAsync();

        return roles.Select(r => new RolePermissionsAggregate
        {
            RoleId = r.Id,
            RoleCode = r.RoleCode,
            Permissions = permissions
                .Where(p => p.RoleId == r.Id)
                .Select(p => new PermissionAggregate { ModuleKey = p.ModuleKey, Actions = p.Actions })
                .ToList()
        }).ToList();
    }

    private string GenerateAccessToken(UserAccountEntity user, TenantEntity? tenant, IEnumerable<string> roles)
    {
        var jwtSecret = _configuration["Authentication:JwtSecret"];
        var jwtIssuer = _configuration["Authentication:JwtIssuer"];
        var jwtAudience = _configuration["Authentication:JwtAudience"];

        if (string.IsNullOrWhiteSpace(jwtSecret))
        {
            throw new InvalidOperationException("JWT Secret não configurado");
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.Username),
            new(ClaimTypes.Email, user.Email),
            new("user_id", user.Id.ToString()),
            new("username", user.Username),
            new("email", user.Email)
        };

        if (tenant != null)
        {
            claims.Add(new Claim("tenant_id", tenant.Id.ToString()));
            claims.Add(new Claim("tenant_slug", tenant.Slug));
        }

        foreach (var role in roles.Where(r => !string.IsNullOrWhiteSpace(r)).Distinct(StringComparer.OrdinalIgnoreCase))
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        // As permissões foram removidas do JWT para mantê-lo leve. 
        // O backend agora as valida via cache/DB e o frontend as busca via endpoint /me.

        var expires = DateTime.UtcNow.AddMinutes(GetAccessTokenExpirationMinutes());
        var token = new JwtSecurityToken(
            issuer: string.IsNullOrWhiteSpace(jwtIssuer) ? null : jwtIssuer,
            audience: string.IsNullOrWhiteSpace(jwtAudience) ? null : jwtAudience,
            claims: claims,
            expires: expires,
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private static string GenerateRefreshToken()
    {
        var bytes = RandomNumberGenerator.GetBytes(64);
        return Convert.ToBase64String(bytes);
    }

    private int GetAccessTokenExpirationMinutes()
    {
        if (int.TryParse(_configuration["Authentication:AccessTokenExpirationMinutes"], out var minutes) && minutes > 0)
        {
            return minutes;
        }

        return 30;
    }

    private int GetRefreshTokenExpirationDays()
    {
        if (int.TryParse(_configuration["Authentication:RefreshTokenExpirationDays"], out var days) && days > 0)
        {
            return days;
        }

        return 7;
    }

    private static string GetRefreshTokenCacheKey(string refreshToken) => $"auth:refresh:{refreshToken}";

    private async Task<RefreshTokenPayload?> GetRefreshTokenPayloadAsync(string refreshToken)
    {
        var json = await _cache.GetStringAsync(GetRefreshTokenCacheKey(refreshToken));
        if (string.IsNullOrWhiteSpace(json))
        {
            return null;
        }

        try
        {
            return JsonSerializer.Deserialize<RefreshTokenPayload>(json);
        }
        catch
        {
            return null;
        }
    }
}

