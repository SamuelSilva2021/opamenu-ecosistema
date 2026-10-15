using Opamenu.Commons.Api.DTOs;
using Opamenu.Domain.DTOs.TenantPaymentConfig;

namespace Opamenu.Application.Services.Interfaces.Opamenu
{
    public interface ITenantPaymentConfigService
    {
        Task<ResponseDTO<TenantPaymentConfigResponseDto?>> GetConfigAsync();
        Task<ResponseDTO<TenantPaymentConfigResponseDto>> UpsertConfigAsync(UpsertTenantPaymentConfigRequestDto dto);
    }
}
