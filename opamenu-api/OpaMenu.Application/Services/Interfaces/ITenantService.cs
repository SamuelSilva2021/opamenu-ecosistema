using OpaMenu.Application.DTOs;
using OpaMenu.Domain.DTOs.Tenant;

namespace OpaMenu.Application.Services.Interfaces;

public interface ITenantService
{
    Task<ResponseDTO<TenantBusinessResponseDto?>> GetTenantBusinessInfoBySlugAsync(string slug);
}
