using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using OpaMenu.Application.DTOs;
using OpaMenu.Commons.Api.Commons;
using OpaMenu.Commons.Api.DTOs;
using OpaMenu.Domain.DTOs.Subscription;
using OpaMenu.Domain.Interfaces;
using OpaMenu.Web.UserEntry;
using OpaMenu.Infrastructure.Anotations;
using OpaMenu.Infrastructure.Filters;
using OpaMenu.Application.Services.Interfaces.Opamenu;
using Microsoft.EntityFrameworkCore;
using OpaMenu.Domain.DTOs.MultiTenant;
using OpaMenu.Infrastructure.Shared.Data.Context.MultTenant;
using OpaMenu.Infrastructure.Shared.Entities.MultiTenant.Plan;
using OpaMenu.Infrastructure.Shared.Entities.MultiTenant.PlanModule;
using OpaMenu.Infrastructure.Shared.Entities.MultiTenant.Subscription;
using OpaMenu.Infrastructure.Shared.Entities.MultiTenant.Tenant;
using OpaMenu.Infrastructure.Shared.Entities.MultiTenant.TenantModule;

namespace OpaMenu.Web.UserEntry.Subscription
{
    [ApiController]
    [Route("api/subscription")]
    [Authorize]
    [ServiceFilter(typeof(PermissionAuthorizationFilter))]
    public class SubscriptionController(ISubscriptionService subscriptionService) : BaseController
    {
        [HttpPost("activate/{planId:guid}")]
        public async Task<IActionResult> ActivatePlan(
            [FromServices] MultiTenantDbContext multiTenantDbContext,
            [FromServices] IDistributedCache cache,
            [FromServices] ICurrentUserService currentUserService,
            [FromRoute] Guid planId)
        {
            try
            {
                var tenantId = currentUserService.GetTenantGuid();
                if (!tenantId.HasValue || tenantId.Value == Guid.Empty)
                {
                    return BadRequest(StaticResponseBuilder<string>.BuildError("Tenant não identificado."));
                }

                var plan = await multiTenantDbContext.Plans.AsNoTracking().FirstOrDefaultAsync(p => p.Id == planId);
                if (plan == null || plan.Status != EPlanStatus.Ativo)
                {
                    return BadRequest(StaticResponseBuilder<string>.BuildError("Plano inválido ou inativo."));
                }

                var tenant = await multiTenantDbContext.Tenants.FirstOrDefaultAsync(t => t.Id == tenantId.Value);
                if (tenant == null)
                {
                    return BadRequest(StaticResponseBuilder<string>.BuildError("Tenant não encontrado."));
                }

                var now = DateTime.UtcNow;
                var subscription = await multiTenantDbContext.Subscriptions.FirstOrDefaultAsync(s => s.TenantId == tenantId.Value);

                if (subscription == null)
                {
                    var product = await multiTenantDbContext.Products.AsNoTracking()
                        .OrderBy(p => p.CreatedAt)
                        .FirstOrDefaultAsync();

                    if (product == null)
                    {
                        return BadRequest(StaticResponseBuilder<string>.BuildError("Nenhum produto configurado no sistema."));
                    }

                    subscription = new SubscriptionEntity
                    {
                        Id = Guid.NewGuid(),
                        TenantId = tenantId.Value,
                        ProductId = product.Id,
                        PlanId = planId,
                        Status = plan.IsTrial ? ESubscriptionStatus.Trial : ESubscriptionStatus.Ativo,
                        TrialEndsAt = plan.IsTrial ? now.AddDays(plan.TrialPeriodDays) : null,
                        CurrentPeriodStart = now,
                        CurrentPeriodEnd = ComputePeriodEnd(now, plan),
                        CancelAtPeriodEnd = false,
                        CancelledAt = null,
                        CustomPricing = null,
                        UsageLimits = null,
                        CreatedAt = now,
                        UpdatedAt = null
                    };

                    multiTenantDbContext.Subscriptions.Add(subscription);
                }
                else
                {
                    subscription.PlanId = planId;
                    subscription.Status = plan.IsTrial ? ESubscriptionStatus.Trial : ESubscriptionStatus.Ativo;
                    subscription.TrialEndsAt = plan.IsTrial ? now.AddDays(plan.TrialPeriodDays) : null;
                    subscription.CurrentPeriodStart = now;
                    subscription.CurrentPeriodEnd = ComputePeriodEnd(now, plan);
                    subscription.CancelAtPeriodEnd = false;
                    subscription.CancelledAt = null;
                    subscription.UpdatedAt = now;
                }

                tenant.Status = ETenantStatus.Ativo;
                tenant.ActiveSubscriptionId = subscription.Id;
                tenant.UpdatedAt = now;

                await multiTenantDbContext.SaveChangesAsync();

                var planModuleIds = await multiTenantDbContext.Set<PlanModuleEntity>()
                    .AsNoTracking()
                    .Where(pm => pm.PlanId == planId)
                    .Select(pm => pm.ModuleId)
                    .ToListAsync();

                var tenantModules = await multiTenantDbContext.Set<TenantModuleEntity>()
                    .Where(tm => tm.TenantId == tenantId.Value)
                    .ToListAsync();

                if (tenantModules.Count > 0)
                {
                    multiTenantDbContext.Set<TenantModuleEntity>().RemoveRange(tenantModules);
                }

                if (planModuleIds.Count > 0)
                {
                    foreach (var moduleId in planModuleIds.Distinct())
                    {
                        multiTenantDbContext.Set<TenantModuleEntity>().Add(new TenantModuleEntity
                        {
                            Id = Guid.NewGuid(),
                            TenantId = tenantId.Value,
                            ModuleId = moduleId,
                            IsEnabled = true,
                            Configuration = "{}",
                            CreatedAt = now,
                            UpdatedAt = null
                        });
                    }
                }

                await multiTenantDbContext.SaveChangesAsync();

                await InvalidatePermissionsCacheAsync(cache, tenantId.Value, currentUserService.UserId);

                return Ok(StaticResponseBuilder<string>.BuildOk("Plano ativado com sucesso"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, StaticResponseBuilder<string>.BuildErrorResponse(ex));
            }
        }

        [HttpPost("activate-trial/{planId:guid}")]
        public Task<IActionResult> ActivateTrial(
            [FromServices] MultiTenantDbContext multiTenantDbContext,
            [FromServices] IDistributedCache cache,
            [FromServices] ICurrentUserService currentUserService,
            [FromRoute] Guid planId)
        {
            return ActivatePlan(multiTenantDbContext, cache, currentUserService, planId);
        }

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

        private static DateTime ComputePeriodEnd(DateTime startUtc, PlanEntity plan)
        {
            if (plan.IsTrial && plan.TrialPeriodDays > 0)
            {
                return startUtc.AddDays(plan.TrialPeriodDays);
            }

            return plan.BillingCycle switch
            {
                EBillingCycle.Anual => startUtc.AddYears(1),
                EBillingCycle.Semestral => startUtc.AddMonths(6),
                EBillingCycle.Semanal => startUtc.AddDays(7),
                EBillingCycle.Diario => startUtc.AddDays(1),
                _ => startUtc.AddMonths(1)
            };
        }

        private static async Task InvalidatePermissionsCacheAsync(IDistributedCache cache, Guid tenantId, string userIdValue)
        {
            if (!Guid.TryParse(userIdValue, out var userId))
            {
                return;
            }

            await cache.RemoveAsync($"auth:permissions:{userId}:{tenantId}");
        }
    }
}
