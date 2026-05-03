using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpaMenu.Application.Services.Interfaces.Opamenu;
using OpaMenu.Domain.DTOs.MultiTenant;

namespace OpaMenu.Web.UserEntry.MultiTenant;

[ApiController]
[Route("api/plans")]
[Authorize(Roles = "SUPER_ADMIN")]
public sealed class PlansController(IPlanAdminService planAdminService) : ControllerBase
{
    private readonly IPlanAdminService _planAdminService = planAdminService;

    [HttpGet("active")]
    [AllowAnonymous]
    public async Task<IActionResult> GetActive()
    {
        var plans = await _planAdminService.GetActiveAsync();
        return Ok(plans);
    }

    [HttpGet]
    public async Task<ActionResult<PlanListResponseDto>> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? name = null,
        [FromQuery] string? status = null)
    {
        var response = await _planAdminService.GetAllAsync(page, pageSize, name, status);
        return Ok(response);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponseDto<PlanDto>>> GetById([FromRoute] Guid id)
    {
        var response = await _planAdminService.GetByIdAsync(id);
        if (!response.Succeeded) return NotFound(response);
        return Ok(response);
    }

    [HttpPost]
    public async Task<ActionResult<PlanDto>> Create([FromBody] CreatePlanRequestDto request)
    {
        try
        {
            var result = await _planAdminService.CreateAsync(request);
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<PlanDto>> Update([FromRoute] Guid id, [FromBody] UpdatePlanRequestDto request)
    {
        try
        {
            var result = await _planAdminService.UpdateAsync(id, request);
            return Ok(result);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<bool>> Delete([FromRoute] Guid id)
    {
        var result = await _planAdminService.DeleteAsync(id);
        return Ok(result);
    }
}
