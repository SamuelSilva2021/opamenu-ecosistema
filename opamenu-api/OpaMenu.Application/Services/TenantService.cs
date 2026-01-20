using AutoMapper;
using OpaMenu.Application.Services.Interfaces;
using OpaMenu.Commons.Api.Commons;
using OpaMenu.Commons.Api.DTOs;
using OpaMenu.Domain.DTOs.Loyalty;
using OpaMenu.Domain.DTOs.Tenant;
using OpaMenu.Domain.Interfaces;

namespace OpaMenu.Application.Services;

public class TenantService(ITenantRepository tenantRepository, ILoyaltyProgramRepository loyaltyProgramRepository, IMapper mapper) : ITenantService
{
    public async Task<ResponseDTO<TenantBusinessResponseDto?>> GetTenantBusinessInfoBySlugAsync(string slug)
    {
        var tenant = await tenantRepository.GetBySlugWithBusinessInfoAsync(slug);
        if (tenant == null) return StaticResponseBuilder<TenantBusinessResponseDto?>.BuildNotFound(null);

        var dto = mapper.Map<TenantBusinessResponseDto>(tenant);

        var loyaltyProgram = await loyaltyProgramRepository.GetByTenantIdAsync(tenant.Id);
        if (loyaltyProgram != null && loyaltyProgram.IsActive)
        {
            dto = dto with { LoyaltyProgram = new LoyaltyProgramDto
            {
                Id = loyaltyProgram.Id,
                Name = loyaltyProgram.Name,
                Description = loyaltyProgram.Description,
                PointsPerCurrency = loyaltyProgram.PointsPerCurrency,
                MinOrderValue = loyaltyProgram.MinOrderValue,
                PointsValidityDays = loyaltyProgram.PointsValidityDays,
                IsActive = loyaltyProgram.IsActive
            }};
        }

        return StaticResponseBuilder<TenantBusinessResponseDto?>.BuildOk(dto);
    }
}
