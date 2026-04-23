using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OpaMenu.Domain.DTOs.AccessControl;
using OpaMenu.Infrastructure.Shared.Data.Context.AccessControl;

namespace OpaMenu.Web.UserEntry.AccessControl;

[ApiController]
[Route("api/permission-operations")]
[Authorize(Roles = "ADMIN,SUPER_ADMIN")]
public sealed class PermissionOperationsController(AccessControlDbContext dbContext) : ControllerBase
{
    private readonly AccessControlDbContext _dbContext = dbContext;

    [HttpPost("bulk")]
    [Authorize(Roles = "SUPER_ADMIN")]
    public async Task<ActionResult<bool>> Bulk([FromBody] PermissionOperationBulkRequestDto request)
    {
        if (request.PermissionId == Guid.Empty)
        {
            return BadRequest();
        }

        var rp = await _dbContext.RolePermissions.FirstOrDefaultAsync(x => x.Id == request.PermissionId);
        if (rp == null)
        {
            return NotFound();
        }

        rp.Actions = NormalizeOperationIds(request.OperationIds);
        rp.UpdatedAt = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync();
        return Ok(true);
    }

    private static List<string> NormalizeOperationIds(List<string> operationIds)
    {
        var allowed = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "CREATE", "READ", "UPDATE", "DELETE", "SELECT" };

        if (operationIds.Count == 0)
        {
            return [];
        }

        return operationIds
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Select(x => x.Trim().ToUpperInvariant())
            .Where(allowed.Contains)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
    }
}

