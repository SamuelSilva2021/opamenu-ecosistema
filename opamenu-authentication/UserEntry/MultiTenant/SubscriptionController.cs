using Microsoft.AspNetCore.Mvc;
using Authenticator.API.Core.Application.Interfaces.MultiTenant;
using Authenticator.API.Core.Domain.Api;
using Authenticator.API.Core.Domain.MultiTenant.Subscriptions.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace Authenticator.API.UserEntry.MultiTenant
{
    [Route("api/subscription")]
    [ApiController]
    public class SubscriptionController(
        ISubscriptionService subscriptionService
        ) : BaseController
    {
        [HttpPost("checkout/{planId}")]
        [Authorize]
        public async Task<ActionResult<ResponseDTO<string>>> CreateCheckoutSession(Guid planId)
        {
            var result = await subscriptionService.CreateCheckoutSessionAsync(planId);
            return BuildResponse(result);
        }

        [HttpPost("activate-trial/{planId}")]
        [Authorize]
        public async Task<ActionResult<ResponseDTO<string>>> ActivatePlan(Guid planId)
        {
            var result = await subscriptionService.ActivatePlanAsync(planId);
            return BuildResponse(result);
        }

        [HttpGet("current")]
        [Authorize]
        public async Task<ActionResult<ResponseDTO<SubscriptionDTO>>> GetCurrent()
        {
            var result = await subscriptionService.GetCurrentSubscriptionAsync();
            return BuildResponse(result);
        }

        [HttpGet("tenant/{tenantId:guid}")]
        [Authorize(Roles = "SUPER_ADMIN")]
        public async Task<ActionResult<ResponseDTO<SubscriptionDTO>>> GetByTenant(Guid tenantId)
        {
            var result = await subscriptionService.GetByTenantAsync(tenantId);
            return BuildResponse(result);
        }

        // Endpoints administrativos podem ser adicionados aqui (Create, Update, Cancel)
    }
}
