using System.Linq;
using AutoMapper;
using Opamenu.Application.Services.Interfaces.Opamenu;
using Opamenu.Commons.Api.Commons;
using Opamenu.Commons.Api.DTOs;
using Opamenu.Domain.DTOs.Loyalty;
using Opamenu.Domain.DTOs.Tenant;
using Opamenu.Domain.Interfaces;

namespace Opamenu.Application.Services.Opamenu;

public class TenantService(ITenantRepository tenantRepository, ILoyaltyProgramRepository loyaltyProgramRepository, IMapper mapper) : ITenantService
{
    public async Task<ResponseDTO<TenantBusinessResponseDto?>> GetTenantBusinessInfoBySlugAsync(string slug)
    {
        var tenant = await tenantRepository.GetBySlugWithBusinessInfoAsync(slug);
        if (tenant == null) return StaticResponseBuilder<TenantBusinessResponseDto?>.BuildNotFound(null);

        var dto = mapper.Map<TenantBusinessResponseDto>(tenant);

        var programs = await loyaltyProgramRepository.GetByTenantIdAsync(tenant.Id);
        var loyaltyProgram = programs.FirstOrDefault();

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
