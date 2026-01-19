using Authenticator.API.Core.Application.Interfaces.Auth;
using Authenticator.API.Core.Application.Interfaces.MultiTenant;
using Authenticator.API.Core.Application.Interfaces.Payment;
using Authenticator.API.Core.Domain.Api;
using Authenticator.API.Core.Domain.Api.Commons;
using OpaMenu.Infrastructure.Shared.Entities.MultiTenant.Subscription;
using Authenticator.API.Core.Domain.MultiTenant.Subscriptions.DTOs;
using OpaMenu.Infrastructure.Shared.Entities.MultiTenant.Tenant; // For ETenantStatus if needed
using AutoMapper;

namespace Authenticator.API.Core.Application.Implementation.MultiTenant;

public class SubscriptionService(
    ISubscriptionRepository subscriptionRepository,
    ITenantRepository tenantRepository,
    IPlanRepository planRepository,
    IPaymentGatewayService paymentGatewayService,
    IUserContext userContext,
    ITenantProductRepository tenantProductRepository,
    IMapper mapper,
    ILogger<SubscriptionService> logger) : ISubscriptionService
{
    public async Task<ResponseDTO<string>> CreateCheckoutSessionAsync(Guid planId)
    {
        try
        {
            var tenantId = userContext.CurrentUser?.TenantId;
            if (tenantId == null || tenantId == Guid.Empty)
            {
                return StaticResponseBuilder<string>.BuildError("Tenant nÃ£o identificado no contexto do usuário");
            }

            var subscription = await subscriptionRepository.GetByTenantIdAsync(tenantId.Value);
            
            if (subscription == null)
            {
                subscription = new SubscriptionEntity
                {
                    Id = Guid.NewGuid(),
                    TenantId = tenantId.Value,
                    PlanId = planId,
                    Status = ESubscriptionStatus.Pendente,
                    CreatedAt = DateTime.UtcNow
                };
                await subscriptionRepository.AddAsync(subscription);
            }
            else 
            {
                // Atualiza o plano da assinatura existente
                subscription.PlanId = planId;
                await subscriptionRepository.UpdateAsync(subscription);
            }

            var plan = await planRepository.GetByIdAsync(planId);
            if (plan == null)
            {
                return StaticResponseBuilder<string>.BuildError("Plano nÃ£o encontrado");
            }

            var checkoutUrl = await paymentGatewayService.CreateCheckoutSessionAsync(
                subscription, 
                plan.Price, 
                $"http://localhost:5173/payment/success?session_id={{CHECKOUT_SESSION_ID}}",
                $"http://localhost:5173/payment/cancel"
            );

            return StaticResponseBuilder<string>.BuildOk(checkoutUrl);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro ao criar sessÃ£o de checkout");
            return StaticResponseBuilder<string>.BuildError("Erro interno ao criar sessÃ£o de checkout");
        }
    }

    public async Task<ResponseDTO<string>> ActivatePlanAsync(Guid planId)
    {
        try
        {
            var tenantId = userContext.CurrentUser?.TenantId;
            if (tenantId == null || tenantId == Guid.Empty)
                StaticResponseBuilder<string>.BuildError("Tenant nÃ£o identificado no contexto do usuário");

            var subscription = await subscriptionRepository.GetByTenantIdAsync(tenantId.Value);

            var plan = await planRepository.GetByIdAsync(planId);

            if (subscription == null)
            {
                var product = await tenantProductRepository.GetDefaultProductAsync();
                if (product == null)
                {
                    var products = await tenantProductRepository.GetAllAsync();
                    product = products.FirstOrDefault();
                    
                    if (product == null)
                        return StaticResponseBuilder<string>.BuildError("Nenhum produto configurado no sistema");
                }

                subscription = new SubscriptionEntity
                {
                    Id = Guid.NewGuid(),
                    TenantId = tenantId.Value,
                    ProductId = product.Id,
                    PlanId = planId,
                    Status = plan!.IsTrial ? ESubscriptionStatus.Trial : ESubscriptionStatus.Ativo,
                    CreatedAt = DateTime.UtcNow,
                    TrialEndsAt = plan!.IsTrial ? DateTime.UtcNow.AddDays(plan!.TrialPeriodDays) : null
                };
                await subscriptionRepository.AddAsync(subscription);
            }
            else
            {
                subscription.PlanId = planId;
                subscription.Status = ESubscriptionStatus.Ativo;
                subscription.TrialEndsAt = DateTime.UtcNow.AddDays(14);
                await subscriptionRepository.UpdateAsync(subscription);
            }

            if (subscription.TrialEndsAt == null || subscription.TrialEndsAt < DateTime.UtcNow)
                return StaticResponseBuilder<string>.BuildError("Plano selecionado nÃ£o Ã© elegÃ­vel para trial");

            // Ativar Tenant
            var tenant = await tenantRepository.GetByIdAsync(tenantId.Value);
            if (tenant != null)
            {
                tenant.Status = ETenantStatus.Ativo;
                await tenantRepository.UpdateAsync(tenant);
            }
            
            return StaticResponseBuilder<string>.BuildOk("Plano ativado com sucesso");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro ao ativar trial");
            return StaticResponseBuilder<string>.BuildError("Erro interno ao ativar plano");
        }
    }

    public async Task<ResponseDTO<SubscriptionDTO>> GetCurrentSubscriptionAsync()
    {
        var tenantId = userContext.CurrentUser?.TenantId;
        if (tenantId == null || tenantId == Guid.Empty)
            return StaticResponseBuilder<SubscriptionDTO>.BuildError("Tenant nÃ£o identificado no contexto do usuário");

        var subscription = await subscriptionRepository.GetByTenantIdAsync(tenantId.Value);
        if (subscription == null)
            return StaticResponseBuilder<SubscriptionDTO>.BuildOk(new SubscriptionDTO { });

        var subscriptionDto = mapper.Map<SubscriptionDTO>(subscription);
        return StaticResponseBuilder<SubscriptionDTO>.BuildOk(subscriptionDto);
    }

    public async Task<ResponseDTO<SubscriptionDTO>> GetByTenantAsync(Guid tenantId)
    {
        try
        {
            if (tenantId == Guid.Empty)
                return StaticResponseBuilder<SubscriptionDTO>.BuildError("Tenant inválido");

            var subscription = await subscriptionRepository.GetByTenantIdAsync(tenantId);
            if (subscription == null)
                return StaticResponseBuilder<SubscriptionDTO>.BuildOk(new SubscriptionDTO { });

            var dto = mapper.Map<SubscriptionDTO>(subscription);
            return StaticResponseBuilder<SubscriptionDTO>.BuildOk(dto);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro ao buscar assinatura para o tenant {TenantId}", tenantId);
            return StaticResponseBuilder<SubscriptionDTO>.BuildError("Erro ao buscar assinatura do tenant");
        }
    }

    public async Task<ResponseDTO<SubscriptionDTO>> CreateAsync(CreateSubscriptionDTO dto)
    {
        try 
        {
            var entity = mapper.Map<SubscriptionEntity>(dto);            
            var created = await subscriptionRepository.AddAsync(entity);

            var dtoResult = mapper.Map<SubscriptionDTO>(created);

            return StaticResponseBuilder<SubscriptionDTO>.BuildOk(dtoResult);
        }
        catch(Exception ex)
        {
            logger.LogError(ex, "Erro ao criar assinatura");
            return StaticResponseBuilder<SubscriptionDTO>.BuildError("Erro ao criar assinatura");
        }
    }

    public async Task<ResponseDTO<SubscriptionDTO>> UpdateAsync(Guid id, UpdateSubscriptionDTO dto)
    {
        try
        {
            var entity = await subscriptionRepository.GetByIdAsync(id);
            if (entity == null) return StaticResponseBuilder<SubscriptionDTO>.BuildNotFound(new SubscriptionDTO { });

            mapper.Map(dto, entity);
            entity.UpdatedAt = DateTime.UtcNow;
            
            await subscriptionRepository.UpdateAsync(entity);

            var dtoResult = mapper.Map<SubscriptionDTO>(entity);
            return StaticResponseBuilder<SubscriptionDTO>.BuildOk(dtoResult);
        }
        catch(Exception ex)
        {
            logger.LogError(ex, "Erro ao atualizar assinatura");
            return StaticResponseBuilder<SubscriptionDTO>.BuildError("Erro ao atualizar assinatura");
        }
    }

    public async Task<ResponseDTO<bool>> CancelAsync(Guid id)
    {
        try
        {
            var entity = await subscriptionRepository.GetByIdAsync(id);
            if (entity == null) return StaticResponseBuilder<bool>.BuildNotFound(false);

            entity.Status = ESubscriptionStatus.Cancelado;
            entity.UpdatedAt = DateTime.UtcNow;

            await subscriptionRepository.UpdateAsync(entity);

            return StaticResponseBuilder<bool>.BuildOk(true);
        }
        catch(Exception ex)
        {
            logger.LogError(ex, "Erro ao cancelar assinatura");
            return StaticResponseBuilder<bool>.BuildError("Erro ao cancelar assinatura");
        }
    }
}

