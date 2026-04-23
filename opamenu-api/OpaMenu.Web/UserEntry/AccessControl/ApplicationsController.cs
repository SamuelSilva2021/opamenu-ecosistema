using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OpaMenu.Domain.DTOs.AccessControl;
using OpaMenu.Infrastructure.Shared.Data.Context.AccessControl;
using OpaMenu.Infrastructure.Shared.Entities.AccessControl;

namespace OpaMenu.Web.UserEntry.AccessControl;

[ApiController]
[Route("api/applications")]
[Authorize(Roles = "ADMIN,SUPER_ADMIN")]
public sealed class ApplicationsController(AccessControlDbContext dbContext) : ControllerBase
{
    private readonly AccessControlDbContext _dbContext = dbContext;

    [HttpGet]
    public async Task<ActionResult<PagedResultDto<ApplicationDto>>> GetApplications(
        [FromQuery] int page = 1,
        [FromQuery] int limit = 10,
        [FromQuery] string? search = null)
    {
        page = page <= 0 ? 1 : page;
        limit = limit <= 0 ? 10 : limit;

        var query = _dbContext.Applications.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.Trim();
            query = query.Where(a =>
                a.Name.Contains(s) ||
                a.Description.Contains(s) ||
                (a.Code != null && a.Code.Contains(s)));
        }

        var total = await query.CountAsync();
        var totalPages = (int)Math.Ceiling(total / (double)limit);

        var items = await query
            .OrderBy(a => a.Name)
            .Skip((page - 1) * limit)
            .Take(limit)
            .Select(a => new ApplicationDto
            {
                Id = a.Id,
                Name = a.Name,
                Description = a.Description,
                Code = a.Code,
                IsActive = a.IsActive,
                CreatedAt = a.CreatedAt,
                UpdatedAt = a.UpdatedAt
            })
            .ToListAsync();

        return Ok(new PagedResultDto<ApplicationDto>
        {
            Items = items,
            Page = page,
            Limit = limit,
            Total = total,
            TotalPages = totalPages
        });
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApplicationDto>> GetById([FromRoute] Guid id)
    {
        var app = await _dbContext.Applications.AsNoTracking().FirstOrDefaultAsync(a => a.Id == id);
        if (app == null)
        {
            return NotFound();
        }

        return Ok(new ApplicationDto
        {
            Id = app.Id,
            Name = app.Name,
            Description = app.Description,
            Code = app.Code,
            IsActive = app.IsActive,
            CreatedAt = app.CreatedAt,
            UpdatedAt = app.UpdatedAt
        });
    }

    [HttpPost]
    [Authorize(Roles = "SUPER_ADMIN")]
    public async Task<ActionResult<ApplicationDto>> Create([FromBody] CreateApplicationRequestDto request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return BadRequest();
        }

        var entity = new ApplicationEntity
        {
            Id = Guid.NewGuid(),
            Name = request.Name.Trim(),
            Description = request.Description ?? string.Empty,
            Code = request.Code,
            Url = string.Empty,
            SecretKey = null,
            AuxiliarSchema = null,
            IsActive = request.IsActive,
            Visible = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = null
        };

        _dbContext.Applications.Add(entity);
        await _dbContext.SaveChangesAsync();

        return Ok(new ApplicationDto
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

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "SUPER_ADMIN")]
    public async Task<ActionResult<ApplicationDto>> Update([FromRoute] Guid id, [FromBody] UpdateApplicationRequestDto request)
    {
        var entity = await _dbContext.Applications.FirstOrDefaultAsync(a => a.Id == id);
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

        if (request.IsActive.HasValue)
        {
            entity.IsActive = request.IsActive.Value;
        }

        entity.UpdatedAt = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync();

        return Ok(new ApplicationDto
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

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "SUPER_ADMIN")]
    public async Task<IActionResult> Delete([FromRoute] Guid id)
    {
        var entity = await _dbContext.Applications.FirstOrDefaultAsync(a => a.Id == id);
        if (entity == null)
        {
            return NoContent();
        }

        _dbContext.Applications.Remove(entity);
        await _dbContext.SaveChangesAsync();
        return NoContent();
    }
}

