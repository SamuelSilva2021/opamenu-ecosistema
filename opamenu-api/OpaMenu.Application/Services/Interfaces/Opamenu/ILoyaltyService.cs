using System;
using System.Threading.Tasks;
using OpaMenu.Commons.Api.DTOs;
using OpaMenu.Domain.DTOs.Loyalty;

namespace OpaMenu.Application.Services.Interfaces.Opamenu;

public interface ILoyaltyService
{
    Task<ResponseDTO<LoyaltyProgramDto>> GetProgramAsync(Guid tenantId);
    Task<ResponseDTO<IEnumerable<LoyaltyProgramDto>>> GetAllProgramsAsync(Guid tenantId);
    Task<ResponseDTO<LoyaltyProgramDto>> CreateProgramAsync(Guid tenantId, CreateLoyaltyProgramDto dto);
    Task<ResponseDTO<LoyaltyProgramDto>> UpdateProgramAsync(Guid tenantId, Guid programId, CreateLoyaltyProgramDto dto);
    Task<ResponseDTO<CustomerLoyaltySummaryDto>> GetCustomerBalanceAsync(Guid tenantId, string customerPhone);
    Task ProcessOrderPointsAsync(Guid orderId, Guid tenantId);
    Task<ResponseDTO<LoyaltyProgramDto>> ToggleStatus(Guid tenantId, Guid id, bool status);
    Task<ResponseDTO<bool>> DeleteProgramAsync(Guid tenantId, Guid id);
    Task<ResponseDTO<bool>> RedeemPointsAsync(Guid tenantId, RedeemLoyaltyPointsDto dto);
}
