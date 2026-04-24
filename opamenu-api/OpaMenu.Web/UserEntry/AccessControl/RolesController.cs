using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OpaMenu.Commons.Api.Commons;
using OpaMenu.Domain.DTOs.AccessControl;
using OpaMenu.Domain.Interfaces;
using OpaMenu.Infrastructure.Shared.Data.Context.AccessControl;
using OpaMenu.Infrastructure.Shared.Entities.AccessControl;

namespace OpaMenu.Web.UserEntry.AccessControl;

[ApiController]
[Route("api/roles")]
[Authorize(Roles = "ADMIN,SUPER_ADMIN")]
public sealed class RolesController(AccessControlDbContext dbContext, ICurrentUserService currentUserService) : ControllerBase
{
    private readonly AccessControlDbContext _dbContext = dbContext;
    private readonly ICurrentUserService _currentUserService = currentUserService;

    [HttpGet]
    public async Task<ActionResult<PagedResultDto<RoleDto>>> GetRoles(
        [FromQuery] int page = 1,
        [FromQuery] int limit = 10,
        [FromQuery] string? search = null)
    {
        page = page <= 0 ? 1 : page;
        limit = limit <= 0 ? 10 : limit;

        var query = _dbContext.Roles.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.Trim();
            query = query.Where(r =>
                r.Name.Contains(s) ||
                r.Description.Contains(s) ||
                (r.Code != null && r.Code.Contains(s)));
        }

        var total = await query.CountAsync();
        var totalPages = (int)Math.Ceiling(total / (double)limit);

        var roleIds = await query
            .OrderByDescending(r => r.CreatedAt)
            .Skip((page - 1) * limit)
            .Take(limit)
            .Select(r => r.Id)
            .ToListAsync();

        var roles = await _dbContext.Roles
            .AsNoTracking()
            .Where(r => roleIds.Contains(r.Id))
            .ToListAsync();

        var permissions = await _dbContext.RolePermissions
            .AsNoTracking()
            .Where(rp => roleIds.Contains(rp.RoleId) && rp.IsActive)
            .ToListAsync();

        var items = roles
            .OrderByDescending(r => r.CreatedAt)
            .Select(r => new RoleDto
            {
                Id = r.Id,
                Name = r.Name,
                Description = r.Description,
                Code = r.Code,
                TenantId = r.TenantId,
                ApplicationId = r.ApplicationId,
                IsActive = r.IsActive,
                CreatedAt = r.CreatedAt,
                UpdatedAt = r.UpdatedAt,
                Permissions = permissions
                    .Where(p => p.RoleId == r.Id)
                    .Select(p => new SimplifiedPermissionDto { Module = p.ModuleKey, Actions = p.Actions })
                    .ToList()
            })
            .ToList();

        return Ok(new PagedResultDto<RoleDto>
        {
            Items = items,
            Page = page,
            Limit = limit,
            Total = total,
            TotalPages = totalPages
        });
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<RoleDto>> GetById([FromRoute] Guid id)
    {
        var role = await _dbContext.Roles.AsNoTracking().FirstOrDefaultAsync(r => r.Id == id);
        if (role == null)
        {
            return NotFound();
        }

        var permissions = await _dbContext.RolePermissions.AsNoTracking()
            .Where(rp => rp.RoleId == id && rp.IsActive)
            .ToListAsync();

        return Ok(new RoleDto
        {
            Id = role.Id,
            Name = role.Name,
            Description = role.Description,
            Code = role.Code,
            TenantId = role.TenantId,
            ApplicationId = role.ApplicationId,
            IsActive = role.IsActive,
            CreatedAt = role.CreatedAt,
            UpdatedAt = role.UpdatedAt,
            Permissions = permissions.Select(p => new SimplifiedPermissionDto { Module = p.ModuleKey, Actions = p.Actions }).ToList()
        });
    }

    [HttpPost]
    public async Task<ActionResult<RoleDto>> Create([FromBody] CreateRoleRequestDto request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return BadRequest();
        }

        if (!string.IsNullOrWhiteSpace(request.Code))
        {
            var exists = await _dbContext.Roles.AsNoTracking().AnyAsync(r => r.Code != null && r.Code == request.Code);
            if (exists)
            {
                return BadRequest();
            }
        }

