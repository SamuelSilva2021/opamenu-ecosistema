using AutoMapper;
using OpaMenu.Application.Services.Interfaces.Opamenu;
using OpaMenu.Commons.Api.Commons;
using OpaMenu.Commons.Api.DTOs;
using OpaMenu.Domain.DTOs.Subscription;
using OpaMenu.Domain.Interfaces;
using OpaMenu.Infrastructure.Shared.Entities.MultiTenant.Plan;
using System;
using System.Threading.Tasks;

namespace OpaMenu.Application.Services.Opamenu
{
    public class SubscriptionService(
        ISubscriptionRepository subscriptionRepository,
        IPlanRepository planRepository,
        ICurrentUserService currentUserService,
        IMapper mapper) : ISubscriptionService
    {
        public async Task<ResponseDTO<SubscriptionStatusResponseDto>> GetCurrentSubscriptionStatusAsync()
        {
            try
            {
                var tenantId = currentUserService.GetTenantGuid();
                if (tenantId == null)
                    return StaticResponseBuilder<SubscriptionStatusResponseDto>.BuildError("Tenant não identificado.");

                var subscription = await subscriptionRepository.GetActiveSubscriptionAsync(tenantId.Value);
                
                if (subscription == null)
                    return StaticResponseBuilder<SubscriptionStatusResponseDto>.BuildError("Nenhuma assinatura ativa encontrada para este tenant.");

                var dto = mapper.Map<SubscriptionStatusResponseDto>(subscription);
                
                return StaticResponseBuilder<SubscriptionStatusResponseDto>.BuildOk(dto);
            }
            catch (Exception ex)
            {
                return StaticResponseBuilder<SubscriptionStatusResponseDto>.BuildErrorResponse(ex);
            }
        }

        public async Task<ResponseDTO<bool>> CancelSubscriptionAsync(CancelSubscriptionRequestDto request)
        {
            try
            {
                var tenantId = currentUserService.GetTenantGuid();
                if (tenantId == null)
                    return StaticResponseBuilder<bool>.BuildError("Tenant não identificado.");

                var subscription = await subscriptionRepository.GetActiveSubscriptionAsync(tenantId.Value);
                if (subscription == null)
                    return StaticResponseBuilder<bool>.BuildError("Nenhuma assinatura ativa encontrada.");

                subscription.CancelAtPeriodEnd = true;
                subscription.UpdatedAt = DateTime.UtcNow;
                // Poderíamos salvar o motivo do cancelamento em algum log ou campo de observação se existisse

                await subscriptionRepository.UpdateAsync(subscription);

                return StaticResponseBuilder<bool>.BuildOk(true);
            }
            catch (Exception ex)
            {
                return StaticResponseBuilder<bool>.BuildErrorResponse(ex);
            }
        }

        public async Task<ResponseDTO<bool>> ChangePlanAsync(ChangePlanRequestDto request)
        {
            try
            {
                var tenantId = currentUserService.GetTenantGuid();
                if (tenantId == null)
                    return StaticResponseBuilder<bool>.BuildError("Tenant não identificado.");

                var subscription = await subscriptionRepository.GetActiveSubscriptionAsync(tenantId.Value);
                if (subscription == null)
                    return StaticResponseBuilder<bool>.BuildError("Nenhuma assinatura ativa encontrada.");

                var newPlan = await planRepository.GetByIdAsync(request.NewPlanId);
                if (newPlan == null || newPlan.Status != EPlanStatus.Ativo)
                    return StaticResponseBuilder<bool>.BuildError("Plano inválido ou inativo.");

                if (subscription.PlanId == newPlan.Id)
                    return StaticResponseBuilder<bool>.BuildError("A assinatura já está neste plano.");

                // TODO: Integrar com gateway de pagamento para pro-rata e cobrança
                subscription.PlanId = newPlan.Id;
                subscription.UpdatedAt = DateTime.UtcNow;
                
                // Se estava cancelado, reativa ao mudar de plano? Depende da regra de negócio.
                // Vamos manter o status atual, mas remover flag de cancelamento se houver upgrade explícito
                subscription.CancelAtPeriodEnd = false; 

                await subscriptionRepository.UpdateAsync(subscription);

                return StaticResponseBuilder<bool>.BuildOk(true);
            }
            catch (Exception ex)
            {
                return StaticResponseBuilder<bool>.BuildErrorResponse(ex);
            }
        }

        public async Task<ResponseDTO<string>> GetBillingPortalUrlAsync()
        {
            // Simulação de retorno de URL de portal de pagamento (ex: Stripe Customer Portal)
            // Em produção, isso chamaria a API do gateway para gerar uma sessão temporária
            return await Task.FromResult(StaticResponseBuilder<string>.BuildOk("https://billing.opamenu.com.br/portal/session_mock_123"));
        }
    }
}
