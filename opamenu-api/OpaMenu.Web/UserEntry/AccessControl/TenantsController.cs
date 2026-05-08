using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpaMenu.Application.Services.Interfaces.AccessControl;
using OpaMenu.Commons.Api.Commons;
using OpaMenu.Domain.DTOs.Auth;
using OpaMenu.Domain.DTOs.AccessControl;
using OpaMenu.Domain.DTOs.MultiTenant;

namespace OpaMenu.Web.UserEntry.AccessControl;

[ApiController]
[Route("api/tenants")]
[Authorize(Roles = "SUPER_ADMIN")]
public sealed class TenantsController(
    ITenantAdminService tenantAdminService) : ControllerBase
{
    private readonly ITenantAdminService _tenantAdminService = tenantAdminService;

    [HttpPost("/api/register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] RegisterTenantRequestDto request)
    {
        var (body, statusCode) = await _tenantAdminService.RegisterAsync(request);
        return statusCode switch
        {
            200 =>  Ok(body),
            400 => BadRequest(body),
            _ => StatusCode(statusCode, body)
        };
    }

    [HttpGet]
    public async Task<ActionResult<PagedResultDto<TenantSummaryDto>>> GetTenants([FromQuery] int page = 1, [FromQuery] int limit = 10)
    {
        page = page <= 0 ? 1 : page;
        limit = limit <= 0 ? 10 : limit;

        var filterName = Request.Query["filter.name"].ToString();
        var filterSlug = Request.Query["filter.slug"].ToString();
        var filterDomain = Request.Query["filter.domain"].ToString();
        var filterEmail = Request.Query["filter.email"].ToString();
        var filterPhone = Request.Query["filter.phone"].ToString();
        var filterStatus = Request.Query["filter.status"].ToString();

        var result = await _tenantAdminService.GetTenantsAsync(page, limit, filterName, filterSlug, filterDomain, filterEmail, filterPhone, filterStatus);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<TenantDto>> GetById([FromRoute] Guid id)
    {
        var tenant = await _tenantAdminService.GetByIdAsync(id);
        if (tenant == null)
        {
            return NotFound();
        }

        return Ok(tenant);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<TenantDto>> Update([FromRoute] Guid id, [FromBody] UpdateTenantRequestDto request)
    {
        var (tenant, notFound, badRequest) = await _tenantAdminService.UpdateAsync(id, request);
        if (notFound)
        {
            return NotFound();
        }

        if (badRequest || tenant == null)
        {
            return BadRequest();
        }

        return Ok(tenant);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete([FromRoute] Guid id)
    {
        await _tenantAdminService.DeleteAsync(id);
        return NoContent();
    }

    [HttpGet("{tenantId:guid}/modules")]
    public async Task<ActionResult<List<ModuleDto>>> GetTenantModules([FromRoute] Guid tenantId)
    {
        var modules = await _tenantAdminService.GetTenantModulesAsync(tenantId);
        return Ok(modules);
    }

    [HttpPost("{tenantId:guid}/modules/{moduleId:guid}")]
    public async Task<IActionResult> AddTenantModule([FromRoute] Guid tenantId, [FromRoute] Guid moduleId)
    {
        var (success, notFound, badRequest) = await _tenantAdminService.AddTenantModuleAsync(tenantId, moduleId);
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

    [HttpDelete("{tenantId:guid}/modules/{moduleId:guid}")]
    public async Task<IActionResult> RemoveTenantModule([FromRoute] Guid tenantId, [FromRoute] Guid moduleId)
    {
        var result = await _tenantAdminService.RemoveTenantModuleAsync(tenantId, moduleId);
        return Ok(result);
    }
}
