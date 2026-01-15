using AutoMapper;
using OpaMenu.Application.Services.Interfaces;
using OpaMenu.Commons.Api.Commons;
using OpaMenu.Commons.Api.DTOs;
using OpaMenu.Domain.DTOs.Tenant;
using OpaMenu.Domain.Interfaces;

namespace OpaMenu.Application.Services;

public class TenantService(ITenantRepository tenantRepository, IMapper mapper) : ITenantService
{
    public async Task<ResponseDTO<TenantBusinessResponseDto?>> GetTenantBusinessInfoBySlugAsync(string slug)
    {
        var tenant = await tenantRepository.GetBySlugWithBusinessInfoAsync(slug);
        if (tenant == null) return StaticResponseBuilder<TenantBusinessResponseDto?>.BuildNotFound(null);

        var dto = mapper.Map<TenantBusinessResponseDto>(tenant);

        return StaticResponseBuilder<TenantBusinessResponseDto?>.BuildOk(dto);
    }
}
