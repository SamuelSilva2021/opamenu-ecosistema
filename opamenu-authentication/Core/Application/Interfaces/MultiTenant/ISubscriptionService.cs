using Authenticator.API.Core.Domain.Api;
using Authenticator.API.Core.Domain.MultiTenant.Subscriptions.DTOs;

namespace Authenticator.API.Core.Application.Interfaces.MultiTenant;

public interface ISubscriptionService
{
    Task<ResponseDTO<string>> CreateCheckoutSessionAsync(Guid planId);
    Task<ResponseDTO<string>> ActivatePlanAsync(Guid planId);
    Task<ResponseDTO<SubscriptionDTO>> GetCurrentSubscriptionAsync();
    Task<ResponseDTO<SubscriptionDTO>> CreateAsync(CreateSubscriptionDTO dto);
    Task<ResponseDTO<SubscriptionDTO>> UpdateAsync(Guid id, UpdateSubscriptionDTO dto);
    Task<ResponseDTO<bool>> CancelAsync(Guid id);
}