        var role = new RoleEntity
        {
            Id = Guid.NewGuid(),
            Name = request.Name.Trim(),
            Description = request.Description ?? string.Empty,
            Code = request.Code,
            TenantId = request.TenantId,
            ApplicationId = request.ApplicationId,
            IsActive = request.IsActive ?? true,
            IsSystem = false,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = null
        };

        _dbContext.Roles.Add(role);

        var permissions = NormalizePermissions(request.Permissions);
        if (permissions.Count > 0)
        {
            foreach (var p in permissions)
            {
                _dbContext.RolePermissions.Add(new RolePermissionEntity
                {
                    Id = Guid.NewGuid(),
                    RoleId = role.Id,
                    ModuleKey = p.Module,
                    Actions = p.Actions,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = null
                });
            }
        }

        await _dbContext.SaveChangesAsync();

        return Ok(new RoleDto
        {
            Id = role.Id,
            Name = role.Name,
            Description = role.Description,
            Code = role.Code,
            TenantId = role.TenantId,
            ApplicationId = role.ApplicationId,
            IsActive = role.IsActive,
            CreatedAt = role.CreatedAt,
            UpdatedAt = role.UpdatedAt,
            Permissions = permissions
        });
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<RoleDto>> Update([FromRoute] Guid id, [FromBody] UpdateRoleRequestDto request)
    {
        var role = await _dbContext.Roles.FirstOrDefaultAsync(r => r.Id == id);
        if (role == null)
        {
            return NotFound();
        }

        if (request.Name != null)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
            {
                return BadRequest();
            }

            role.Name = request.Name.Trim();
        }

        if (request.Description != null)
        {
            role.Description = request.Description;
        }

        if (request.Code != null)
        {
            if (!string.IsNullOrWhiteSpace(request.Code))
            {
                var exists = await _dbContext.Roles.AsNoTracking().AnyAsync(r => r.Id != id && r.Code != null && r.Code == request.Code);
                if (exists)
                {
                    return BadRequest();
                }
            }

            role.Code = request.Code;
        }

        if (request.TenantId.HasValue)
        {
            role.TenantId = request.TenantId;
        }

        if (request.ApplicationId.HasValue)
        {
            role.ApplicationId = request.ApplicationId;
        }

        if (request.IsActive.HasValue)
        {
            role.IsActive = request.IsActive.Value;
        }

        role.UpdatedAt = DateTime.UtcNow;

        List<SimplifiedPermissionDto>? resultingPermissions = null;
        if (request.Permissions != null)
        {
            var permissions = NormalizePermissions(request.Permissions);
            resultingPermissions = permissions;

            var existing = await _dbContext.RolePermissions.Where(rp => rp.RoleId == id).ToListAsync();
            var requestedKeys = permissions.Select(p => p.Module).ToHashSet(StringComparer.OrdinalIgnoreCase);

            foreach (var rp in existing)
            {
                if (!requestedKeys.Contains(rp.ModuleKey))
                {
                    _dbContext.RolePermissions.Remove(rp);
                }
            }

            foreach (var p in permissions)
            {
                var rp = existing.FirstOrDefault(x => string.Equals(x.ModuleKey, p.Module, StringComparison.OrdinalIgnoreCase));
                if (rp == null)
                {
                    _dbContext.RolePermissions.Add(new RolePermissionEntity
                    {
                        Id = Guid.NewGuid(),
                        RoleId = id,
                        ModuleKey = p.Module,
                        Actions = p.Actions,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = null
                    });
                }
                else
                {
                    rp.Actions = p.Actions;
                    rp.IsActive = true;
                    rp.UpdatedAt = DateTime.UtcNow;
                }
            }
        }

        await _dbContext.SaveChangesAsync();

        if (resultingPermissions == null)
        {
            var permissions = await _dbContext.RolePermissions.AsNoTracking()
                .Where(rp => rp.RoleId == id && rp.IsActive)
                .Select(rp => new SimplifiedPermissionDto { Module = rp.ModuleKey, Actions = rp.Actions })
                .ToListAsync();
            resultingPermissions = permissions;
        }

