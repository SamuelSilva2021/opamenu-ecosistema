using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpaMenu.Application.Services.Interfaces.AccessControl;
using OpaMenu.Commons.Api.Commons;
using OpaMenu.Domain.DTOs.AccessControl;
using OpaMenu.Domain.Interfaces;

namespace OpaMenu.Web.UserEntry.AccessControl;

[ApiController]
[Route("api/roles")]
[Authorize(Roles = "ADMIN,SUPER_ADMIN")]
public sealed class RolesController(IRoleService roleService, ICurrentUserService currentUserService) : ControllerBase
{
    private readonly IRoleService _roleService = roleService;
    private readonly ICurrentUserService _currentUserService = currentUserService;

    [HttpGet]
    public async Task<ActionResult<PagedResultDto<RoleDto>>> GetRoles(
        [FromQuery] int page = 1,
        [FromQuery] int limit = 10,
        [FromQuery] string? search = null)
    {
        var result = await _roleService.GetRolesAsync(page, limit, search);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<RoleDto>> GetById([FromRoute] Guid id)
    {
        var role = await _roleService.GetByIdAsync(id);
        if (role == null)
        {
            return NotFound();
        }

        return Ok(role);
    }

    [HttpPost]
    public async Task<ActionResult<RoleDto>> Create([FromBody] CreateRoleRequestDto request)
    {
        var result = await _roleService.CreateAsync(request);
        return result == null ? BadRequest() : Ok(result);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<RoleDto>> Update([FromRoute] Guid id, [FromBody] UpdateRoleRequestDto request)
    {
        var (role, notFound) = await _roleService.UpdateAsync(id, request);
        if (notFound)
        {
            return NotFound();
        }

        return role == null ? BadRequest() : Ok(role);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete([FromRoute] Guid id)
    {
        await _roleService.DeleteAsync(id);
        return NoContent();
    }

    [HttpGet("/api/roles-painel")]
    public async Task<IActionResult> GetRolesPainel(
        [FromQuery] int page = 1,
        [FromQuery] int limit = 10,
        [FromQuery] string? name = null)
    {
        var tenantId = _currentUserService.GetTenantGuid();
        if (!tenantId.HasValue || tenantId.Value == Guid.Empty)
        {
            return BadRequest(StaticResponseBuilder<PagedResultDto<RolePainelDto>>.BuildError("Tenant não identificado."));
        }

        var search = string.IsNullOrWhiteSpace(name) ? Request.Query["search"].ToString() : name;
        var result = await _roleService.GetRolesPainelAsync(tenantId.Value, page, limit, search);
        return StatusCode(result.Code, result);
    }

    [HttpGet("/api/roles-painel/{id:guid}")]
    public async Task<IActionResult> GetRolePainelById([FromRoute] Guid id)
    {
        var tenantId = _currentUserService.GetTenantGuid();
        if (!tenantId.HasValue || tenantId.Value == Guid.Empty)
        {
            return BadRequest(StaticResponseBuilder<RolePainelDto>.BuildError("Tenant não identificado."));
        }

        var result = await _roleService.GetRolePainelByIdAsync(tenantId.Value, id);
        return StatusCode(result.Code, result);
    }

    [HttpGet("/api/roles-painel/modules")]
    public async Task<IActionResult> GetAvailableModulesPainel()
    {
        var tenantId = _currentUserService.GetTenantGuid();
        if (!tenantId.HasValue || tenantId.Value == Guid.Empty)
        {
            return BadRequest(StaticResponseBuilder<List<AvailableModuleDto>>.BuildError("Tenant não identificado."));
        }

        var result = await _roleService.GetAvailableModulesPainelAsync(tenantId.Value);
        return StatusCode(result.Code, result);
    }

    [HttpPost("/api/roles-painel")]
    public async Task<IActionResult> CreateRolePainel([FromBody] CreateRoleRequestDto request)
    {
        var tenantId = _currentUserService.GetTenantGuid();
        if (!tenantId.HasValue || tenantId.Value == Guid.Empty)
        {
            return BadRequest(StaticResponseBuilder<RolePainelDto>.BuildError("Tenant não identificado."));
        }

        var result = await _roleService.CreateRolePainelAsync(tenantId.Value, request);
        return StatusCode(result.Code, result);
    }

    [HttpPut("/api/roles-painel/{id:guid}")]
    public async Task<IActionResult> UpdateRolePainel([FromRoute] Guid id, [FromBody] UpdateRoleRequestDto request)
    {
        var tenantId = _currentUserService.GetTenantGuid();
        if (!tenantId.HasValue || tenantId.Value == Guid.Empty)
        {
            return BadRequest(StaticResponseBuilder<RolePainelDto>.BuildError("Tenant não identificado."));
        }

        var result = await _roleService.UpdateRolePainelAsync(tenantId.Value, id, request);
        return StatusCode(result.Code, result);
    }

    [HttpDelete("/api/roles-painel/{id:guid}")]
    public async Task<IActionResult> DeleteRolePainel([FromRoute] Guid id)
    {
        var tenantId = _currentUserService.GetTenantGuid();
        if (!tenantId.HasValue || tenantId.Value == Guid.Empty)
        {
            return BadRequest(StaticResponseBuilder<bool>.BuildError("Tenant não identificado."));
        }

        var result = await _roleService.DeleteRolePainelAsync(tenantId.Value, id);
        return StatusCode(result.Code, result);
    }
}
