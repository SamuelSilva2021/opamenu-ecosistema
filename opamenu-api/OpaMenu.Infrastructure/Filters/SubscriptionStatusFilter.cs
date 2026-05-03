using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Http;
using OpaMenu.Infrastructure.Shared.Interfaces;

namespace OpaMenu.Infrastructure.Filters
{
    /// <summary>
    /// Filtro para bloquear acesso se a assinatura do tenant não estiver ativa
    /// </summary>
    public class SubscriptionStatusFilter(ITenantContext tenantContext) : IAsyncActionFilter
    {
        private readonly ITenantContext _tenantContext = tenantContext;

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            // Ignorar se não houver tenant (ex: endpoints públicos de login, registro, etc)
            if (!_tenantContext.HasTenant)
            {
                await next();
                return;
            }

            // Lista de caminhos permitidos mesmo com assinatura inativa
            var allowedPaths = new[] 
            { 
                "/api/subscription", 
                "/api/payments", 
                "/api/auth",
                "/health"
            };

            var path = context.HttpContext.Request.Path.Value?.ToLower() ?? "";
            if (allowedPaths.Any(p => path.Contains(p)))
            {
                await next();
                return;
            }

            if (!_tenantContext.IsSubscriptionActive)
            {
                context.Result = new ObjectResult(new 
                { 
                    message = "Assinatura inativa ou vencida. Por favor, regularize seu pagamento para continuar usando o sistema.",
                    code = "SUBSCRIPTION_REQUIRED",
                    requiresPayment = true
                })
                {
                    StatusCode = StatusCodes.Status402PaymentRequired
                };
                return;
            }

            await next();
        }
    }
}
