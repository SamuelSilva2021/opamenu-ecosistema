using OpaMenu.Application.DTOs;
using OpaMenu.Commons.Api.DTOs;
using OpaMenu.Domain.DTOs.TenantPaymentMethod;

namespace OpaMenu.Application.Services.Interfaces.Opamenu;

public interface ITenantPaymentMethodService
{
    Task<ResponseDTO<IEnumerable<TenantPaymentMethodResponseDto>>> GetAllByTenantAsync();
    Task<ResponseDTO<TenantPaymentMethodResponseDto?>> GetByIdAsync(Guid id);
    Task<ResponseDTO<TenantPaymentMethodResponseDto>> CreateAsync(CreateTenantPaymentMethodRequestDto dto);
    Task<ResponseDTO<TenantPaymentMethodResponseDto>> UpdateAsync(Guid id, UpdateTenantPaymentMethodRequestDto dto);
    Task<ResponseDTO<bool>> DeleteAsync(Guid id);
    Task<ResponseDTO<bool>> ToggleActiveAsync(Guid id);
    Task<ResponseDTO<bool>> ReorderAsync(List<Guid> orderedIds);
}
