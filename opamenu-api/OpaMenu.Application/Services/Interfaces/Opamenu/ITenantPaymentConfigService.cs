using OpaMenu.Commons.Api.DTOs;
using OpaMenu.Domain.DTOs.TenantPaymentConfig;

namespace OpaMenu.Application.Services.Interfaces.Opamenu
{
    public interface ITenantPaymentConfigService
    {
        Task<ResponseDTO<TenantPaymentConfigResponseDto?>> GetConfigAsync();
        Task<ResponseDTO<TenantPaymentConfigResponseDto>> UpsertConfigAsync(UpsertTenantPaymentConfigRequestDto dto);
    }
}
