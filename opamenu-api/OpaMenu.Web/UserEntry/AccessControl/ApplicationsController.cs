using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpaMenu.Application.Services.Interfaces.AccessControl;
using OpaMenu.Domain.DTOs.AccessControl;

namespace OpaMenu.Web.UserEntry.AccessControl;

[ApiController]
[Route("api/applications")]
[Authorize(Roles = "ADMIN,SUPER_ADMIN")]
public sealed class ApplicationsController(IApplicationService applicationService) : ControllerBase
{
    private readonly IApplicationService _applicationService = applicationService;

    [HttpGet]
    public async Task<ActionResult<PagedResultDto<ApplicationDto>>> GetApplications(
        [FromQuery] int page = 1,
        [FromQuery] int limit = 10,
        [FromQuery] string? search = null)
    {
        var result = await _applicationService.GetApplicationsAsync(page, limit, search);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApplicationDto>> GetById([FromRoute] Guid id)
    {
        var app = await _applicationService.GetByIdAsync(id);
        if (app == null)
        {
            return NotFound();
        }

        return Ok(app);
    }

    [HttpPost]
    [Authorize(Roles = "SUPER_ADMIN")]
    public async Task<ActionResult<ApplicationDto>> Create([FromBody] CreateApplicationRequestDto request)
    {
        var result = await _applicationService.CreateAsync(request);
        return result == null ? BadRequest() : Ok(result);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "SUPER_ADMIN")]
    public async Task<ActionResult<ApplicationDto>> Update([FromRoute] Guid id, [FromBody] UpdateApplicationRequestDto request)
    {
        var (app, notFound) = await _applicationService.UpdateAsync(id, request);
        if (notFound)
        {
            return NotFound();
        }

        return app == null ? BadRequest() : Ok(app);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "SUPER_ADMIN")]
    public async Task<IActionResult> Delete([FromRoute] Guid id)
    {
        await _applicationService.DeleteAsync(id);
        return NoContent();
    }
}
