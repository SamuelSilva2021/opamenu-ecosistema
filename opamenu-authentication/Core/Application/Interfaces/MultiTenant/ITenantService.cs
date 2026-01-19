using Authenticator.API.Core.Domain.Api;
using Authenticator.API.Core.Domain.MultiTenant.Tenant.DTOs;

namespace Authenticator.API.Core.Application.Interfaces.MultiTenant
{
    public interface ITenantService
    {
        Task<ResponseDTO<RegisterTenantResponseDTO>> AddTenantAsync(CreateTenantDTO tenant);

        Task<ResponseDTO<PagedResponseDTO<TenantSummaryDTO>>> GetAllAsync(int page, int limit, TenantFilterDTO? filter = null);

        Task<ResponseDTO<TenantDTO>> GetByIdAsync(Guid id);

        Task<ResponseDTO<TenantDTO>> UpdateAsync(Guid id, UpdateTenantDTO dto);

        Task<ResponseDTO<bool>> DeleteAsync(Guid id);
    }
}
