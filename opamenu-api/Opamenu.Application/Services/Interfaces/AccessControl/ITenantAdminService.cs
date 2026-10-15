using Opamenu.Commons.Api.DTOs;
using Opamenu.Domain.DTOs.AccessControl;
using Opamenu.Domain.DTOs.MultiTenant;

namespace Opamenu.Application.Services.Interfaces.AccessControl;

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

