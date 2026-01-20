using System;
using System.Threading.Tasks;
using OpaMenu.Commons.Api.DTOs;
using OpaMenu.Domain.DTOs.Loyalty;

namespace OpaMenu.Application.Services.Interfaces;

public interface ILoyaltyService
{
    Task<ResponseDTO<LoyaltyProgramDto>> GetProgramAsync(Guid tenantId);
    Task<ResponseDTO<LoyaltyProgramDto>> UpsertProgramAsync(Guid tenantId, CreateLoyaltyProgramDto dto);
    Task<ResponseDTO<CustomerLoyaltySummaryDto>> GetCustomerBalanceAsync(Guid tenantId, string customerPhone);
    Task ProcessOrderPointsAsync(int orderId, Guid tenantId);
    Task<ResponseDTO<LoyaltyProgramDto>> ToggleStatus(Guid tenantId, int id, bool status);
}
