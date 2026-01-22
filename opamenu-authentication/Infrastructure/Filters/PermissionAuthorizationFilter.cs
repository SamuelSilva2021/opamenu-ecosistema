using Authenticator.API.Core.Application.Interfaces;
using Authenticator.API.Core.Application.Interfaces.MultiTenant;
using Authenticator.API.Core.Domain.AccessControl.Modules.DTOs;
using Authenticator.API.Core.Domain.Api;
using Authenticator.API.Infrastructure.Configurations;
using OpaMenu.Infrastructure.Shared.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Security.Claims;

namespace Authenticator.API.Infrastructure.Filters
{
    /// <summary>
    /// Filtro global de autorizaÃ§Ã£o baseado em permissÃµes.
    /// LÃª o atributo [MapPermission(module, operation)] aplicado em actions ou controladores
    /// e valida se o usuário autenticado possui a permissÃ£o necessÃ¡ria.
    /// 
    /// EstratÃ©gia:
    /// 1) Busca o usuário e suas permissÃµes via cache usando IAuthenticationService.GetUserInfoAsync
    /// 2) Caso o cache/serviÃ§o falhe, realiza fallback para os claims do JWT ("permission")
    /// 3) Atualmente as permissÃµes no token representam o nome do mÃ³dulo (Permission.Name).
    ///    Assim, a validaÃ§Ã£o considera o mÃ³dulo informado em [MapPermission]. A operaÃ§Ã£o
    ///    Ã© registrada para auditoria, mas nÃ£o influencia na decisÃ£o enquanto o token nÃ£o
    ///    incluir operaÃ§Ãµes por permissÃ£o.
    /// </summary>
    public class PermissionAuthorizationFilter : IAsyncActionFilter
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly ITenantContext _tenantContext;
        private readonly ILogger<PermissionAuthorizationFilter> _logger;

        public PermissionAuthorizationFilter(
            IAuthenticationService authenticationService,
            ITenantContext tenantContext,
            ILogger<PermissionAuthorizationFilter> logger)
        {
            _authenticationService = authenticationService;
            _tenantContext = tenantContext;
            _logger = logger;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var descriptor = context.ActionDescriptor as ControllerActionDescriptor;
            if (descriptor == null)
            {
                await next();
                return;
            }

            // Tenta obter o atributo [MapPermission] da action; se nÃ£o houver, verifica no controller
            var mapAttr = descriptor.MethodInfo
                .GetCustomAttributes(typeof(MapPermission), inherit: true)
                .OfType<MapPermission>()
                .FirstOrDefault()
                ?? descriptor.ControllerTypeInfo
                    .GetCustomAttributes(typeof(MapPermission), inherit: true)
                    .OfType<MapPermission>()
                    .FirstOrDefault();

            // Se nÃ£o houver mapeamento de permissÃ£o, segue o fluxo normal
            if (mapAttr == null)
            {
                await next();
                return;
            }

            // Garante que estÃ¡ autenticado
            if (context.HttpContext.User?.Identity?.IsAuthenticated != true)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            // ObtÃ©m UserId do claim (preferÃªncia pelo ClaimTypes.NameIdentifier)
            var userIdClaim = context.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                              ?? context.HttpContext.User.FindFirst("user_id")?.Value;

            if (string.IsNullOrWhiteSpace(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                _logger.LogWarning("UserId invÃ¡lido nos claims ao validar permissÃ£o {Module}/{Operation}", mapAttr.Module, mapAttr.Operation);
                context.Result = new UnauthorizedResult();
                return;
            }

            var tenantSlug = _tenantContext.TenantSlug; // populado pelo TenantContextMiddleware
            var requiredModuleName = mapAttr.Module?.Trim();
            var requiredOperation = mapAttr.Operation?.Trim();

            try
            {
                // 1) Tenta obter permissÃµes via serviÃ§o (com cache interno)
                var userInfoResponse = await _authenticationService.GetUserInfoAsync(userId, tenantSlug);
                List<string> permissions = [];
                IEnumerable<ModuleBasicDTO> modules = [];

                if (userInfoResponse.Succeeded && userInfoResponse.Data != null)
                {
                    var accessGroup = userInfoResponse.Data.Permissions.AccessGroups;
                    var roles = accessGroup.SelectMany(ag => ag.Roles).ToList();
                    modules = roles.SelectMany(r => r.Modules).DistinctBy(m => m.Key).ToList();
                }
                else
                {
                    // 2) Fallback: extrai permissÃµes dos claims do JWT
                    permissions = context.HttpContext.User.FindAll("permission")
                        .Select(c => c.Value)
                        .Where(v => !string.IsNullOrWhiteSpace(v))
                        .Distinct(StringComparer.OrdinalIgnoreCase)
                        .ToList();
                }

                var moduleToValidate = modules.FirstOrDefault(m => string.Equals(m.Key, requiredModuleName, StringComparison.OrdinalIgnoreCase));
                var operationsToModule = moduleToValidate.Operations;

                var hasPermissionModule = modules.Any(p => string.Equals(p.Key, requiredModuleName, StringComparison.OrdinalIgnoreCase));
                var hasPermissionOperation = operationsToModule.Any(o => string.Equals(o, requiredOperation, StringComparison.OrdinalIgnoreCase));

                if (!hasPermissionModule)
                {
                    _logger.LogWarning(
                        "Acesso negado. usuário {UserId} nÃ£o possui permissÃ£o para {Module}/{Operation} no tenant {Tenant}",
                        userId, requiredModuleName, requiredOperation, tenantSlug);
                    context.Result = new ForbidResult();
                    return;
                }

                if(!hasPermissionOperation)
                {
                    _logger.LogWarning(
                        "Acesso negado. usuário {UserId} nÃ£o possui permissÃ£o para operaÃ§Ã£o {Operation} no mÃ³dulo {Module} no tenant {Tenant}",
                        userId, requiredOperation, requiredModuleName, tenantSlug);
                    context.Result = new ForbidResult();
                    return;
                }

                // Log informativo (auditoria)
                _logger.LogInformation(
                    "Acesso permitido. usuário {UserId} possui permissÃ£o para {Module}/{Operation} no tenant {Tenant}",
                    userId, requiredModuleName, requiredOperation, tenantSlug);

                await next();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao validar permissÃ£o {Module}/{Operation} para usuário {UserId}", requiredModuleName, requiredOperation, userId);
                context.Result = new ObjectResult(ResponseBuilder<bool>
                        .Fail(new ErrorDTO { Message = "Erro na validaÃ§Ã£o de permissÃ£o" })
                        .WithException(ex)
                        .WithCode(500)
                        .Build())
                {
                    StatusCode = 500
                };
            }
        }
    }
}
