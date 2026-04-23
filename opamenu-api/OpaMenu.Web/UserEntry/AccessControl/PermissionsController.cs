using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OpaMenu.Domain.DTOs.AccessControl;
using OpaMenu.Infrastructure.Shared.Data.Context.AccessControl;
using OpaMenu.Infrastructure.Shared.Entities.AccessControl;

namespace OpaMenu.Web.UserEntry.AccessControl;

[ApiController]
[Route("api/permissions")]
[Authorize(Roles = "ADMIN,SUPER_ADMIN")]
public sealed class PermissionsController(AccessControlDbContext dbContext) : ControllerBase
{
    private readonly AccessControlDbContext _dbContext = dbContext;

    [HttpGet]
    public async Task<ActionResult<PagedResultDto<PermissionDto>>> GetPermissions(
        [FromQuery] int page = 1,
        [FromQuery] int limit = 10,
        [FromQuery] string? search = null,
        [FromQuery] Guid? moduleId = null,
        [FromQuery] Guid? tenantId = null)
    {
        page = page <= 0 ? 1 : page;
        limit = limit <= 0 ? 10 : limit;

        var rolePermissionsQuery = _dbContext.RolePermissions.AsNoTracking().Include(rp => rp.Role).AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.Trim();
            rolePermissionsQuery = rolePermissionsQuery.Where(rp =>
                rp.ModuleKey.Contains(s) ||
                rp.Role.Name.Contains(s) ||
                (rp.Role.Code != null && rp.Role.Code.Contains(s)));
        }

        if (tenantId.HasValue && tenantId.Value != Guid.Empty)
        {
            rolePermissionsQuery = rolePermissionsQuery.Where(rp => rp.Role.TenantId == tenantId.Value);
        }

        Dictionary<Guid, string> moduleIdToKey = new();
        if (moduleId.HasValue && moduleId.Value != Guid.Empty)
        {
            var moduleKey = await _dbContext.Modules.AsNoTracking().Where(m => m.Id == moduleId.Value).Select(m => m.Key).FirstOrDefaultAsync();
            if (string.IsNullOrWhiteSpace(moduleKey))
            {
                return Ok(new PagedResultDto<PermissionDto>
                {
                    Items = [],
                    Page = page,
                    Limit = limit,
                    Total = 0,
                    TotalPages = 0
                });
            }

            rolePermissionsQuery = rolePermissionsQuery.Where(rp => rp.ModuleKey == moduleKey);
        }

        var total = await rolePermissionsQuery.CountAsync();
        var totalPages = (int)Math.Ceiling(total / (double)limit);

        var ids = await rolePermissionsQuery
            .OrderByDescending(rp => rp.CreatedAt)
            .Skip((page - 1) * limit)
            .Take(limit)
            .Select(rp => rp.Id)
            .ToListAsync();

        var data = await _dbContext.RolePermissions.AsNoTracking()
            .Include(rp => rp.Role)
            .Where(rp => ids.Contains(rp.Id))
            .ToListAsync();

        var moduleKeys = data.Select(d => d.ModuleKey).Distinct(StringComparer.OrdinalIgnoreCase).ToList();
        var modules = await _dbContext.Modules.AsNoTracking()
            .Where(m => m.Key != null && moduleKeys.Contains(m.Key))
            .Select(m => new { m.Id, m.Key, m.Name })
            .ToListAsync();

        var moduleByKey = modules.ToDictionary(m => m.Key!, m => new { m.Id, m.Name }, StringComparer.OrdinalIgnoreCase);

        var items = data
            .OrderByDescending(rp => rp.CreatedAt)
            .Select(rp =>
            {
                var module = moduleByKey.TryGetValue(rp.ModuleKey, out var m) ? m : null;
                var roleCode = rp.Role.Code ?? rp.Role.Name;
                return new PermissionDto
                {
                    Id = rp.Id,
                    Name = $"{roleCode}:{rp.ModuleKey}",
                    Description = null,
                    Code = null,
                    TenantId = rp.Role.TenantId,
                    RoleId = rp.RoleId,
                    ModuleId = module?.Id,
                    ModuleName = module?.Name,
                    IsActive = rp.IsActive,
                    CreatedAt = rp.CreatedAt,
                    UpdatedAt = rp.UpdatedAt,
                    Operations = rp.Actions.Select(a => new OperationDto
                    {
                        Id = a,
                        Name = a,
                        Code = a,
                        Value = a,
                        IsActive = true,
                        CreatedAt = rp.CreatedAt,
                        UpdatedAt = rp.UpdatedAt
                    }).ToList()
                };
            })
            .ToList();

