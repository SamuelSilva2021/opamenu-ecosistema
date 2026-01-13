using Authenticator.API.Core.Domain.Api;
using Authenticator.API.Core.Domain.MultiTenant.Plan.DTOs;

namespace Authenticator.API.Core.Application.Interfaces.MultiTenant;

public interface IPlanService
{
    Task<ResponseDTO<PlanDTO>> CreateAsync(CreatePlanDTO dto);
    Task<ResponseDTO<PlanDTO>> UpdateAsync(Guid id, UpdatePlanDTO dto);
    Task<ResponseDTO<bool>> DeleteAsync(Guid id);
    Task<ResponseDTO<PlanDTO>> GetByIdAsync(Guid id);
    Task<ResponseDTO<PlanDTO>> GetBySlugAsync(string slug);
    Task<ResponseDTO<PagedResponseDTO<PlanDTO>>> GetAllAsync(PlanFilterDTO filter);
    Task<ResponseDTO<List<PlanSummaryDTO>>> GetActivePlansAsync();
}
