using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using OpaMenu.Domain.Interfaces;
using OpaMenu.Infrastructure.Anotations;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using OpaMenu.Infrastructure.Shared.Data.Context;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
using OpaMenu.Infrastructure.Shared.Data.Context.AccessControl;

namespace OpaMenu.Infrastructure.Filters
{
    public class PermissionAuthorizationFilter(
        ICurrentUserService currentUserService,
        ILogger<PermissionAuthorizationFilter> logger,
        AccessControlDbContext dbContext,
        IDistributedCache cache) : IAsyncActionFilter
    {
        private readonly ICurrentUserService _currentUserService = currentUserService;
        private readonly ILogger<PermissionAuthorizationFilter> _logger = logger;
        private readonly AccessControlDbContext _dbContext = dbContext;
        private readonly IDistributedCache _cache = cache;

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (context.ActionDescriptor is not ControllerActionDescriptor descriptor)
            {
                await next();
                return;
            }

            var mapAttr = descriptor.MethodInfo
                .GetCustomAttributes(typeof(MapPermission), inherit: true)
                .OfType<MapPermission>()
                .FirstOrDefault()
                ?? descriptor.ControllerTypeInfo
                    .GetCustomAttributes(typeof(MapPermission), inherit: true)
                    .OfType<MapPermission>()
                    .FirstOrDefault();

            if (mapAttr == null)
            {
                await next();
                return;
            }

            if (!_currentUserService.IsAuthenticated)
            {
                context.Result = new ObjectResult(new { message = "Não autenticado" })
                {
                    StatusCode = StatusCodes.Status401Unauthorized
                };
                return;
            }

            var requiredModule = mapAttr.Modulo?.Trim();
            var requiredOperation = mapAttr.Operation?.Trim();
            if (string.IsNullOrWhiteSpace(requiredModule))
            {
                await next();
                return;
            }

            var permissions = _currentUserService.Permissions
                .Where(p => !string.IsNullOrWhiteSpace(p))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            var hasPermission = permissions.Any(p =>
                string.Equals(p, requiredModule, StringComparison.OrdinalIgnoreCase) ||
                (!string.IsNullOrEmpty(requiredOperation) &&
                 string.Equals(p, $"{requiredModule}:{requiredOperation}", StringComparison.OrdinalIgnoreCase)));

            if (!hasPermission)
            {
                if (Guid.TryParse(_currentUserService.UserId, out var userId) &&
                    Guid.TryParse(_currentUserService.TenantId, out var tenantId))
                {
                    try
                    {
                        // 1. Tentar buscar permissões do cache
                        var cacheKey = $"auth:permissions:{userId}:{tenantId}";
                        var cachedPermissions = await _cache.GetStringAsync(cacheKey);
                        List<string> userPermissions;

                        if (!string.IsNullOrEmpty(cachedPermissions))
                        {
                            userPermissions = JsonSerializer.Deserialize<List<string>>(cachedPermissions) ?? new List<string>();
                        }
                        else
                        {
                            // 2. Se não estiver no cache, buscar do banco (TODAS as permissões do usuário)
                            // A query busca todas as permissões ativas associadas ao usuário através da hierarquia
                            var permissionsData = await _dbContext.UserAccounts
                                .AsNoTracking()
                                .Where(u => u.Id == userId && u.TenantId == tenantId)
                                .SelectMany(u => u.AccountAccessGroups)
                                .Select(aag => aag.AccessGroup)
                                .Where(ag => ag.IsActive)
                                .SelectMany(ag => ag.RoleAccessGroups)
                                .Select(rag => rag.Role)
                                .Where(r => r.IsActive)
                                .SelectMany(r => r.RolePermissions)
                                .Select(rp => rp.Permission)
                                .Where(p => p.IsActive)
                                .Select(p => new 
                                { 
                                    ModuleKey = p.Module.Key, 
                                    Operations = p.PermissionOperations
                                        .Where(po => po.IsActive)
                                        .Select(po => po.Operation.Value)
                                        .ToList()
                                })
                                .ToListAsync();

                            // 3. Processar e formatar permissões
                            userPermissions = new List<string>();
                            foreach (var p in permissionsData)
                            {
                                if (!string.IsNullOrEmpty(p.ModuleKey))
                                {
                                    // Adiciona permissão de módulo (ex: "DASHBOARD")
                                    userPermissions.Add(p.ModuleKey);
                                    
                                    // Adiciona permissões de operação (ex: "DASHBOARD:READ")
                                    foreach (var op in p.Operations)
                                    {
                                        if (!string.IsNullOrEmpty(op))
                                        {
                                            userPermissions.Add($"{p.ModuleKey}:{op}");
                                        }
                                    }
                                }
                            }
                            
                            // Remove duplicatas e normaliza
                            userPermissions = userPermissions.Distinct(StringComparer.OrdinalIgnoreCase).ToList();

                            // 4. Salvar no cache (expiração de 30 minutos)
                            var cacheOptions = new DistributedCacheEntryOptions
                            {
                                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30)
                            };
                            await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(userPermissions), cacheOptions);
                        }

                        // 5. Verificar se possui a permissão necessária na lista carregada
                        hasPermission = userPermissions.Any(p =>
                            string.Equals(p, requiredModule, StringComparison.OrdinalIgnoreCase) ||
                            (!string.IsNullOrEmpty(requiredOperation) &&
                             string.Equals(p, $"{requiredModule}:{requiredOperation}", StringComparison.OrdinalIgnoreCase)));

                        if (hasPermission)
                        {
                            _logger.LogInformation(
                                "Permissão confirmada via Cache/Banco para usuário {UserId} no módulo {Module} operação {Operation}",
                                userId, requiredModule, requiredOperation);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Erro ao verificar permissões no cache/banco para o usuário {UserId}", userId);
                    }
                }
            }

            if (!hasPermission && permissions.Count == 0)
            {
                var roles = _currentUserService.Roles
                    .Where(r => !string.IsNullOrWhiteSpace(r))
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .ToList();

                var isAdminRole = roles.Any(r =>
                    string.Equals(r, "ADMIN", StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(r, "SUPER_ADMIN", StringComparison.OrdinalIgnoreCase));

                if (isAdminRole)
                {
                    _logger.LogInformation(
                        "Acesso permitido (fallback por role) para usuário {UserId} no módulo {Module} operação {Operation}",
                        _currentUserService.UserId,
                        requiredModule,
                        requiredOperation);
                    await next();
                    return;
                }
            }

            if (!hasPermission)
            {
                _logger.LogWarning(
                    "Acesso negado para usuário {UserId} no módulo {Module} operação {Operation}",
                    _currentUserService.UserId,
                    requiredModule,
                    requiredOperation);
                context.Result = new ObjectResult(new { message = "Acesso negado" })
                {
                    StatusCode = StatusCodes.Status403Forbidden
                };
                return;
            }

            _logger.LogInformation(
                "Acesso permitido para usuário {UserId} no módulo {Module} operação {Operation}",
                _currentUserService.UserId,
                requiredModule,
                requiredOperation);

            await next();
        }
    }
}
