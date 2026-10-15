using Opamenu.Application.DTOs;
using Opamenu.Commons.Api.DTOs;
using Opamenu.Domain.DTOs.Tenant;

namespace Opamenu.Application.Services.Interfaces.Opamenu;

public interface ITenantService
{
    Task<ResponseDTO<TenantBusinessResponseDto?>> GetTenantBusinessInfoBySlugAsync(string slug);
}
