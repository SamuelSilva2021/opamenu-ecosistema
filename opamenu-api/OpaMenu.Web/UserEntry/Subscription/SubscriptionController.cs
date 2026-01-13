using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OpaMenu.Application.DTOs;
using OpaMenu.Application.Services.Interfaces;
using OpaMenu.Domain.DTOs.Subscription;
using OpaMenu.Web.UserEntry;

namespace OpaMenu.Web.UserEntry.Subscription
{
    [ApiController]
    [Route("api/subscription")]
    [Authorize]
    public class SubscriptionController(ISubscriptionService subscriptionService) : BaseController
    {
        [HttpGet("status")]
        public async Task<ActionResult<ResponseDTO<SubscriptionStatusResponseDto>>> GetStatus()
        {
            var resultService = await subscriptionService.GetCurrentSubscriptionStatusAsync();
            return BuildResponse(resultService);
        }

        [HttpPost("cancel")]
        public async Task<ActionResult<ResponseDTO<bool>>> CancelSubscription([FromBody] CancelSubscriptionRequestDto request)
        {
            var resultService = await subscriptionService.CancelSubscriptionAsync(request);
            return BuildResponse(resultService);
        }

        [HttpPost("change-plan")]
        public async Task<ActionResult<ResponseDTO<bool>>> ChangePlan([FromBody] ChangePlanRequestDto request)
        {
            var resultService = await subscriptionService.ChangePlanAsync(request);
            return BuildResponse(resultService);
        }

        [HttpGet("billing-portal")]
        public async Task<ActionResult<ResponseDTO<string>>> GetBillingPortalUrl()
        {
            var resultService = await subscriptionService.GetBillingPortalUrlAsync();
            return BuildResponse(resultService);
        }
    }
}
