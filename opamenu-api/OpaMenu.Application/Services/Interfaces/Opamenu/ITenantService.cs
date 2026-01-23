using OpaMenu.Application.DTOs;
using OpaMenu.Commons.Api.DTOs;
using OpaMenu.Domain.DTOs.Tenant;

namespace OpaMenu.Application.Services.Interfaces.Opamenu;

public interface ITenantService
{
    Task<ResponseDTO<TenantBusinessResponseDto?>> GetTenantBusinessInfoBySlugAsync(string slug);
}
