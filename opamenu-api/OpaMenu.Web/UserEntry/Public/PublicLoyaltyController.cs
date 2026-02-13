using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpaMenu.Application.Services.Interfaces.Opamenu;
using OpaMenu.Commons.Api.Commons;
using OpaMenu.Commons.Api.DTOs;
using OpaMenu.Domain.DTOs.Loyalty;
using OpaMenu.Domain.Interfaces;

namespace OpaMenu.Web.UserEntry.Public;

[ApiController]
[Route("api/public/{slug}/loyalty")]
[AllowAnonymous]
public class PublicLoyaltyController(
    ILoyaltyService loyaltyService,
    ITenantService tenantService
    ) : BaseController
{
    private readonly ILoyaltyService _loyaltyService = loyaltyService;
    private readonly ITenantService _tenantService = tenantService;

    /// <summary>
    /// Obtém todos os programas de fidelidade ativos para uma loja específica (slug)
    /// </summary>
    [HttpGet("programs")]
    public async Task<ActionResult<ResponseDTO<IEnumerable<LoyaltyProgramDto>>>> GetPublicPrograms(string slug)
    {
        var tenantResponse = await _tenantService.GetTenantBusinessInfoBySlugAsync(slug);
        if (!tenantResponse.Succeeded || tenantResponse.Data == null)
            return NotFound(StaticResponseBuilder<IEnumerable<LoyaltyProgramDto>>.BuildError("Restaurante não encontrado"));

        var response = await _loyaltyService.GetAllProgramsAsync(tenantResponse.Data.Id);
        
        // Filtrar apenas programas ativos para o público
        if (response.Succeeded && response.Data != null)
        {
            response.Data = response.Data.Where(p => p.IsActive);
        }

        return BuildResponse(response);
    }

    /// <summary>
    /// Obtém o saldo de fidelidade de um cliente para uma loja específica (slug)
    /// </summary>
    [HttpGet("balance/{phone}")]
    public async Task<ActionResult<ResponseDTO<CustomerLoyaltySummaryDto>>> GetPublicCustomerBalance(string slug, string phone)
    {
        var tenantResponse = await _tenantService.GetTenantBusinessInfoBySlugAsync(slug);
        if (!tenantResponse.Succeeded || tenantResponse.Data == null)
            return NotFound(StaticResponseBuilder<CustomerLoyaltySummaryDto>.BuildError("Restaurante não encontrado"));

        var response = await _loyaltyService.GetCustomerBalanceAsync(tenantResponse.Data.Id, phone);
        return BuildResponse(response);
    }
}