        return Ok(new RoleDto
        {
            Id = role.Id,
            Name = role.Name,
            Description = role.Description,
            Code = role.Code,
            TenantId = role.TenantId,
            ApplicationId = role.ApplicationId,
            IsActive = role.IsActive,
            CreatedAt = role.CreatedAt,
            UpdatedAt = role.UpdatedAt,
            Permissions = resultingPermissions
        });
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete([FromRoute] Guid id)
    {
        var role = await _dbContext.Roles.FirstOrDefaultAsync(r => r.Id == id);
        if (role == null)
        {
            return NoContent();
        }

        var permissions = await _dbContext.RolePermissions.Where(rp => rp.RoleId == id).ToListAsync();
        _dbContext.RolePermissions.RemoveRange(permissions);
        _dbContext.Roles.Remove(role);
        await _dbContext.SaveChangesAsync();

        return NoContent();
    }

    [HttpGet("/api/roles-painel")]
    public async Task<IActionResult> GetRolesPainel(
        [FromQuery] int page = 1,
        [FromQuery] int limit = 10,
        [FromQuery] string? name = null)
    {
        var tenantId = _currentUserService.GetTenantGuid();
        if (!tenantId.HasValue || tenantId.Value == Guid.Empty)
        {
            return BadRequest(StaticResponseBuilder<PagedResultDto<RolePainelDto>>.BuildError("Tenant não identificado."));
        }

        page = page <= 0 ? 1 : page;
        limit = limit <= 0 ? 10 : limit;

        var search = string.IsNullOrWhiteSpace(name) ? Request.Query["search"].ToString() : name;

        var query = _dbContext.Roles
            .AsNoTracking()
            .Where(r => r.TenantId == tenantId.Value || (r.TenantId == null && r.IsSystem))
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.Trim();
            query = query.Where(r => r.Name.Contains(s) || (r.Code != null && r.Code.Contains(s)));
        }

        var total = await query.CountAsync();
        var totalPages = (int)Math.Ceiling(total / (double)limit);

        var roleIds = await query
            .OrderByDescending(r => r.CreatedAt)
            .Skip((page - 1) * limit)
            .Take(limit)
            .Select(r => r.Id)
            .ToListAsync();

        var roles = await _dbContext.Roles
            .AsNoTracking()
            .Where(r => roleIds.Contains(r.Id))
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();

        var permissions = await _dbContext.RolePermissions
            .AsNoTracking()
            .Where(rp => roleIds.Contains(rp.RoleId) && rp.IsActive)
            .ToListAsync();

        var items = roles.Select(r => new RolePainelDto
        {
            Id = r.Id,
            Name = r.Name,
            Description = r.Description,
            Code = r.Code,
            IsDefault = r.IsSystem,
            IsActive = r.IsActive,
            Permissions = permissions
                .Where(p => p.RoleId == r.Id)
                .Select(p => new SimplifiedPermissionDto { Module = p.ModuleKey, Actions = p.Actions })
                .ToList()
        }).ToList();

