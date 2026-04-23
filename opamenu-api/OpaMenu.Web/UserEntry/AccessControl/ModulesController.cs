using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OpaMenu.Domain.DTOs.AccessControl;
using OpaMenu.Infrastructure.Shared.Data.Context.AccessControl;
using OpaMenu.Infrastructure.Shared.Entities.AccessControl;

namespace OpaMenu.Web.UserEntry.AccessControl;

[ApiController]
[Route("api/modules")]
[Authorize(Roles = "ADMIN,SUPER_ADMIN")]
public sealed class ModulesController(AccessControlDbContext dbContext) : ControllerBase
{
    private readonly AccessControlDbContext _dbContext = dbContext;

    [HttpGet]
    public async Task<ActionResult<PagedResultDto<ModuleDto>>> GetModules(
        [FromQuery] int page = 1,
        [FromQuery] int limit = 10,
        [FromQuery] string? search = null,
        [FromQuery] bool? isActive = null,
        [FromQuery] string? sortBy = null,
        [FromQuery] string? sortOrder = null)
    {
        page = page <= 0 ? 1 : page;
        limit = limit <= 0 ? 10 : limit;

        var query = _dbContext.Modules.AsNoTracking().Include(m => m.Application).AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.Trim();
            query = query.Where(m =>
                m.Name.Contains(s) ||
                m.Description.Contains(s) ||
                (m.Key != null && m.Key.Contains(s)) ||
                (m.Code != null && m.Code.Contains(s)));
        }

        if (isActive.HasValue)
        {
            query = query.Where(m => m.IsActive == isActive.Value);
        }

        query = ApplyModuleSorting(query, sortBy, sortOrder);

        var total = await query.CountAsync();
        var totalPages = (int)Math.Ceiling(total / (double)limit);

        var items = await query
            .Skip((page - 1) * limit)
            .Take(limit)
            .Select(m => new ModuleDto
            {
                Id = m.Id,
                Name = m.Name,
                Description = m.Description,
                Url = m.Url,
                Key = m.Key,
                Code = m.Code,
                ApplicationId = m.ApplicationId,
                ApplicationName = m.Application != null ? m.Application.Name : null,
                IsActive = m.IsActive,
                CreatedAt = m.CreatedAt,
                UpdatedAt = m.UpdatedAt
            })
            .ToListAsync();

        return Ok(new PagedResultDto<ModuleDto>
        {
            Items = items,
            Page = page,
            Limit = limit,
            Total = total,
            TotalPages = totalPages
        });
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ModuleDto>> GetModuleById([FromRoute] Guid id)
    {
        var module = await _dbContext.Modules.AsNoTracking().Include(m => m.Application).FirstOrDefaultAsync(m => m.Id == id);
        if (module == null)
        {
            return NotFound();
        }

        return Ok(new ModuleDto
        {
            Id = module.Id,
            Name = module.Name,
            Description = module.Description,
            Url = module.Url,
            Key = module.Key,
            Code = module.Code,
            ApplicationId = module.ApplicationId,
            ApplicationName = module.Application?.Name,
            IsActive = module.IsActive,
            CreatedAt = module.CreatedAt,
            UpdatedAt = module.UpdatedAt
        });
    }

