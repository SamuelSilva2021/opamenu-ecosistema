using Authenticator.API.Core.Application.Interfaces.Payment;
using OpaMenu.Infrastructure.Shared.Entities.MultiTenant.Subscription;

namespace Authenticator.API.Infrastructure.Services.Payment
{
    public class MockPaymentGatewayService : IPaymentGatewayService
    {
        public Task<string> CreateCheckoutSessionAsync(SubscriptionEntity subscription, decimal amount, string successUrl, string cancelUrl)
        {
            // Simula a criaÃ§Ã£o de uma sessÃ£o de checkout retornando uma URL fictÃ­cia
            // Em produÃ§Ã£o, aqui chamaria Stripe/Pagar.me
            var sessionId = Guid.NewGuid().ToString();
            return Task.FromResult($"https://mock-payment-gateway.com/checkout/{sessionId}?success={successUrl}&cancel={cancelUrl}");
        }

        public Task<bool> VerifyPaymentAsync(string sessionId)
        {
            // Simula verificação de pagamento sempre bem sucedida
            return Task.FromResult(true);
        }
    }
}

