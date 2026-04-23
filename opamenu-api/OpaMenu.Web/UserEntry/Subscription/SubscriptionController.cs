using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OpaMenu.Application.DTOs;
using OpaMenu.Commons.Api.DTOs;
using OpaMenu.Domain.DTOs.Subscription;
using OpaMenu.Web.UserEntry;
using OpaMenu.Infrastructure.Anotations;
using OpaMenu.Infrastructure.Filters;
using OpaMenu.Application.Services.Interfaces.Opamenu;
using Microsoft.EntityFrameworkCore;
using OpaMenu.Domain.DTOs.MultiTenant;
using OpaMenu.Infrastructure.Shared.Data.Context.MultTenant;

namespace OpaMenu.Web.UserEntry.Subscription
{
    [ApiController]
    [Route("api/subscription")]
    [Authorize]
    [ServiceFilter(typeof(PermissionAuthorizationFilter))]
    public class SubscriptionController(ISubscriptionService subscriptionService) : BaseController
    {
        [HttpGet("tenant/{tenantId:guid}")]
        [Authorize(Roles = "SUPER_ADMIN")]
        public async Task<ActionResult<SubscriptionDto>> GetByTenant(
            [FromServices] MultiTenantDbContext multiTenantDbContext,
            [FromRoute] Guid tenantId)
        {
            var subscription = await multiTenantDbContext.Subscriptions
                .AsNoTracking()
                .Include(s => s.Plan)
                .Include(s => s.Tenant)
                .OrderByDescending(s => s.UpdatedAt ?? s.CreatedAt)
                .FirstOrDefaultAsync(s => s.TenantId == tenantId);

            if (subscription == null)
            {
                return Ok(new SubscriptionDto
                {
                    Id = Guid.Empty,
                    TenantId = tenantId,
                    ProductId = Guid.Empty,
                    PlanId = Guid.Empty,
                    Status = string.Empty,
                    TrialEndsAt = null,
                    CurrentPeriodStart = DateTime.UtcNow,
                    CurrentPeriodEnd = DateTime.UtcNow,
                    CancelAtPeriodEnd = false,
                    CancelledAt = null,
                    CustomPricing = null,
                    UsageLimits = null,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = null,
                    Tenant = null,
                    Plan = null
                });
            }

            return Ok(new SubscriptionDto
            {
                Id = subscription.Id,
                TenantId = subscription.TenantId,
                ProductId = subscription.ProductId,
                PlanId = subscription.PlanId,
                Status = subscription.Status.ToString(),
                TrialEndsAt = subscription.TrialEndsAt,
                CurrentPeriodStart = subscription.CurrentPeriodStart,
                CurrentPeriodEnd = subscription.CurrentPeriodEnd,
                CancelAtPeriodEnd = subscription.CancelAtPeriodEnd,
                CancelledAt = subscription.CancelledAt,
                CustomPricing = subscription.CustomPricing,
                UsageLimits = subscription.UsageLimits,
                CreatedAt = subscription.CreatedAt,
                UpdatedAt = subscription.UpdatedAt,
                Tenant = subscription.Tenant == null
                    ? null
                    : new TenantSummaryDto
                    {
                        Id = subscription.Tenant.Id,
                        Name = subscription.Tenant.Name,
                        Slug = subscription.Tenant.Slug,
                        Domain = subscription.Tenant.Domain,
                        Status = subscription.Tenant.Status.ToString(),
                        Email = subscription.Tenant.Email,
                        Phone = subscription.Tenant.Phone,
                        CreatedAt = subscription.Tenant.CreatedAt,
                        UpdatedAt = subscription.Tenant.UpdatedAt,
                        ActiveSubscriptionId = subscription.Tenant.ActiveSubscriptionId
                    },
                Plan = subscription.Plan == null
                    ? null
                    : new SubscriptionPlanDto
                    {
                        Id = subscription.Plan.Id,
                        Name = subscription.Plan.Name,
                        Slug = subscription.Plan.Slug,
                        Description = subscription.Plan.Description,
                        Price = subscription.Plan.Price,
                        BillingCycle = subscription.Plan.BillingCycle.ToString()
                    }
            });
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
