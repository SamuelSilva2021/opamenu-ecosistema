using OpaMenu.Commons.Api.Commons;
using OpaMenu.Domain.DTOs.MultiTenant;

namespace OpaMenu.Application.Services.Interfaces.Opamenu;

public interface IPlanAdminService
{
    Task<PlanListResponseDto> GetAllAsync(int page, int pageSize, string? name = null, string? status = null);
    Task<ApiResponseDto<PlanDto>> GetByIdAsync(Guid id);
    Task<IEnumerable<object>> GetActiveAsync();
    Task<PlanDto> CreateAsync(CreatePlanRequestDto request);
    Task<PlanDto> UpdateAsync(Guid id, UpdatePlanRequestDto request);
    Task<bool> DeleteAsync(Guid id);
}
