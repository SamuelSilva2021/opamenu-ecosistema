using Authenticator.API.Core.Domain.Api;
using OpaMenu.Infrastructure.Shared.Entities.MultiTenant.Subscription;

namespace Authenticator.API.Core.Application.Interfaces.Payment
{
    public interface IPaymentGatewayService
    {
        Task<string> CreateCheckoutSessionAsync(SubscriptionEntity subscription, decimal amount, string successUrl, string cancelUrl);
        Task<bool> VerifyPaymentAsync(string sessionId);
    }
}

