using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpaMenu.Domain.DTOs.AccessControl;

namespace OpaMenu.Web.UserEntry.AccessControl;

[ApiController]
[Route("api/operation")]
[Authorize(Roles = "ADMIN,SUPER_ADMIN")]
public sealed class OperationsController : ControllerBase
{
    [HttpGet]
    public ActionResult<PagedResultDto<OperationDto>> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int limit = 10,
        [FromQuery] string? search = null)
    {
        page = page <= 0 ? 1 : page;
        limit = limit <= 0 ? 10 : limit;

        var operations = GetDefaultOperations();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.Trim();
            operations = operations
                .Where(o =>
                    o.Name.Contains(s, StringComparison.OrdinalIgnoreCase) ||
                    (o.Code != null && o.Code.Contains(s, StringComparison.OrdinalIgnoreCase)) ||
                    (o.Value != null && o.Value.Contains(s, StringComparison.OrdinalIgnoreCase)))
                .ToList();
        }

        var total = operations.Count;
        var totalPages = (int)Math.Ceiling(total / (double)limit);

        var items = operations
            .Skip((page - 1) * limit)
            .Take(limit)
            .ToList();

        return Ok(new PagedResultDto<OperationDto>
        {
            Items = items,
            Page = page,
            Limit = limit,
            Total = total,
            TotalPages = totalPages
        });
    }

    [HttpGet("{id}")]
    public ActionResult<OperationDto> GetById([FromRoute] string id)
    {
        var op = GetDefaultOperations().FirstOrDefault(o => string.Equals(o.Id, id, StringComparison.OrdinalIgnoreCase));
        if (op == null)
        {
            return NotFound();
        }

        return Ok(op);
    }

    private static List<OperationDto> GetDefaultOperations()
    {
        var now = DateTime.UtcNow;
        return
        [
            new OperationDto { Id = "CREATE", Name = "CREATE", Code = "CREATE", Value = "CREATE", IsActive = true, CreatedAt = now, UpdatedAt = null },
            new OperationDto { Id = "READ", Name = "READ", Code = "READ", Value = "READ", IsActive = true, CreatedAt = now, UpdatedAt = null },
            new OperationDto { Id = "UPDATE", Name = "UPDATE", Code = "UPDATE", Value = "UPDATE", IsActive = true, CreatedAt = now, UpdatedAt = null },
            new OperationDto { Id = "DELETE", Name = "DELETE", Code = "DELETE", Value = "DELETE", IsActive = true, CreatedAt = now, UpdatedAt = null },
            new OperationDto { Id = "SELECT", Name = "SELECT", Code = "SELECT", Value = "SELECT", IsActive = true, CreatedAt = now, UpdatedAt = null }
        ];
    }
}

