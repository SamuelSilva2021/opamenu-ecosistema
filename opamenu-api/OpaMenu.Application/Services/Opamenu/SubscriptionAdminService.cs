using Microsoft.Extensions.Caching.Distributed;
using OpaMenu.Application.Services.Interfaces.Opamenu;
using OpaMenu.Commons.Api.Commons;
using OpaMenu.Commons.Api.DTOs;
using OpaMenu.Domain.DTOs.MultiTenant;
using OpaMenu.Domain.Interfaces;
using OpaMenu.Infrastructure.Shared.Entities.MultiTenant.Plan;
using OpaMenu.Infrastructure.Shared.Entities.MultiTenant.Subscription;
using OpaMenu.Infrastructure.Shared.Entities.MultiTenant.Tenant;
using OpaMenu.Infrastructure.Shared.Entities.MultiTenant.TenantModule;

namespace OpaMenu.Application.Services.Opamenu;

public sealed class SubscriptionAdminService(
    IPlanRepository planRepository,
    IPlanModuleRepository planModuleRepository,
    IMultiTenantProductRepository productRepository,
    ITenantRepository tenantRepository,
    ITenantModuleRepository tenantModuleRepository,
    ISubscriptionRepository subscriptionRepository,
    ICurrentUserService currentUserService,
    IDistributedCache cache) : ISubscriptionAdminService
{
    private readonly IPlanRepository _planRepository = planRepository;
    private readonly IPlanModuleRepository _planModuleRepository = planModuleRepository;
    private readonly IMultiTenantProductRepository _productRepository = productRepository;
    private readonly ITenantRepository _tenantRepository = tenantRepository;
    private readonly ITenantModuleRepository _tenantModuleRepository = tenantModuleRepository;
    private readonly ISubscriptionRepository _subscriptionRepository = subscriptionRepository;
    private readonly ICurrentUserService _currentUserService = currentUserService;
    private readonly IDistributedCache _cache = cache;

    public async Task<(ResponseDTO<string> Body, int StatusCode)> ActivatePlanAsync(Guid planId, Guid? tenantId = null)
    {
        try
        {
            // Se tenantId for fornecido (via Admin), usa ele. Caso contrário, pega do contexto do usuário logado.
            tenantId ??= _currentUserService.GetTenantGuid();

            if (!tenantId.HasValue || tenantId.Value == Guid.Empty)
            {
                return (StaticResponseBuilder<string>.BuildError("Tenant não identificado."), 400);
            }

            var plan = await _planRepository.GetByIdAsync(planId);
            if (plan == null || plan.Status != EPlanStatus.Ativo)
            {
                return (StaticResponseBuilder<string>.BuildError("Plano inválido ou inativo."), 400);
            }

            var tenant = await _tenantRepository.GetByIdTrackedAsync(tenantId.Value);
            if (tenant == null)
            {
                return (StaticResponseBuilder<string>.BuildError("Tenant não encontrado."), 400);
            }

            var now = DateTime.UtcNow;
            var subscription = await _subscriptionRepository.GetByTenantIdTrackedAsync(tenantId.Value);

            if (plan.IsTrial && subscription != null && subscription.TrialEndsAt.HasValue)
            {
                if (subscription.TrialEndsAt.Value <= DateTime.UtcNow)
                {
                    return (StaticResponseBuilder<string>.BuildError("O período de teste (trial) já foi utilizado por esta conta e não pode ser renovado."), 400);
                }
            }

            if (subscription == null)
            {
                var product = await _productRepository.GetFirstAsync();
                if (product == null)
                {
                    return (StaticResponseBuilder<string>.BuildError("Nenhum produto configurado no sistema."), 400);
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

                await _subscriptionRepository.AddAsync(subscription);
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

            await _tenantRepository.SaveChangesAsync();
            await _subscriptionRepository.SaveChangesAsync();

            var planModuleIds = await _planModuleRepository.GetModuleIdsByPlanIdAsync(planId);

            var tenantModules = await _tenantModuleRepository.GetByTenantTrackedAsync(tenantId.Value);
            if (tenantModules.Count > 0)
            {
                await _tenantModuleRepository.RemoveRangeAsync(tenantModules);
            }

            if (planModuleIds.Count > 0)
            {
                var entities = planModuleIds.Distinct().Select(moduleId => new TenantModuleEntity
                {
                    Id = Guid.NewGuid(),
                    TenantId = tenantId.Value,
                    ModuleId = moduleId,
                    IsEnabled = true,
                    Configuration = "{}",
                    CreatedAt = now,
                    UpdatedAt = null
                });

                await _tenantModuleRepository.AddRangeAsync(entities);
            }

            await _tenantModuleRepository.SaveChangesAsync();
            await InvalidatePermissionsCacheAsync(tenantId.Value, _currentUserService.UserId);
            await InvalidateTenantContextCacheAsync(tenantId.Value);

            return (StaticResponseBuilder<string>.BuildOk("Plano ativado com sucesso"), 200);
        }
        catch (Exception ex)
        {
            return (StaticResponseBuilder<string>.BuildErrorResponse(ex), 500);
        }
    }

    public async Task<SubscriptionDto> GetByTenantAsync(Guid tenantId)
    {
        var subscription = await _subscriptionRepository.GetByTenantIdWithPlanAndTenantAsync(tenantId);
        if (subscription == null)
        {
            var now = DateTime.UtcNow;
            return new SubscriptionDto
            {
                Id = Guid.Empty,
                TenantId = tenantId,
                ProductId = Guid.Empty,
                PlanId = Guid.Empty,
                Status = string.Empty,
                TrialEndsAt = null,
                CurrentPeriodStart = now,
                CurrentPeriodEnd = now,
                CancelAtPeriodEnd = false,
                CancelledAt = null,
                CustomPricing = null,
                UsageLimits = null,
                CreatedAt = now,
                UpdatedAt = null,
                Tenant = null,
                Plan = null
            };
        }

        return new SubscriptionDto
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
        };
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

    private async Task InvalidatePermissionsCacheAsync(Guid tenantId, string userIdValue)
    {
        if (!Guid.TryParse(userIdValue, out var userId))
        {
            return;
        }

        await _cache.RemoveAsync($"auth:permissions:{userId}:{tenantId}");
    }

    private async Task InvalidateTenantContextCacheAsync(Guid tenantId)
    {
        await _cache.RemoveAsync($"tenant:context:{tenantId}");
    }
}
