using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Opamenu.Application.DTOs;
using Opamenu.Application.DTOs.Dashboard;
using Opamenu.Commons.Api.DTOs;
using Opamenu.Web.UserEntry;
using Opamenu.Infrastructure.Anotations;
using Opamenu.Infrastructure.Filters;
using Opamenu.Application.Services.Interfaces.Opamenu;

namespace Opamenu.Web.UserEntry.Dashboard;

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
