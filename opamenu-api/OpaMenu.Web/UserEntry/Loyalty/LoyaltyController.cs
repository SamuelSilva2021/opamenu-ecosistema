using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpaMenu.Domain.DTOs.Loyalty;
using OpaMenu.Infrastructure.Authentication;
using OpaMenu.Domain.Interfaces;
using OpaMenu.Infrastructure.Anotations;
using OpaMenu.Infrastructure.Filters;
using OpaMenu.Application.Services.Interfaces.Opamenu;
using OpaMenu.Commons.Api.DTOs;

namespace OpaMenu.Web.UserEntry.Loyalty;

[Route("api/loyalty")]
[ApiController]
[ServiceFilter(typeof(PermissionAuthorizationFilter))]
public class LoyaltyController(
    ILoyaltyService loyaltyService,
    ICurrentUserService currentUserService
    ) : BaseController
{
    private readonly ILoyaltyService _loyaltyService = loyaltyService;
    private readonly ICurrentUserService _currentUserService = currentUserService;

    /// <summary>
    /// Obtém o programa de fidelidade do tenant atual
    /// </summary>
    [HttpGet("program")]
    [Authorize]
    [MapPermission(MODULE_LOYALTY, OPERATION_SELECT)]
    public async Task<ActionResult<ResponseDTO<LoyaltyProgramDto>>> GetProgram()
    {
        var tenantId = _currentUserService.GetTenantGuid();
        if (!tenantId.HasValue)
            return Unauthorized();

        var response = await _loyaltyService.GetProgramAsync(tenantId.Value);
        return BuildResponse(response);
    }

    /// <summary>
    /// Cria ou atualiza o programa de fidelidade
    /// </summary>
    [HttpPost("program")]
    [Authorize]
    [MapPermission(MODULE_LOYALTY, OPERATION_INSERT)]
    public async Task<ActionResult> InsertProgram([FromBody] CreateLoyaltyProgramDto dto)
    {
        var tenantId = _currentUserService.GetTenantGuid();
        if (!tenantId.HasValue)
            return Unauthorized();

        var response = await _loyaltyService.UpsertProgramAsync(tenantId.Value, dto);
        return BuildResponse(response);
    }

    /// <summary>
    /// Cria ou atualiza o programa de fidelidade
    /// </summary>
    [HttpPut("program")]
    [Authorize]
    [MapPermission(MODULE_LOYALTY, OPERATION_UPDATE)]
    public async Task<ActionResult> UpdateProgram([FromBody] CreateLoyaltyProgramDto dto)
    {
        var tenantId = _currentUserService.GetTenantGuid();
        if (!tenantId.HasValue)
            return Unauthorized();

        var response = await _loyaltyService.UpsertProgramAsync(tenantId.Value, dto);
        return BuildResponse(response);
    }

    [HttpPatch("program/{id}/toggle-status")]
    [Authorize]
    [MapPermission(MODULE_LOYALTY, OPERATION_UPDATE)]
    public async Task<ActionResult> ToggleProgramStatus(Guid id, [FromQuery] bool status)
    {
        var tenantId = _currentUserService.GetTenantGuid();
        if (!tenantId.HasValue)
            return Unauthorized();
        var response = await _loyaltyService.ToggleStatus(tenantId.Value, id, status);
        return BuildResponse(response);
    }

    /// <summary>
    /// Obtém o saldo de um cliente (Requer autenticação do tenant para consultar clientes, ou lógica pública futura)
    /// Por enquanto, vamos permitir que o tenant consulte o saldo de um cliente específico via telefone
    /// </summary>
    [HttpGet("balance/{phone}")]
    [Authorize]
    [MapPermission(MODULE_LOYALTY, OPERATION_SELECT)]
    public async Task<ActionResult> GetCustomerBalance(string phone)
    {
        var tenantId = _currentUserService.GetTenantGuid();
        if (!tenantId.HasValue)
            return Unauthorized();

        var response = await _loyaltyService.GetCustomerBalanceAsync(tenantId.Value, phone);
        return BuildResponse(response);
    }
}
