using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpaMenu.Application.Services.Interfaces.AccessControl;
using OpaMenu.Commons.Api.Commons;
using OpaMenu.Domain.DTOs.AccessControl;
using OpaMenu.Domain.Interfaces;

namespace OpaMenu.Web.UserEntry.AccessControl;

[ApiController]
[Route("api/users")]
[Authorize(Roles = "ADMIN,SUPER_ADMIN")]
public sealed class UsersController(IUserService userService, ICurrentUserService currentUserService) : ControllerBase
{
    private readonly IUserService _userService = userService;
    private readonly ICurrentUserService _currentUserService = currentUserService;

    [HttpGet]
    public async Task<ActionResult<PagedResultDto<UserAccountDto>>> GetUsers(
        [FromQuery] int page = 1,
        [FromQuery] int limit = 10,
        [FromQuery] string? search = null)
    {
        var result = await _userService.GetUsersAsync(page, limit, search);
        return Ok(result);
    }

    [HttpGet("active")]
    public async Task<ActionResult<List<UserAccountDto>>> GetActiveUsers()
    {
        var users = await _userService.GetActiveUsersAsync();
        return Ok(users);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<UserAccountDto>> GetById([FromRoute] Guid id)
    {
        var user = await _userService.GetByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        return Ok(user);
    }

    [HttpPost]
    public async Task<ActionResult<UserAccountDto>> Create([FromBody] CreateUserAccountRequestDto request)
    {
        var result = await _userService.CreateAsync(request);
        return result == null ? BadRequest() : Ok(result);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<UserAccountDto>> Update([FromRoute] Guid id, [FromBody] UpdateUserAccountRequestDto request)
    {
        var (user, notFound) = await _userService.UpdateAsync(id, request);
        if (notFound)
        {
            return NotFound();
        }

        return user == null ? BadRequest() : Ok(user);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete([FromRoute] Guid id)
    {
        await _userService.DeleteAsync(id);
        return NoContent();
    }

    [HttpGet("/api/user-accounts-painel")]
    public async Task<IActionResult> GetEmployeesPainel(
        [FromQuery] int page = 1,
        [FromQuery] int limit = 10,
        [FromQuery] string? search = null)
    {
        var tenantId = _currentUserService.GetTenantGuid();
        if (!tenantId.HasValue || tenantId.Value == Guid.Empty)
        {
            return BadRequest(StaticResponseBuilder<PagedResultDto<UserAccountDto>>.BuildError("Tenant não identificado."));
        }

        var result = await _userService.GetEmployeesPainelAsync(tenantId.Value, page, limit, search);
        return StatusCode(result.Code, result);
    }

    [HttpGet("/api/user-accounts-painel/{id:guid}")]
    public async Task<IActionResult> GetEmployeePainelById([FromRoute] Guid id)
    {
        var tenantId = _currentUserService.GetTenantGuid();
        if (!tenantId.HasValue || tenantId.Value == Guid.Empty)
        {
            return BadRequest(StaticResponseBuilder<UserAccountDto>.BuildError("Tenant não identificado."));
        }

        var result = await _userService.GetEmployeePainelByIdAsync(tenantId.Value, id);
        return StatusCode(result.Code, result);
    }

    [HttpPost("/api/user-accounts-painel")]
    public async Task<IActionResult> CreateEmployeePainel([FromBody] CreateUserAccountRequestDto request)
    {
        var tenantId = _currentUserService.GetTenantGuid();
        if (!tenantId.HasValue || tenantId.Value == Guid.Empty)
        {
            return BadRequest(StaticResponseBuilder<UserAccountDto>.BuildError("Tenant não identificado."));
        }

        var result = await _userService.CreateEmployeePainelAsync(tenantId.Value, request);
        return StatusCode(result.Code, result);
    }

    [HttpPut("/api/user-accounts-painel/{id:guid}")]
    public async Task<IActionResult> UpdateEmployeePainel([FromRoute] Guid id, [FromBody] UpdateUserAccountRequestDto request)
    {
        var tenantId = _currentUserService.GetTenantGuid();
        if (!tenantId.HasValue || tenantId.Value == Guid.Empty)
        {
            return BadRequest(StaticResponseBuilder<UserAccountDto>.BuildError("Tenant não identificado."));
        }

        var result = await _userService.UpdateEmployeePainelAsync(tenantId.Value, id, request);
        return StatusCode(result.Code, result);
    }

    [HttpPatch("/api/user-accounts-painel/{id:guid}/toggle-status")]
    public async Task<IActionResult> ToggleEmployeeStatusPainel([FromRoute] Guid id)
    {
        var tenantId = _currentUserService.GetTenantGuid();
        if (!tenantId.HasValue || tenantId.Value == Guid.Empty)
        {
            return BadRequest(StaticResponseBuilder<UserAccountDto>.BuildError("Tenant não identificado."));
        }

        var result = await _userService.ToggleEmployeeStatusPainelAsync(tenantId.Value, id);
        return StatusCode(result.Code, result);
    }

    [HttpDelete("/api/user-accounts-painel/{id:guid}")]
    public async Task<IActionResult> DeleteEmployeePainel([FromRoute] Guid id)
    {
        var tenantId = _currentUserService.GetTenantGuid();
        if (!tenantId.HasValue || tenantId.Value == Guid.Empty)
        {
            return BadRequest(StaticResponseBuilder<bool>.BuildError("Tenant não identificado."));
        }

        var result = await _userService.DeleteEmployeePainelAsync(tenantId.Value, id);
        return StatusCode(result.Code, result);
    }

    [HttpPost("forgot-password")]
    [AllowAnonymous]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequestDto request)
    {
        await _userService.ForgotPasswordAsync(request);
        return Ok(true);
    }

    [HttpPost("reset-password")]
    [AllowAnonymous]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequestDto request)
    {
        var success = await _userService.ResetPasswordAsync(request);
        return success ? Ok(true) : BadRequest();
    }

    [HttpGet("{userId:guid}/access-groups")]
    public async Task<ActionResult<List<AccessGroupDto>>> GetUserAccessGroups([FromRoute] Guid userId)
    {
        var groups = await _userService.GetUserAccessGroupsAsync(userId);
        return Ok(groups);
    }

    [HttpPost("{userId:guid}/access-groups")]
    public async Task<ActionResult<bool>> AssignUserAccessGroups([FromRoute] Guid userId, [FromBody] AssignUserAccessGroupsRequestDto request)
    {
        var (success, notFound, badRequest) = await _userService.AssignUserAccessGroupsAsync(userId, request);
        if (notFound)
        {
            return NotFound();
        }

        if (badRequest)
        {
            return BadRequest();
        }

        return Ok(success);
    }

    [HttpDelete("{userId:guid}/access-groups/{groupId:guid}")]
    public async Task<ActionResult<bool>> RevokeUserAccessGroup([FromRoute] Guid userId, [FromRoute] Guid groupId)
    {
        var result = await _userService.RevokeUserAccessGroupAsync(userId, groupId);
        return Ok(result);
    }
}
