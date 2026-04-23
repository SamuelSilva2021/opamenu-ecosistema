using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OpaMenu.Domain.DTOs.AccessControl;
using OpaMenu.Infrastructure.Shared.Data.Context.AccessControl;
using OpaMenu.Infrastructure.Shared.Entities.AccessControl;

namespace OpaMenu.Web.UserEntry.AccessControl;

[ApiController]
[Route("api/access-group")]
[Authorize(Roles = "ADMIN,SUPER_ADMIN")]
public sealed class AccessGroupController(AccessControlDbContext dbContext) : ControllerBase
{
    private readonly AccessControlDbContext _dbContext = dbContext;

    [HttpGet]
    public async Task<ActionResult<PagedResultDto<AccessGroupDto>>> GetAccessGroups(
        [FromQuery] int page = 1,
        [FromQuery] int limit = 10,
        [FromQuery] string? search = null)
    {
        page = page <= 0 ? 1 : page;
        limit = limit <= 0 ? 10 : limit;

        var query = _dbContext.AccessGroups.AsNoTracking().Include(g => g.GroupType).AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.Trim();
            query = query.Where(g =>
                g.Name.Contains(s) ||
                g.Description.Contains(s) ||
                (g.Code != null && g.Code.Contains(s)));
        }

        var total = await query.CountAsync();
        var totalPages = (int)Math.Ceiling(total / (double)limit);

        var items = await query
            .OrderByDescending(g => g.CreatedAt)
            .Skip((page - 1) * limit)
            .Take(limit)
            .Select(g => new AccessGroupDto
            {
                Id = g.Id,
                Name = g.Name,
                Description = g.Description,
                Code = g.Code,
                TenantId = g.TenantId,
                GroupTypeId = g.GroupTypeId,
                GroupTypeName = g.GroupType != null ? g.GroupType.Name : null,
                IsActive = g.IsActive,
                CreatedAt = g.CreatedAt,
                UpdatedAt = g.UpdatedAt
            })
            .ToListAsync();

        return Ok(new PagedResultDto<AccessGroupDto>
        {
            Items = items,
            Page = page,
            Limit = limit,
            Total = total,
            TotalPages = totalPages
        });
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<AccessGroupDto>> GetById([FromRoute] Guid id)
    {
        var group = await _dbContext.AccessGroups.AsNoTracking().Include(g => g.GroupType).FirstOrDefaultAsync(g => g.Id == id);
        if (group == null)
        {
            return NotFound();
        }

        return Ok(new AccessGroupDto
        {
            Id = group.Id,
            Name = group.Name,
            Description = group.Description,
            Code = group.Code,
            TenantId = group.TenantId,
            GroupTypeId = group.GroupTypeId,
            GroupTypeName = group.GroupType?.Name,
            IsActive = group.IsActive,
            CreatedAt = group.CreatedAt,
            UpdatedAt = group.UpdatedAt
        });
    }

    [HttpPost]
    public async Task<ActionResult<AccessGroupDto>> Create([FromBody] CreateAccessGroupRequestDto request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return BadRequest();
        }

        var groupType = await _dbContext.GroupTypes.AsNoTracking().FirstOrDefaultAsync(gt => gt.Id == request.GroupTypeId);
        if (groupType == null)
        {
            return BadRequest();
        }

        var entity = new AccessGroupEntity
        {
            Id = Guid.NewGuid(),
            Name = request.Name.Trim(),
            Description = request.Description ?? string.Empty,
            Code = request.Code,
            TenantId = request.TenantId,
            GroupTypeId = request.GroupTypeId,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = null
        };

        _dbContext.AccessGroups.Add(entity);
        await _dbContext.SaveChangesAsync();

        return Ok(new AccessGroupDto
        {
            Id = entity.Id,
            Name = entity.Name,
            Description = entity.Description,
            Code = entity.Code,
            TenantId = entity.TenantId,
            GroupTypeId = entity.GroupTypeId,
            GroupTypeName = groupType.Name,
            IsActive = entity.IsActive,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        });
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<AccessGroupDto>> Update([FromRoute] Guid id, [FromBody] UpdateAccessGroupRequestDto request)
    {
        var entity = await _dbContext.AccessGroups.FirstOrDefaultAsync(g => g.Id == id);
        if (entity == null)
        {
            return NotFound();
        }

        if (request.Name != null)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
            {
                return BadRequest();
            }

            entity.Name = request.Name.Trim();
        }

        if (request.Description != null)
        {
            entity.Description = request.Description;
        }

        if (request.Code != null)
        {
            entity.Code = request.Code;
        }

        if (request.TenantId.HasValue)
        {
            entity.TenantId = request.TenantId;
        }

        if (request.GroupTypeId.HasValue)
        {
            var existsGroupType = await _dbContext.GroupTypes.AsNoTracking().AnyAsync(gt => gt.Id == request.GroupTypeId.Value);
            if (!existsGroupType)
            {
                return BadRequest();
            }

            entity.GroupTypeId = request.GroupTypeId;
        }

        if (request.IsActive.HasValue)
        {
            entity.IsActive = request.IsActive.Value;
        }

