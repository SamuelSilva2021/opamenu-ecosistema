using OpaMenu.Application.DTOs;
using OpaMenu.Domain.DTOs.TenantPaymentMethod;

namespace OpaMenu.Application.Services.Interfaces;

public interface ITenantPaymentMethodService
{
    Task<ResponseDTO<IEnumerable<TenantPaymentMethodResponseDto>>> GetAllByTenantAsync();
    Task<ResponseDTO<TenantPaymentMethodResponseDto?>> GetByIdAsync(int id);
    Task<ResponseDTO<TenantPaymentMethodResponseDto>> CreateAsync(CreateTenantPaymentMethodRequestDto dto);
    Task<ResponseDTO<TenantPaymentMethodResponseDto>> UpdateAsync(int id, UpdateTenantPaymentMethodRequestDto dto);
    Task<ResponseDTO<bool>> DeleteAsync(int id);
    Task<ResponseDTO<bool>> ToggleActiveAsync(int id);
    Task<ResponseDTO<bool>> ReorderAsync(List<int> orderedIds);
}
