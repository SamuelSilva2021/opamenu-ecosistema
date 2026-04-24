using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpaMenu.Application.Services.Interfaces.AccessControl;
using OpaMenu.Domain.DTOs.AccessControl;

namespace OpaMenu.Web.UserEntry.AccessControl;

[ApiController]
[Route("api/modules")]
[Authorize(Roles = "ADMIN,SUPER_ADMIN")]
public sealed class ModulesController(IModuleService moduleService) : ControllerBase
{
    private readonly IModuleService _moduleService = moduleService;

    [HttpGet]
    public async Task<ActionResult<PagedResultDto<ModuleDto>>> GetModules(
        [FromQuery] int page = 1,
        [FromQuery] int limit = 10,
        [FromQuery] string? search = null,
        [FromQuery] bool? isActive = null,
        [FromQuery] string? sortBy = null,
        [FromQuery] string? sortOrder = null)
    {
        var result = await _moduleService.GetModulesAsync(page, limit, search, isActive, sortBy, sortOrder);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ModuleDto>> GetModuleById([FromRoute] Guid id)
    {
        var module = await _moduleService.GetModuleByIdAsync(id);
        if (module == null)
        {
            return NotFound();
        }

        return Ok(module);
    }

    [HttpPost]
    [Authorize(Roles = "SUPER_ADMIN")]
    public async Task<ActionResult<ModuleDto>> Create([FromBody] CreateModuleRequestDto request)
    {
        var result = await _moduleService.CreateAsync(request);
        return result == null ? BadRequest() : Ok(result);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "SUPER_ADMIN")]
    public async Task<ActionResult<ModuleDto>> Update([FromRoute] Guid id, [FromBody] UpdateModuleRequestDto request)
    {
        var (module, notFound) = await _moduleService.UpdateAsync(id, request);
        if (notFound)
        {
            return NotFound();
        }

        return module == null ? BadRequest() : Ok(module);
    }

    [HttpPatch("{id:guid}/toggle-status")]
    [Authorize(Roles = "SUPER_ADMIN")]
    public async Task<ActionResult<ModuleDto>> ToggleStatus([FromRoute] Guid id)
    {
        var (module, notFound) = await _moduleService.ToggleStatusAsync(id);
        if (notFound)
        {
            return NotFound();
        }

        return module == null ? BadRequest() : Ok(module);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "SUPER_ADMIN")]
    public async Task<IActionResult> Delete([FromRoute] Guid id)
    {
        await _moduleService.DeleteAsync(id);
        return NoContent();
    }
}
