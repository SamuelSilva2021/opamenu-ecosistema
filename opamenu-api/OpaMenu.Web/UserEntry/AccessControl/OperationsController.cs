using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpaMenu.Application.Services.Interfaces.AccessControl;
using OpaMenu.Domain.DTOs.AccessControl;

namespace OpaMenu.Web.UserEntry.AccessControl;

[ApiController]
[Route("api/operation")]
[Authorize(Roles = "ADMIN,SUPER_ADMIN")]
public sealed class OperationsController(IOperationService operationService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PagedResultDto<OperationDto>>> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int limit = 10,
        [FromQuery] string? search = null)
    {
        var result = await operationService.GetAllAsync(page, limit, search);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<OperationDto>> GetById([FromRoute] string id)
    {
        var op = await operationService.GetByIdAsync(id);
        if (op == null)
        {
            return NotFound();
        }

        return Ok(op);
    }
}