        return Ok(StaticResponseBuilder<PagedResultDto<RolePainelDto>>.BuildOk(new PagedResultDto<RolePainelDto>
        {
            Items = items,
            Page = page,
            Limit = limit,
            Total = total,
            TotalPages = totalPages
        }));
    }

    [HttpGet("/api/roles-painel/{id:guid}")]
    public async Task<IActionResult> GetRolePainelById([FromRoute] Guid id)
    {
        var tenantId = _currentUserService.GetTenantGuid();
        if (!tenantId.HasValue || tenantId.Value == Guid.Empty)
        {
            return BadRequest(StaticResponseBuilder<RolePainelDto>.BuildError("Tenant não identificado."));
        }

        var role = await _dbContext.Roles.AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == id && (r.TenantId == tenantId.Value || (r.TenantId == null && r.IsSystem)));

        if (role == null)
        {
            return NotFound(StaticResponseBuilder<RolePainelDto>.BuildNotFound(new RolePainelDto()));
        }

        var permissions = await _dbContext.RolePermissions.AsNoTracking()
            .Where(rp => rp.RoleId == id && rp.IsActive)
            .Select(rp => new SimplifiedPermissionDto { Module = rp.ModuleKey, Actions = rp.Actions })
            .ToListAsync();

        return Ok(StaticResponseBuilder<RolePainelDto>.BuildOk(new RolePainelDto
        {
            Id = role.Id,
            Name = role.Name,
            Description = role.Description,
            Code = role.Code,
            IsDefault = role.IsSystem,
            IsActive = role.IsActive,
            Permissions = permissions
        }));
    }

    [HttpGet("/api/roles-painel/modules")]
    public async Task<IActionResult> GetAvailableModulesPainel()
    {
        var tenantId = _currentUserService.GetTenantGuid();
        if (!tenantId.HasValue || tenantId.Value == Guid.Empty)
        {
            return BadRequest(StaticResponseBuilder<List<AvailableModuleDto>>.BuildError("Tenant não identificado."));
        }

        var actions = DefaultActions();
        var modules = await _dbContext.Modules.AsNoTracking()
            .Where(m => m.IsActive && m.Key != null)
            .OrderBy(m => m.Name)
            .Select(m => new AvailableModuleDto
            {
                Key = m.Key!,
                Name = m.Name,
                Description = m.Description ?? string.Empty,
                AvailableActions = actions
            })
            .ToListAsync();

        return Ok(StaticResponseBuilder<List<AvailableModuleDto>>.BuildOk(modules));
    }

    [HttpPost("/api/roles-painel")]
    public async Task<IActionResult> CreateRolePainel([FromBody] CreateRoleRequestDto request)
    {
        var tenantId = _currentUserService.GetTenantGuid();
        if (!tenantId.HasValue || tenantId.Value == Guid.Empty)
        {
            return BadRequest(StaticResponseBuilder<RolePainelDto>.BuildError("Tenant não identificado."));
        }

        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return BadRequest(StaticResponseBuilder<RolePainelDto>.BuildError("Nome inválido."));
        }

        if (!string.IsNullOrWhiteSpace(request.Code))
        {
            var code = request.Code.Trim();
            var exists = await _dbContext.Roles.AsNoTracking()
                .AnyAsync(r => r.Code != null && r.Code == code && (r.TenantId == tenantId.Value || r.TenantId == null));
            if (exists)
            {
                return BadRequest(StaticResponseBuilder<RolePainelDto>.BuildError("Código de role já existe."));
            }
        }

        var now = DateTime.UtcNow;
        var role = new RoleEntity
        {
            Id = Guid.NewGuid(),
            Name = request.Name.Trim(),
            Description = request.Description ?? string.Empty,
            Code = request.Code,
            TenantId = tenantId.Value,
            ApplicationId = null,
            IsActive = request.IsActive ?? true,
            IsSystem = false,
            CreatedAt = now,
            UpdatedAt = null
        };

        _dbContext.Roles.Add(role);

        var permissions = NormalizePermissions(request.Permissions);
        foreach (var p in permissions)
        {
            _dbContext.RolePermissions.Add(new RolePermissionEntity
            {
                Id = Guid.NewGuid(),
                RoleId = role.Id,
                ModuleKey = p.Module,
                Actions = p.Actions,
                IsActive = true,
                CreatedAt = now,
                UpdatedAt = null
            });
        }

        await _dbContext.SaveChangesAsync();

        return Ok(StaticResponseBuilder<RolePainelDto>.BuildOk(new RolePainelDto
        {
            Id = role.Id,
            Name = role.Name,
            Description = role.Description,
            Code = role.Code,
            IsDefault = false,
            IsActive = role.IsActive,
            Permissions = permissions
        }));
    }

    [HttpPut("/api/roles-painel/{id:guid}")]
    public async Task<IActionResult> UpdateRolePainel([FromRoute] Guid id, [FromBody] UpdateRoleRequestDto request)
    {
        var tenantId = _currentUserService.GetTenantGuid();
        if (!tenantId.HasValue || tenantId.Value == Guid.Empty)
        {
            return BadRequest(StaticResponseBuilder<RolePainelDto>.BuildError("Tenant não identificado."));
        }

        var role = await _dbContext.Roles.FirstOrDefaultAsync(r => r.Id == id && r.TenantId == tenantId.Value);
        if (role == null)
        {
            return NotFound(StaticResponseBuilder<RolePainelDto>.BuildNotFound(new RolePainelDto()));
        }

        if (role.IsSystem)
        {
            return BadRequest(StaticResponseBuilder<RolePainelDto>.BuildError("Role padrão não pode ser alterada."));
        }

        if (request.Name != null)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
            {
                return BadRequest(StaticResponseBuilder<RolePainelDto>.BuildError("Nome inválido."));
            }
            role.Name = request.Name.Trim();
        }

        if (request.Description != null)
        {
            role.Description = request.Description;
        }

        if (request.Code != null)
        {
            if (!string.IsNullOrWhiteSpace(request.Code))
            {
                var code = request.Code.Trim();
                var exists = await _dbContext.Roles.AsNoTracking()
                    .AnyAsync(r => r.Id != id && r.Code != null && r.Code == code && (r.TenantId == tenantId.Value || r.TenantId == null));
                if (exists)
                {
                    return BadRequest(StaticResponseBuilder<RolePainelDto>.BuildError("Código de role já existe."));
                }
            }

            role.Code = request.Code;
        }

        if (request.IsActive.HasValue)
        {
            role.IsActive = request.IsActive.Value;
        }

        role.UpdatedAt = DateTime.UtcNow;

        List<SimplifiedPermissionDto> permissions;
        if (request.Permissions != null)
        {
            permissions = NormalizePermissions(request.Permissions);

            var existing = await _dbContext.RolePermissions.Where(rp => rp.RoleId == id).ToListAsync();
            var requestedKeys = permissions.Select(p => p.Module).ToHashSet(StringComparer.OrdinalIgnoreCase);

            foreach (var rp in existing)
            {
                if (!requestedKeys.Contains(rp.ModuleKey))
                {
                    _dbContext.RolePermissions.Remove(rp);
                }
            }

            foreach (var p in permissions)
            {
                var rp = existing.FirstOrDefault(x => string.Equals(x.ModuleKey, p.Module, StringComparison.OrdinalIgnoreCase));
                if (rp == null)
                {
                    _dbContext.RolePermissions.Add(new RolePermissionEntity
                    {
                        Id = Guid.NewGuid(),
                        RoleId = id,
                        ModuleKey = p.Module,
                        Actions = p.Actions,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = null
                    });
                }
                else
                {
                    rp.Actions = p.Actions;
                    rp.IsActive = true;
                    rp.UpdatedAt = DateTime.UtcNow;
                }
            }
        }
        else
        {
            permissions = await _dbContext.RolePermissions.AsNoTracking()
                .Where(rp => rp.RoleId == id && rp.IsActive)
                .Select(rp => new SimplifiedPermissionDto { Module = rp.ModuleKey, Actions = rp.Actions })
                .ToListAsync();
        }

        await _dbContext.SaveChangesAsync();

        return Ok(StaticResponseBuilder<RolePainelDto>.BuildOk(new RolePainelDto
        {
            Id = role.Id,
            Name = role.Name,
            Description = role.Description,
            Code = role.Code,
            IsDefault = role.IsSystem,
            IsActive = role.IsActive,
            Permissions = permissions
        }));
    }

    [HttpDelete("/api/roles-painel/{id:guid}")]
    public async Task<IActionResult> DeleteRolePainel([FromRoute] Guid id)
    {
        var tenantId = _currentUserService.GetTenantGuid();
        if (!tenantId.HasValue || tenantId.Value == Guid.Empty)
        {
            return BadRequest(StaticResponseBuilder<bool>.BuildError("Tenant não identificado."));
        }

        var role = await _dbContext.Roles.FirstOrDefaultAsync(r => r.Id == id && r.TenantId == tenantId.Value);
        if (role == null)
        {
            return Ok(StaticResponseBuilder<bool>.BuildOk(true));
        }

        if (role.IsSystem)
        {
            return BadRequest(StaticResponseBuilder<bool>.BuildError("Role padrão não pode ser removida."));
        }

        role.IsActive = false;
        role.UpdatedAt = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync();

        return Ok(StaticResponseBuilder<bool>.BuildOk(true));
    }

    private static List<string> DefaultActions() => ["CREATE", "READ", "UPDATE", "DELETE"];

    private static List<SimplifiedPermissionDto> NormalizePermissions(List<SimplifiedPermissionDto>? permissions)
    {
        if (permissions == null || permissions.Count == 0)
        {
            return [];
        }

        return permissions
            .Where(p => !string.IsNullOrWhiteSpace(p.Module))
            .GroupBy(p => p.Module.Trim(), StringComparer.OrdinalIgnoreCase)
            .Select(g => new SimplifiedPermissionDto
            {
                Module = g.Key,
                Actions = g
                    .SelectMany(x => x.Actions ?? [])
                    .Where(a => !string.IsNullOrWhiteSpace(a))
                    .Select(a => a.Trim())
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .ToList()
            })
            .ToList();
    }
}
