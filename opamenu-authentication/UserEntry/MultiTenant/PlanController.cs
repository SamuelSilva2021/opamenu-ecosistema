using Authenticator.API.Core.Application.Interfaces.MultiTenant;
using Authenticator.API.Core.Domain.Api;
using Authenticator.API.Core.Domain.MultiTenant.Plan.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Authenticator.API.UserEntry.MultiTenant;

[Route("api/plans")]
[ApiController]
public class PlanController(IPlanService planService) : BaseController
{
    [HttpPost]
    [Authorize(Roles = "SUPER_ADMIN")]
    public async Task<ActionResult<ResponseDTO<PlanDTO>>> Create([FromBody] CreatePlanDTO dto)
    {
        var result = await planService.CreateAsync(dto);
        return BuildResponse(result);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "SUPER_ADMIN")]
    public async Task<ActionResult<ResponseDTO<PlanDTO>>> Update(Guid id, [FromBody] UpdatePlanDTO dto)
    {
        var result = await planService.UpdateAsync(id, dto);
        return BuildResponse(result);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "SUPER_ADMIN")]
    public async Task<ActionResult<ResponseDTO<bool>>> Delete(Guid id)
    {
        var result = await planService.DeleteAsync(id);
        return BuildResponse(result);
    }

    [HttpGet("{id:guid}")]
    [Authorize]
    public async Task<ActionResult<ResponseDTO<PlanDTO>>> GetById(Guid id)
    {
        var result = await planService.GetByIdAsync(id);
        return BuildResponse(result);
    }

    [HttpGet("slug/{slug}")]
    [AllowAnonymous]
    public async Task<ActionResult<ResponseDTO<PlanDTO>>> GetBySlug(string slug)
    {
        var result = await planService.GetBySlugAsync(slug);
        return BuildResponse(result);
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<ResponseDTO<PagedResponseDTO<PlanDTO>>>> GetAll([FromQuery] PlanFilterDTO filter)
    {
        var result = await planService.GetAllAsync(filter);
        return BuildResponse(result);
    }

    [HttpGet("active")]
    [AllowAnonymous]
    public async Task<ActionResult<ResponseDTO<List<PlanSummaryDTO>>>> GetActive()
    {
        var result = await planService.GetActivePlansAsync();
        return BuildResponse(result);
    }
}
