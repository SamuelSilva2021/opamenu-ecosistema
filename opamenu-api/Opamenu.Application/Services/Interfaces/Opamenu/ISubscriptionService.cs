using Opamenu.Application.DTOs;
using Opamenu.Commons.Api.DTOs;
using Opamenu.Domain.DTOs.Subscription;
using System.Threading.Tasks;

namespace Opamenu.Application.Services.Interfaces.Opamenu
{
    public interface ISubscriptionService
    {
        Task<ResponseDTO<SubscriptionStatusResponseDto>> GetCurrentSubscriptionStatusAsync();
        Task<ResponseDTO<bool>> CancelSubscriptionAsync(CancelSubscriptionRequestDto request);
        Task<ResponseDTO<bool>> ChangePlanAsync(ChangePlanRequestDto request);
        Task<ResponseDTO<string>> GetBillingPortalUrlAsync();
    }
}