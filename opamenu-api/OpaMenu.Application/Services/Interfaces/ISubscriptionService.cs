using OpaMenu.Application.DTOs;
using OpaMenu.Commons.Api.DTOs;
using OpaMenu.Domain.DTOs.Subscription;
using System.Threading.Tasks;

namespace OpaMenu.Application.Services.Interfaces
{
    public interface ISubscriptionService
    {
        Task<ResponseDTO<SubscriptionStatusResponseDto>> GetCurrentSubscriptionStatusAsync();
        Task<ResponseDTO<bool>> CancelSubscriptionAsync(CancelSubscriptionRequestDto request);
        Task<ResponseDTO<bool>> ChangePlanAsync(ChangePlanRequestDto request);
        Task<ResponseDTO<string>> GetBillingPortalUrlAsync();
    }
}