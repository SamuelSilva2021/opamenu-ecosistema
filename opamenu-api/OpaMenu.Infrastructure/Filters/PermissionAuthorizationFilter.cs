using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using OpaMenu.Domain.Interfaces;
using OpaMenu.Infrastructure.Anotations;
using Microsoft.AspNetCore.Http;

namespace OpaMenu.Infrastructure.Filters
{
    public class PermissionAuthorizationFilter : IAsyncActionFilter
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly ILogger<PermissionAuthorizationFilter> _logger;

        public PermissionAuthorizationFilter(
            ICurrentUserService currentUserService,
            ILogger<PermissionAuthorizationFilter> logger)
        {
            _currentUserService = currentUserService;
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