        entity.UpdatedAt = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync();

        var groupTypeName = entity.GroupTypeId.HasValue
            ? await _dbContext.GroupTypes.AsNoTracking().Where(gt => gt.Id == entity.GroupTypeId.Value).Select(gt => gt.Name).FirstOrDefaultAsync()
            : null;

        return Ok(new AccessGroupDto
        {
            Id = entity.Id,
            Name = entity.Name,
            Description = entity.Description,
            Code = entity.Code,
            TenantId = entity.TenantId,
            GroupTypeId = entity.GroupTypeId,
            GroupTypeName = groupTypeName,
            IsActive = entity.IsActive,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        });
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete([FromRoute] Guid id)
    {
        var entity = await _dbContext.AccessGroups.FirstOrDefaultAsync(g => g.Id == id);
        if (entity == null)
        {
            return NoContent();
        }

        _dbContext.AccessGroups.Remove(entity);
        await _dbContext.SaveChangesAsync();
        return NoContent();
    }

    [HttpGet("group-types")]
    public async Task<ActionResult<List<GroupTypeDto>>> GetGroupTypes()
    {
        var types = await _dbContext.GroupTypes.AsNoTracking()
            .OrderBy(gt => gt.Name)
            .Select(gt => new GroupTypeDto
            {
                Id = gt.Id,
                Name = gt.Name,
                Description = gt.Description,
                Code = gt.Code,
                IsActive = gt.IsActive,
                CreatedAt = gt.CreatedAt,
                UpdatedAt = gt.UpdatedAt
            })
            .ToListAsync();

        return Ok(types);
    }

    [HttpGet("group-types/{id:guid}")]
    public async Task<ActionResult<GroupTypeDto>> GetGroupTypeById([FromRoute] Guid id)
    {
        var gt = await _dbContext.GroupTypes.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        if (gt == null)
        {
            return NotFound();
        }

        return Ok(new GroupTypeDto
        {
            Id = gt.Id,
            Name = gt.Name,
            Description = gt.Description,
            Code = gt.Code,
            IsActive = gt.IsActive,
            CreatedAt = gt.CreatedAt,
            UpdatedAt = gt.UpdatedAt
        });
    }

    [HttpPost("group-types")]
    public async Task<ActionResult<GroupTypeDto>> CreateGroupType([FromBody] CreateGroupTypeRequestDto request)
    {
        if (string.IsNullOrWhiteSpace(request.Name) || string.IsNullOrWhiteSpace(request.Code))
        {
            return BadRequest();
        }

        var existsCode = await _dbContext.GroupTypes.AsNoTracking().AnyAsync(gt => gt.Code == request.Code);
        if (existsCode)
        {
            return BadRequest();
        }

        var entity = new GroupTypeEntity
        {
            Id = Guid.NewGuid(),
            Name = request.Name.Trim(),
            Description = request.Description,
            Code = request.Code.Trim(),
            IsActive = request.IsActive ?? true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = null
        };

        _dbContext.GroupTypes.Add(entity);
        await _dbContext.SaveChangesAsync();

        return Ok(new GroupTypeDto
        {
            Id = entity.Id,
            Name = entity.Name,
            Description = entity.Description,
            Code = entity.Code,
            IsActive = entity.IsActive,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        });
    }

    [HttpPut("group-types/{id:guid}")]
    public async Task<ActionResult<GroupTypeDto>> UpdateGroupType([FromRoute] Guid id, [FromBody] UpdateGroupTypeRequestDto request)
    {
        var entity = await _dbContext.GroupTypes.FirstOrDefaultAsync(gt => gt.Id == id);
        if (entity == null)
        {
            return NotFound();
        }

        if (request.Name != null)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
            {
                return BadRequest();
            }

            entity.Name = request.Name.Trim();
        }

        if (request.Description != null)
        {
            entity.Description = request.Description;
        }

        if (request.Code != null)
        {
            if (string.IsNullOrWhiteSpace(request.Code))
            {
                return BadRequest();
            }

            var existsCode = await _dbContext.GroupTypes.AsNoTracking().AnyAsync(gt => gt.Id != id && gt.Code == request.Code);
            if (existsCode)
            {
                return BadRequest();
            }

            entity.Code = request.Code.Trim();
        }

        if (request.IsActive.HasValue)
        {
            entity.IsActive = request.IsActive.Value;
        }

        entity.UpdatedAt = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync();

        return Ok(new GroupTypeDto
        {
            Id = entity.Id,
            Name = entity.Name,
            Description = entity.Description,
            Code = entity.Code,
            IsActive = entity.IsActive,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        });
    }

    [HttpDelete("group-types/{id:guid}")]
    public async Task<IActionResult> DeleteGroupType([FromRoute] Guid id)
    {
        var entity = await _dbContext.GroupTypes.FirstOrDefaultAsync(gt => gt.Id == id);
        if (entity == null)
        {
            return NoContent();
        }

        _dbContext.GroupTypes.Remove(entity);
        await _dbContext.SaveChangesAsync();
        return NoContent();
    }
}

