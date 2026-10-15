using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Opamenu.Application.DTOs;
using Opamenu.Commons.Api.Commons;
using Opamenu.Commons.Api.DTOs;
using Opamenu.Domain.DTOs.Subscription;
using Opamenu.Domain.Interfaces;
using Opamenu.Web.UserEntry;
using Opamenu.Infrastructure.Anotations;
using Opamenu.Infrastructure.Filters;
using Opamenu.Application.Services.Interfaces.Opamenu;
using Opamenu.Domain.DTOs.MultiTenant;

namespace Opamenu.Web.UserEntry.Subscription
{
    [ApiController]
    [Route("api/subscription")]
    [Authorize]
    [ServiceFilter(typeof(PermissionAuthorizationFilter))]
    public class SubscriptionController(
        ISubscriptionService subscriptionService,
        ISubscriptionAdminService subscriptionAdminService) : BaseController
    {
        private readonly ISubscriptionAdminService _subscriptionAdminService = subscriptionAdminService;

        [HttpPost("activate/{planId:guid}")]
        public async Task<IActionResult> ActivatePlan([FromRoute] Guid planId)
        {
            var (body, statusCode) = await _subscriptionAdminService.ActivatePlanAsync(planId);
            return statusCode switch
            {
                200 => Ok(body),
                400 => BadRequest(body),
                _ => StatusCode(statusCode, body)
            };
        }

        [HttpPost("activate/{planId:guid}/tenant/{tenantId:guid}")]
        [Authorize(Roles = "SUPER_ADMIN")]
        public async Task<IActionResult> ActivatePlanForTenant([FromRoute] Guid planId, [FromRoute] Guid tenantId)
        {
            var (body, statusCode) = await _subscriptionAdminService.ActivatePlanAsync(planId, tenantId);
            return statusCode switch
            {
                200 => Ok(body),
                400 => BadRequest(body),
                _ => StatusCode(statusCode, body)
            };
        }

        [HttpPost("activate-trial/{planId:guid}")]
        public Task<IActionResult> ActivateTrial(
            [FromRoute] Guid planId)
        {
            return ActivatePlan(planId);
        }

        [HttpGet("tenant/{tenantId:guid}")]
        [Authorize(Roles = "SUPER_ADMIN")]
        public async Task<ActionResult<SubscriptionDto>> GetByTenant(
            [FromRoute] Guid tenantId)
        {
            var result = await _subscriptionAdminService.GetByTenantAsync(tenantId);
            return Ok(result);
        }

        [HttpGet("status")]
        [MapPermission(MODULE_SUBSCRIPTION, OPERATION_SELECT)]
        public async Task<ActionResult<ResponseDTO<SubscriptionStatusResponseDto>>> GetStatus()
        {
            var resultService = await subscriptionService.GetCurrentSubscriptionStatusAsync();
            return BuildResponse(resultService);
        }

        [HttpPost("cancel")]
        [MapPermission(MODULE_SUBSCRIPTION, OPERATION_CANCELLATION)]
        public async Task<ActionResult<ResponseDTO<bool>>> CancelSubscription([FromBody] CancelSubscriptionRequestDto request)
        {
            var resultService = await subscriptionService.CancelSubscriptionAsync(request);
            return BuildResponse(resultService);
        }

        [HttpPost("change-plan")]
        [MapPermission(MODULE_SUBSCRIPTION, OPERATION_UPDATE)]
        public async Task<ActionResult<ResponseDTO<bool>>> ChangePlan([FromBody] ChangePlanRequestDto request)
        {
            var resultService = await subscriptionService.ChangePlanAsync(request);
            return BuildResponse(resultService);
        }

        [HttpGet("billing-portal")]
        [MapPermission(MODULE_SUBSCRIPTION, OPERATION_SELECT)]
        public async Task<ActionResult<ResponseDTO<string>>> GetBillingPortalUrl()
        {
            var resultService = await subscriptionService.GetBillingPortalUrlAsync();
            return BuildResponse(resultService);
        }

    }
}
