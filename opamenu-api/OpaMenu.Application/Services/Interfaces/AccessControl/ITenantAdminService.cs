using OpaMenu.Commons.Api.DTOs;
using OpaMenu.Domain.DTOs.AccessControl;
using OpaMenu.Domain.DTOs.MultiTenant;

namespace OpaMenu.Application.Services.Interfaces.AccessControl;

public interface ITenantAdminService
{
    Task<(ResponseDTO<RegisterTenantResponseDto> Body, int StatusCode)> RegisterAsync(RegisterTenantRequestDto request);

    Task<PagedResultDto<TenantSummaryDto>> GetTenantsAsync(
        int page,
        int limit,
        string? filterName,
        string? filterSlug,
        string? filterDomain,
        string? filterEmail,
        string? filterPhone,
        string? filterStatus,
        string? filterType = null,
        Guid? filterParentTenantId = null);

    Task<TenantDto?> GetByIdAsync(Guid id);
    Task<(TenantDto? Tenant, bool NotFound, bool BadRequest)> UpdateAsync(Guid id, UpdateTenantRequestDto request);
    Task DeleteAsync(Guid id);

    Task<List<ModuleDto>> GetTenantModulesAsync(Guid tenantId);
    Task<(bool Success, bool NotFound, bool BadRequest)> AddTenantModuleAsync(Guid tenantId, Guid moduleId);
    Task<bool> RemoveTenantModuleAsync(Guid tenantId, Guid moduleId);
}

