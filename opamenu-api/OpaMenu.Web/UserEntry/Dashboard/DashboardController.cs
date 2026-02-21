using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpaMenu.Application.DTOs;
using OpaMenu.Application.DTOs.Dashboard;
using OpaMenu.Commons.Api.DTOs;
using OpaMenu.Web.UserEntry;
using OpaMenu.Infrastructure.Anotations;
using OpaMenu.Infrastructure.Filters;
using OpaMenu.Application.Services.Interfaces.Opamenu;

namespace OpaMenu.Web.UserEntry.Dashboard;

[ApiController]
[Route("api/dashboard")]
[Authorize]
[ServiceFilter(typeof(PermissionAuthorizationFilter))]
public class DashboardController(IDashboardService dashboardService) : BaseController
{
    private readonly IDashboardService _dashboardService = dashboardService;

    [HttpGet("summary")]
    [MapPermission(MODULE_DASHBOARD, OPERATION_SELECT)]
    public async Task<ActionResult<ResponseDTO<DashboardSummaryDto>>> GetSummary([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
    {
        var result = await _dashboardService.GetSummaryAsync(startDate, endDate);
        return BuildResponse(result);
    }
}
