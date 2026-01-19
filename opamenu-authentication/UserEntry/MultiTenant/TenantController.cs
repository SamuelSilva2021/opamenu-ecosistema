using Authenticator.API.Core.Application.Interfaces.MultiTenant;
using Authenticator.API.Core.Domain.Api;
using Authenticator.API.Core.Domain.MultiTenant.Tenant.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Authenticator.API.UserEntry.MultiTenant;

[Route("api/tenants")]
[ApiController]
public class TenantController(ITenantService tenantService) : BaseController
{
    [HttpGet]
    [Authorize(Roles = "SUPER_ADMIN")]
    public async Task<ActionResult<ResponseDTO<PagedResponseDTO<TenantSummaryDTO>>>> GetAll([FromQuery] int page = 1, [FromQuery] int limit = 10, [FromQuery] TenantFilterDTO? filter = null)
    {
        var result = await tenantService.GetAllAsync(page, limit, filter);
        return BuildResponse(result);
    }

    [HttpGet("{id:guid}")]
    [Authorize(Roles = "SUPER_ADMIN")]
    public async Task<ActionResult<ResponseDTO<TenantDTO>>> GetById(Guid id)
    {
        var result = await tenantService.GetByIdAsync(id);
        return BuildResponse(result);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "SUPER_ADMIN")]
    public async Task<ActionResult<ResponseDTO<TenantDTO>>> Update(Guid id, [FromBody] UpdateTenantDTO dto)
    {
        var result = await tenantService.UpdateAsync(id, dto);
        return BuildResponse(result);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "SUPER_ADMIN")]
    public async Task<ActionResult<ResponseDTO<bool>>> Delete(Guid id)
    {
        var result = await tenantService.DeleteAsync(id);
        return BuildResponse(result);
    }
}