    [HttpPost]
    [Authorize(Roles = "SUPER_ADMIN")]
    public async Task<ActionResult<ModuleDto>> Create([FromBody] CreateModuleRequestDto request)
    {
        if (string.IsNullOrWhiteSpace(request.Name) || string.IsNullOrWhiteSpace(request.Key))
        {
            return BadRequest();
        }

        var existsKey = await _dbContext.Modules.AsNoTracking().AnyAsync(m => m.Key != null && m.Key == request.Key);
        if (existsKey)
        {
            return BadRequest();
        }

        var entity = new ModuleEntity
        {
            Id = Guid.NewGuid(),
            Name = request.Name.Trim(),
            Description = request.Description ?? string.Empty,
            Url = request.Url ?? string.Empty,
            Key = request.Key.Trim(),
            Code = request.Code,
            ApplicationId = request.ApplicationId,
            IsActive = request.IsActive,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = null
        };

        _dbContext.Modules.Add(entity);
        await _dbContext.SaveChangesAsync();

        var appName = entity.ApplicationId.HasValue
            ? await _dbContext.Applications.AsNoTracking().Where(a => a.Id == entity.ApplicationId.Value).Select(a => a.Name).FirstOrDefaultAsync()
            : null;

        return Ok(new ModuleDto
        {
            Id = entity.Id,
            Name = entity.Name,
            Description = entity.Description,
            Url = entity.Url,
            Key = entity.Key,
            Code = entity.Code,
            ApplicationId = entity.ApplicationId,
            ApplicationName = appName,
            IsActive = entity.IsActive,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        });
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "SUPER_ADMIN")]
    public async Task<ActionResult<ModuleDto>> Update([FromRoute] Guid id, [FromBody] UpdateModuleRequestDto request)
    {
        var entity = await _dbContext.Modules.FirstOrDefaultAsync(m => m.Id == id);
        if (entity == null)
        {
            return NotFound();
        }

        if (string.IsNullOrWhiteSpace(request.Name) || string.IsNullOrWhiteSpace(request.Key))
        {
            return BadRequest();
        }

        var existsKey = await _dbContext.Modules.AsNoTracking().AnyAsync(m => m.Id != id && m.Key != null && m.Key == request.Key);
        if (existsKey)
        {
            return BadRequest();
        }

        entity.Name = request.Name.Trim();
        entity.Description = request.Description ?? string.Empty;
        entity.Url = request.Url ?? string.Empty;
        entity.Key = request.Key.Trim();
        entity.Code = request.Code;
        entity.ApplicationId = request.ApplicationId;
        entity.IsActive = request.IsActive;
        entity.UpdatedAt = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync();

        var appName = entity.ApplicationId.HasValue
            ? await _dbContext.Applications.AsNoTracking().Where(a => a.Id == entity.ApplicationId.Value).Select(a => a.Name).FirstOrDefaultAsync()
            : null;

        return Ok(new ModuleDto
        {
            Id = entity.Id,
            Name = entity.Name,
            Description = entity.Description,
            Url = entity.Url,
            Key = entity.Key,
            Code = entity.Code,
            ApplicationId = entity.ApplicationId,
            ApplicationName = appName,
            IsActive = entity.IsActive,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        });
    }

    [HttpPatch("{id:guid}/toggle-status")]
    [Authorize(Roles = "SUPER_ADMIN")]
    public async Task<ActionResult<ModuleDto>> ToggleStatus([FromRoute] Guid id)
    {
        var entity = await _dbContext.Modules.FirstOrDefaultAsync(m => m.Id == id);
        if (entity == null)
        {
            return NotFound();
        }

        entity.IsActive = !entity.IsActive;
        entity.UpdatedAt = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync();

        var appName = entity.ApplicationId.HasValue
            ? await _dbContext.Applications.AsNoTracking().Where(a => a.Id == entity.ApplicationId.Value).Select(a => a.Name).FirstOrDefaultAsync()
            : null;

        return Ok(new ModuleDto
        {
            Id = entity.Id,
            Name = entity.Name,
            Description = entity.Description,
            Url = entity.Url,
            Key = entity.Key,
            Code = entity.Code,
            ApplicationId = entity.ApplicationId,
            ApplicationName = appName,
            IsActive = entity.IsActive,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        });
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "SUPER_ADMIN")]
    public async Task<IActionResult> Delete([FromRoute] Guid id)
    {
        var entity = await _dbContext.Modules.FirstOrDefaultAsync(m => m.Id == id);
        if (entity == null)
        {
            return NoContent();
        }

        _dbContext.Modules.Remove(entity);
        await _dbContext.SaveChangesAsync();
        return NoContent();
    }

    private static IQueryable<ModuleEntity> ApplyModuleSorting(IQueryable<ModuleEntity> query, string? sortBy, string? sortOrder)
    {
        var desc = string.Equals(sortOrder, "desc", StringComparison.OrdinalIgnoreCase);

        return (sortBy ?? string.Empty).Trim().ToLowerInvariant() switch
        {
            "name" => desc ? query.OrderByDescending(m => m.Name) : query.OrderBy(m => m.Name),
            "key" => desc ? query.OrderByDescending(m => m.Key) : query.OrderBy(m => m.Key),
            "code" => desc ? query.OrderByDescending(m => m.Code) : query.OrderBy(m => m.Code),
            "isactive" => desc ? query.OrderByDescending(m => m.IsActive) : query.OrderBy(m => m.IsActive),
            _ => query.OrderBy(m => m.Name)
        };
    }
}

