using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpaMenu.Application.Services.Interfaces.Opamenu;
using OpaMenu.Domain.DTOs.CashRegister;
using OpaMenu.Infrastructure.Anotations;

namespace OpaMenu.Web.UserEntry.CashRegister;

[Authorize]
[Route("api/cash-register")]
public class CashRegisterController(ICashRegisterService cashRegisterService) : BaseController
{
    private readonly ICashRegisterService _cashRegisterService = cashRegisterService;

    [HttpGet("active")]
    [MapPermission(MODULE_PDV, OPERATION_SELECT)]
    public async Task<ActionResult> GetActiveShift()
    {
        return BuildResponse(await _cashRegisterService.GetActiveShiftAsync());
    }

    [HttpPost("open")]
    [MapPermission(MODULE_PDV, OPERATION_INSERT)]
    public async Task<ActionResult> OpenShift([FromBody] OpenCashShiftRequestDto request)
    {
        return BuildResponse(await _cashRegisterService.OpenShiftAsync(request));
    }

    [HttpPost("close")]
    [MapPermission(MODULE_PDV, OPERATION_INSERT)]
    public async Task<ActionResult> CloseShift([FromBody] CloseCashShiftRequestDto request)
    {
        return BuildResponse(await _cashRegisterService.CloseShiftAsync(request));
    }

    [HttpPost("movement")]
    [MapPermission(MODULE_PDV, OPERATION_INSERT)]
    public async Task<ActionResult> AddMovement([FromBody] AddCashMovementRequestDto request)
    {
        return BuildResponse(await _cashRegisterService.AddMovementAsync(request));
    }

    [HttpGet("history")]
    [MapPermission(MODULE_PDV, OPERATION_SELECT)]
    public async Task<ActionResult> GetHistory([FromQuery] int count = 10)
    {
        return BuildResponse(await _cashRegisterService.GetShiftHistoryAsync(count));
    }
}
