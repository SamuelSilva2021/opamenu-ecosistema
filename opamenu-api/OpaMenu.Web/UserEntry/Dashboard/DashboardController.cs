using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpaMenu.Application.DTOs;
using OpaMenu.Application.DTOs.Dashboard;
using OpaMenu.Application.Interfaces;
using OpaMenu.Commons.Api.DTOs;
using OpaMenu.Web.UserEntry;

namespace OpaMenu.Web.UserEntry.Dashboard;

[ApiController]
[Route("api/dashboard")]
[Authorize]
public class DashboardController(IDashboardService dashboardService) : BaseController
{
    private readonly IDashboardService _dashboardService = dashboardService;

    [HttpGet("summary")]
    public async Task<ActionResult<ResponseDTO<DashboardSummaryDto>>> GetSummary()
    {
        var result = await _dashboardService.GetSummaryAsync();
        return BuildResponse(result);
    }
}
