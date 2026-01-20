using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpaMenu.Application.Services.Interfaces;
using OpaMenu.Domain.DTOs.Loyalty;
using OpaMenu.Infrastructure.Authentication;
using OpaMenu.Domain.Interfaces;

namespace OpaMenu.Web.UserEntry.Loyalty;

[Route("api/loyalty")]
[ApiController]
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
    public async Task<ActionResult> GetProgram()
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
    public async Task<ActionResult> ToggleProgramStatus(int id, [FromQuery] bool status)
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
    public async Task<ActionResult> GetCustomerBalance(string phone)
    {
        var tenantId = _currentUserService.GetTenantGuid();
        if (!tenantId.HasValue)
            return Unauthorized();

        var response = await _loyaltyService.GetCustomerBalanceAsync(tenantId.Value, phone);
        return BuildResponse(response);
    }
}
