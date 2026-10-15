using Opamenu.Commons.Api.DTOs;
using Opamenu.Domain.DTOs.DeliveryArea;

namespace Opamenu.Application.Services.Interfaces.Opamenu;

public interface IDeliveryAreaService
{
    Task<ResponseDTO<IEnumerable<DeliveryAreaResponseDto>>> GetAllAsync(Guid tenantId);
    Task<ResponseDTO<DeliveryAreaResponseDto?>> GetByIdAsync(Guid id, Guid tenantId);
    Task<ResponseDTO<DeliveryAreaResponseDto>> CreateAsync(CreateDeliveryAreaRequestDto request, Guid tenantId);
    Task<ResponseDTO<DeliveryAreaResponseDto>> UpdateAsync(Guid id, CreateDeliveryAreaRequestDto request, Guid tenantId);
    Task<ResponseDTO<bool>> DeleteAsync(Guid id, Guid tenantId);
    Task<ResponseDTO<decimal?>> GetDeliveryFeeAsync(string slug, string city, string? neighborhood);
}
