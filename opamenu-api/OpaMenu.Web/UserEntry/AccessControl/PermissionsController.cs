using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpaMenu.Application.Services.Interfaces.AccessControl;
using OpaMenu.Domain.DTOs.AccessControl;

namespace OpaMenu.Web.UserEntry.AccessControl;

[ApiController]
[Route("api/permissions")]
[Authorize(Roles = "ADMIN,SUPER_ADMIN")]
public sealed class PermissionsController(IPermissionService permissionService) : ControllerBase
{
    private readonly IPermissionService _permissionService = permissionService;

    [HttpGet]
    public async Task<ActionResult<PagedResultDto<PermissionDto>>> GetPermissions(
        [FromQuery] int page = 1,
        [FromQuery] int limit = 10,
        [FromQuery] string? search = null,
        [FromQuery] Guid? moduleId = null,
        [FromQuery] Guid? tenantId = null)
    {
        var result = await _permissionService.GetPermissionsAsync(page, limit, search, moduleId, tenantId);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<PermissionDto>> GetPermissionById([FromRoute] Guid id)
    {
        var rp = await _permissionService.GetPermissionByIdAsync(id);
        if (rp == null)
        {
            return NotFound();
        }

        return Ok(rp);
    }

    [HttpPost]
    [Authorize(Roles = "SUPER_ADMIN")]
    public async Task<ActionResult<PermissionDto>> Create([FromBody] CreatePermissionRequestDto request)
    {
        var result = await _permissionService.CreateAsync(request);
        return result == null ? BadRequest() : Ok(result);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "SUPER_ADMIN")]
    public async Task<ActionResult<PermissionDto>> Update([FromRoute] Guid id, [FromBody] UpdatePermissionRequestDto request)
    {
        var (permission, notFound) = await _permissionService.UpdateAsync(id, request);
        if (notFound)
        {
            return NotFound();
        }

        return permission == null ? BadRequest() : Ok(permission);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "SUPER_ADMIN")]
    public async Task<IActionResult> Delete([FromRoute] Guid id)
    {
        await _permissionService.DeleteAsync(id);
        return NoContent();
    }
}
