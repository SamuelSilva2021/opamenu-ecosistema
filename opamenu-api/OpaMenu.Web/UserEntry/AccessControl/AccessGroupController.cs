using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpaMenu.Application.Services.Interfaces.AccessControl;
using OpaMenu.Domain.DTOs.AccessControl;

namespace OpaMenu.Web.UserEntry.AccessControl;

[ApiController]
[Route("api/access-group")]
[Authorize(Roles = "ADMIN,SUPER_ADMIN")]
public sealed class AccessGroupController(IAccessGroupService accessGroupService) : ControllerBase
{
    private readonly IAccessGroupService _accessGroupService = accessGroupService;

    [HttpGet]
    public async Task<ActionResult<PagedResultDto<AccessGroupDto>>> GetAccessGroups(
        [FromQuery] int page = 1,
        [FromQuery] int limit = 10,
        [FromQuery] string? search = null)
    {
        var result = await _accessGroupService.GetGroupsAsync(page, limit, search);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<AccessGroupDto>> GetById([FromRoute] Guid id)
    {
        var group = await _accessGroupService.GetGroupByIdAsync(id);
        if (group == null)
        {
            return NotFound();
        }

        return Ok(group);
    }

    [HttpPost]
    public async Task<ActionResult<AccessGroupDto>> Create([FromBody] CreateAccessGroupRequestDto request)
    {
        var result = await _accessGroupService.CreateGroupAsync(request);
        return result == null ? BadRequest() : Ok(result);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<AccessGroupDto>> Update([FromRoute] Guid id, [FromBody] UpdateAccessGroupRequestDto request)
    {
        var (group, notFound, badRequest) = await _accessGroupService.UpdateGroupAsync(id, request);
        if (notFound)
        {
            return NotFound();
        }

        if (badRequest || group == null)
        {
            return BadRequest();
        }

        return Ok(group);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete([FromRoute] Guid id)
    {
        await _accessGroupService.DeleteGroupAsync(id);
        return NoContent();
    }

    [HttpGet("group-types")]
    public async Task<ActionResult<List<GroupTypeDto>>> GetGroupTypes()
    {
        var result = await _accessGroupService.GetGroupTypesAsync();
        return Ok(result.ToList());
    }

    [HttpGet("group-types/{id:guid}")]
    public async Task<ActionResult<GroupTypeDto>> GetGroupTypeById([FromRoute] Guid id)
    {
        var gt = await _accessGroupService.GetGroupTypeByIdAsync(id);
        if (gt == null)
        {
            return NotFound();
        }

        return Ok(gt);
    }

    [HttpPost("group-types")]
    public async Task<ActionResult<GroupTypeDto>> CreateGroupType([FromBody] CreateGroupTypeRequestDto request)
    {
        var result = await _accessGroupService.CreateGroupTypeAsync(request);
        return result == null ? BadRequest() : Ok(result);
    }

    [HttpPut("group-types/{id:guid}")]
    public async Task<ActionResult<GroupTypeDto>> UpdateGroupType([FromRoute] Guid id, [FromBody] UpdateGroupTypeRequestDto request)
    {
        var (gt, notFound, badRequest) = await _accessGroupService.UpdateGroupTypeAsync(id, request);
        if (notFound)
        {
            return NotFound();
        }

        if (badRequest || gt == null)
        {
            return BadRequest();
        }

        return Ok(gt);
    }

    [HttpDelete("group-types/{id:guid}")]
    public async Task<IActionResult> DeleteGroupType([FromRoute] Guid id)
    {
        await _accessGroupService.DeleteGroupTypeAsync(id);
        return NoContent();
    }
}
