using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpaMenu.Application.Services.Interfaces.AccessControl;
using OpaMenu.Domain.DTOs.AccessControl;

namespace OpaMenu.Web.UserEntry.AccessControl;

[ApiController]
[Route("api/permission-operations")]
[Authorize(Roles = "ADMIN,SUPER_ADMIN")]
public sealed class PermissionOperationsController(IPermissionOperationService permissionOperationService) : ControllerBase
{
    private readonly IPermissionOperationService _permissionOperationService = permissionOperationService;

    [HttpPost("bulk")]
    [Authorize(Roles = "SUPER_ADMIN")]
    public async Task<ActionResult<bool>> Bulk([FromBody] PermissionOperationBulkRequestDto request)
    {
        var (success, notFound, badRequest) = await _permissionOperationService.BulkAsync(request);
        if (badRequest)
        {
            return BadRequest();
        }

        if (notFound)
        {
            return NotFound();
        }

        return Ok(success);
    }
}