        return Ok(new PagedResultDto<PermissionDto>
        {
            Items = items,
            Page = page,
            Limit = limit,
            Total = total,
            TotalPages = totalPages
        });
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<PermissionDto>> GetPermissionById([FromRoute] Guid id)
    {
        var rp = await _dbContext.RolePermissions.AsNoTracking().Include(x => x.Role).FirstOrDefaultAsync(x => x.Id == id);
        if (rp == null)
        {
            return NotFound();
        }

        var module = await _dbContext.Modules.AsNoTracking()
            .Where(m => m.Key != null && m.Key == rp.ModuleKey)
            .Select(m => new { m.Id, m.Name })
            .FirstOrDefaultAsync();

        var roleCode = rp.Role.Code ?? rp.Role.Name;
        return Ok(new PermissionDto
        {
            Id = rp.Id,
            Name = $"{roleCode}:{rp.ModuleKey}",
            TenantId = rp.Role.TenantId,
            RoleId = rp.RoleId,
            ModuleId = module?.Id,
            ModuleName = module?.Name,
            IsActive = rp.IsActive,
            CreatedAt = rp.CreatedAt,
            UpdatedAt = rp.UpdatedAt,
            Operations = rp.Actions.Select(a => new OperationDto
            {
                Id = a,
                Name = a,
                Code = a,
                Value = a,
                IsActive = true,
                CreatedAt = rp.CreatedAt,
                UpdatedAt = rp.UpdatedAt
            }).ToList()
        });
    }

    [HttpPost]
    [Authorize(Roles = "SUPER_ADMIN")]
    public async Task<ActionResult<PermissionDto>> Create([FromBody] CreatePermissionRequestDto request)
    {
        if (!request.RoleId.HasValue || request.RoleId.Value == Guid.Empty)
        {
            return BadRequest();
        }

        if (request.ModuleId == Guid.Empty)
        {
            return BadRequest();
        }

        var role = await _dbContext.Roles.AsNoTracking().FirstOrDefaultAsync(r => r.Id == request.RoleId.Value);
        if (role == null)
        {
            return BadRequest();
        }

        var module = await _dbContext.Modules.AsNoTracking().FirstOrDefaultAsync(m => m.Id == request.ModuleId);
        if (module == null || string.IsNullOrWhiteSpace(module.Key))
        {
            return BadRequest();
        }

        var actions = NormalizeOperationIds(request.OperationIds);
        if (actions.Count == 0)
        {
            actions = ["READ"];
        }

        var existing = await _dbContext.RolePermissions.FirstOrDefaultAsync(rp =>
            rp.RoleId == request.RoleId.Value &&
            rp.ModuleKey == module.Key);

        if (existing != null)
        {
            existing.Actions = actions;
            existing.IsActive = request.IsActive ?? true;
            existing.UpdatedAt = DateTime.UtcNow;
            await _dbContext.SaveChangesAsync();
            return await GetPermissionById(existing.Id);
        }

        var rpEntity = new RolePermissionEntity
        {
            Id = Guid.NewGuid(),
            RoleId = request.RoleId.Value,
            ModuleKey = module.Key!,
            Actions = actions,
            IsActive = request.IsActive ?? true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = null
        };

        _dbContext.RolePermissions.Add(rpEntity);
        await _dbContext.SaveChangesAsync();

        return await GetPermissionById(rpEntity.Id);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "SUPER_ADMIN")]
    public async Task<ActionResult<PermissionDto>> Update([FromRoute] Guid id, [FromBody] UpdatePermissionRequestDto request)
    {
        var rp = await _dbContext.RolePermissions.FirstOrDefaultAsync(x => x.Id == id);
        if (rp == null)
        {
            return NotFound();
        }

        if (request.ModuleId == Guid.Empty)
        {
            return BadRequest();
        }

        var module = await _dbContext.Modules.AsNoTracking().FirstOrDefaultAsync(m => m.Id == request.ModuleId);
        if (module == null || string.IsNullOrWhiteSpace(module.Key))
        {
            return BadRequest();
        }

        if (request.RoleId.HasValue && request.RoleId.Value != Guid.Empty)
        {
            var roleExists = await _dbContext.Roles.AsNoTracking().AnyAsync(r => r.Id == request.RoleId.Value);
            if (!roleExists)
            {
                return BadRequest();
            }

            rp.RoleId = request.RoleId.Value;
        }

        rp.ModuleKey = module.Key!;
        rp.Actions = NormalizeOperationIds(request.OperationIds);
        rp.IsActive = request.IsActive ?? rp.IsActive;
        rp.UpdatedAt = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync();
        return await GetPermissionById(id);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "SUPER_ADMIN")]
    public async Task<IActionResult> Delete([FromRoute] Guid id)
    {
        var rp = await _dbContext.RolePermissions.FirstOrDefaultAsync(x => x.Id == id);
        if (rp == null)
        {
            return NoContent();
        }

        _dbContext.RolePermissions.Remove(rp);
        await _dbContext.SaveChangesAsync();
        return NoContent();
    }

    private static List<string> NormalizeOperationIds(List<string>? operationIds)
    {
        var allowed = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "CREATE", "READ", "UPDATE", "DELETE", "SELECT" };

        if (operationIds == null || operationIds.Count == 0)
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

