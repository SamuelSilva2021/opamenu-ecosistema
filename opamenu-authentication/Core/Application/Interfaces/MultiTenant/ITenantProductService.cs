using Authenticator.API.Core.Domain.Api;
using Authenticator.API.Core.Domain.MultiTenant.TenantProduct.DTOs;

namespace Authenticator.API.Core.Application.Interfaces.MultiTenant;

public interface ITenantProductService
{
    Task<ResponseDTO<TenantProductDTO>> CreateAsync(CreateTenantProductDTO dto);
    Task<ResponseDTO<TenantProductDTO>> UpdateAsync(Guid id, UpdateTenantProductDTO dto);
    Task<ResponseDTO<bool>> DeleteAsync(Guid id);
    Task<ResponseDTO<TenantProductDTO>> GetByIdAsync(Guid id);
    Task<ResponseDTO<List<TenantProductDTO>>> GetAllAsync();
    Task<ResponseDTO<List<TenantProductSummaryDTO>>> GetActiveProductsAsync();
}
