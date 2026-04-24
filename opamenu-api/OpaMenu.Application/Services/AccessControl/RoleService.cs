using OpaMenu.Application.Services.Interfaces.AccessControl;
using OpaMenu.Commons.Api.Commons;
using OpaMenu.Commons.Api.DTOs;
using OpaMenu.Domain.DTOs.AccessControl;
using OpaMenu.Domain.Interfaces;
using OpaMenu.Infrastructure.Shared.Entities.AccessControl;

namespace OpaMenu.Application.Services.AccessControl;

public sealed class RoleService(
    IRoleRepository roleRepository,
    IRolePermissionRepository rolePermissionRepository,
    IAccessControlModuleRepository accessControlModuleRepository) : IRoleService
{
    private readonly IRoleRepository _roleRepository = roleRepository;
    private readonly IRolePermissionRepository _rolePermissionRepository = rolePermissionRepository;
    private readonly IAccessControlModuleRepository _accessControlModuleRepository = accessControlModuleRepository;

    public async Task<PagedResultDto<RoleDto>> GetRolesAsync(int page, int limit, string? search)
    {
        page = page <= 0 ? 1 : page;
        limit = limit <= 0 ? 10 : limit;

        var (roles, total) = await _roleRepository.GetPagedAsync(page, limit, search);
        var roleIds = roles.Select(r => r.Id).ToList();
        var permissions = await _rolePermissionRepository.GetActiveByRoleIdsAsync(roleIds);

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

        var totalPages = (int)Math.Ceiling(total / (double)limit);

        return new PagedResultDto<RoleDto>
        {
            Items = items,
            Page = page,
            Limit = limit,
            Total = total,
            TotalPages = totalPages
        };
    }

    public async Task<RoleDto?> GetByIdAsync(Guid id)
    {
        var role = await _roleRepository.GetByIdAsync(id);
        if (role == null)
        {
            return null;
        }

        var permissions = await _rolePermissionRepository.GetActiveByRoleIdAsync(id);

        return new RoleDto
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
        };
    }

    public async Task<RoleDto?> CreateAsync(CreateRoleRequestDto request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return null;
        }

        if (!string.IsNullOrWhiteSpace(request.Code))
        {
            var exists = await _roleRepository.CodeExistsAsync(request.Code);
            if (exists)
            {
                return null;
            }
        }

        var now = DateTime.UtcNow;
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
            CreatedAt = now,
            UpdatedAt = null
        };

        await _roleRepository.AddAsync(role);

        var normalizedPermissions = NormalizePermissions(request.Permissions);
        if (normalizedPermissions.Count > 0)
        {
            var entities = normalizedPermissions.Select(p => new RolePermissionEntity
            {
                Id = Guid.NewGuid(),
                RoleId = role.Id,
                ModuleKey = p.Module,
                Actions = p.Actions,
                IsActive = true,
                CreatedAt = now,
                UpdatedAt = null
            });

            await _rolePermissionRepository.AddRangeAsync(entities);
        }

        await _roleRepository.SaveChangesAsync();

        return new RoleDto
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
            Permissions = normalizedPermissions
        };
    }

    public async Task<(RoleDto? Role, bool NotFound)> UpdateAsync(Guid id, UpdateRoleRequestDto request)
    {
        var role = await _roleRepository.GetByIdTrackedAsync(id);
        if (role == null)
        {
            return (null, true);
        }

        if (request.Name != null)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
            {
                return (null, false);
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
                var exists = await _roleRepository.CodeExistsAsync(request.Code, excludeRoleId: id);
                if (exists)
                {
                    return (null, false);
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
            var normalizedPermissions = NormalizePermissions(request.Permissions);
            resultingPermissions = normalizedPermissions;

            var existing = await _rolePermissionRepository.GetByRoleIdAsync(id);
            var requestedKeys = normalizedPermissions.Select(p => p.Module).ToHashSet(StringComparer.OrdinalIgnoreCase);

            var toRemove = existing.Where(rp => !requestedKeys.Contains(rp.ModuleKey)).ToList();
            if (toRemove.Count > 0)
            {
                await _rolePermissionRepository.RemoveRangeAsync(toRemove);
            }

            foreach (var p in normalizedPermissions)
            {
                var rp = existing.FirstOrDefault(x => string.Equals(x.ModuleKey, p.Module, StringComparison.OrdinalIgnoreCase));
                if (rp == null)
                {
                    await _rolePermissionRepository.AddRangeAsync([new RolePermissionEntity
                    {
                        Id = Guid.NewGuid(),
                        RoleId = id,
                        ModuleKey = p.Module,
                        Actions = p.Actions,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = null
                    }]);
                }
                else
                {
                    rp.Actions = p.Actions;
                    rp.IsActive = true;
                    rp.UpdatedAt = DateTime.UtcNow;
                }
            }
        }

        await _roleRepository.SaveChangesAsync();

        if (resultingPermissions == null)
        {
            var permissions = await _rolePermissionRepository.GetActiveByRoleIdAsync(id);
            resultingPermissions = permissions
                .Select(rp => new SimplifiedPermissionDto { Module = rp.ModuleKey, Actions = rp.Actions })
                .ToList();
        }

        return (new RoleDto
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
        }, false);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var role = await _roleRepository.GetByIdTrackedAsync(id);
        if (role == null)
        {
            return false;
        }

        var permissions = await _rolePermissionRepository.GetByRoleIdAsync(id);
        if (permissions.Count > 0)
        {
            await _rolePermissionRepository.RemoveRangeAsync(permissions);
        }

        await _roleRepository.DeleteAsync(role);
        await _roleRepository.SaveChangesAsync();

        return true;
    }

    public async Task<ResponseDTO<PagedResultDto<RolePainelDto>>> GetRolesPainelAsync(Guid tenantId, int page, int limit, string? search)
    {
        page = page <= 0 ? 1 : page;
        limit = limit <= 0 ? 10 : limit;

        var (roles, total) = await _roleRepository.GetPagedForTenantAsync(tenantId, page, limit, search);
        var roleIds = roles.Select(r => r.Id).ToList();
        var permissions = await _rolePermissionRepository.GetActiveByRoleIdsAsync(roleIds);

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

        var totalPages = (int)Math.Ceiling(total / (double)limit);

        return StaticResponseBuilder<PagedResultDto<RolePainelDto>>.BuildOk(new PagedResultDto<RolePainelDto>
        {
            Items = items,
            Page = page,
            Limit = limit,
            Total = total,
            TotalPages = totalPages
        });
    }

    public async Task<ResponseDTO<RolePainelDto>> GetRolePainelByIdAsync(Guid tenantId, Guid id)
    {
        var role = await _roleRepository.GetByIdForTenantAsync(tenantId, id);
        if (role == null)
        {
            return StaticResponseBuilder<RolePainelDto>.BuildNotFound(new RolePainelDto());
        }

        var permissions = await _rolePermissionRepository.GetActiveByRoleIdAsync(id);

        return StaticResponseBuilder<RolePainelDto>.BuildOk(new RolePainelDto
        {
            Id = role.Id,
            Name = role.Name,
            Description = role.Description,
            Code = role.Code,
            IsDefault = role.IsSystem,
            IsActive = role.IsActive,
            Permissions = permissions.Select(rp => new SimplifiedPermissionDto { Module = rp.ModuleKey, Actions = rp.Actions }).ToList()
        });
    }

    public async Task<ResponseDTO<List<AvailableModuleDto>>> GetAvailableModulesPainelAsync(Guid tenantId)
    {
        _ = tenantId;

        var actions = DefaultActions();
        var modules = await _accessControlModuleRepository.GetActiveModulesWithKeyAsync();

        var result = modules.Select(m => new AvailableModuleDto
        {
            Key = m.Key!,
            Name = m.Name,
            Description = m.Description ?? string.Empty,
            AvailableActions = actions
        }).ToList();

        return StaticResponseBuilder<List<AvailableModuleDto>>.BuildOk(result);
    }

    public async Task<ResponseDTO<RolePainelDto>> CreateRolePainelAsync(Guid tenantId, CreateRoleRequestDto request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return StaticResponseBuilder<RolePainelDto>.BuildError("Nome inválido.");
        }

        if (!string.IsNullOrWhiteSpace(request.Code))
        {
            var code = request.Code.Trim();
            var exists = await _roleRepository.CodeExistsAsync(code, tenantId: tenantId, includeNullTenant: true);
            if (exists)
            {
                return StaticResponseBuilder<RolePainelDto>.BuildError("Código de role já existe.");
            }
        }

        var now = DateTime.UtcNow;
        var role = new RoleEntity
        {
            Id = Guid.NewGuid(),
            Name = request.Name.Trim(),
            Description = request.Description ?? string.Empty,
            Code = request.Code,
            TenantId = tenantId,
            ApplicationId = null,
            IsActive = request.IsActive ?? true,
            IsSystem = false,
            CreatedAt = now,
            UpdatedAt = null
        };

        await _roleRepository.AddAsync(role);

        var normalizedPermissions = NormalizePermissions(request.Permissions);
        if (normalizedPermissions.Count > 0)
        {
            var entities = normalizedPermissions.Select(p => new RolePermissionEntity
            {
                Id = Guid.NewGuid(),
                RoleId = role.Id,
                ModuleKey = p.Module,
                Actions = p.Actions,
                IsActive = true,
                CreatedAt = now,
                UpdatedAt = null
            });

            await _rolePermissionRepository.AddRangeAsync(entities);
        }

        await _roleRepository.SaveChangesAsync();

        return StaticResponseBuilder<RolePainelDto>.BuildOk(new RolePainelDto
        {
            Id = role.Id,
            Name = role.Name,
            Description = role.Description,
            Code = role.Code,
            IsDefault = false,
            IsActive = role.IsActive,
            Permissions = normalizedPermissions
        });
    }

    public async Task<ResponseDTO<RolePainelDto>> UpdateRolePainelAsync(Guid tenantId, Guid id, UpdateRoleRequestDto request)
    {
        var role = await _roleRepository.GetByIdForTenantTrackedAsync(tenantId, id);
        if (role == null)
        {
            return StaticResponseBuilder<RolePainelDto>.BuildNotFound(new RolePainelDto());
        }

        if (role.IsSystem)
        {
            return StaticResponseBuilder<RolePainelDto>.BuildError("Role padrão não pode ser alterada.");
        }

        if (request.Name != null)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
            {
                return StaticResponseBuilder<RolePainelDto>.BuildError("Nome inválido.");
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
                var exists = await _roleRepository.CodeExistsAsync(code, excludeRoleId: id, tenantId: tenantId, includeNullTenant: true);
                if (exists)
                {
                    return StaticResponseBuilder<RolePainelDto>.BuildError("Código de role já existe.");
                }
            }

            role.Code = request.Code;
        }

        if (request.IsActive.HasValue)
        {
            role.IsActive = request.IsActive.Value;
        }

        role.UpdatedAt = DateTime.UtcNow;

        List<SimplifiedPermissionDto> resultingPermissions;
        if (request.Permissions != null)
        {
            resultingPermissions = NormalizePermissions(request.Permissions);

            var existing = await _rolePermissionRepository.GetByRoleIdAsync(id);
            var requestedKeys = resultingPermissions.Select(p => p.Module).ToHashSet(StringComparer.OrdinalIgnoreCase);

            var toRemove = existing.Where(rp => !requestedKeys.Contains(rp.ModuleKey)).ToList();
            if (toRemove.Count > 0)
            {
                await _rolePermissionRepository.RemoveRangeAsync(toRemove);
            }

            foreach (var p in resultingPermissions)
            {
                var rp = existing.FirstOrDefault(x => string.Equals(x.ModuleKey, p.Module, StringComparison.OrdinalIgnoreCase));
                if (rp == null)
                {
                    await _rolePermissionRepository.AddRangeAsync([new RolePermissionEntity
                    {
                        Id = Guid.NewGuid(),
                        RoleId = id,
                        ModuleKey = p.Module,
                        Actions = p.Actions,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = null
                    }]);
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
            var permissions = await _rolePermissionRepository.GetActiveByRoleIdAsync(id);
            resultingPermissions = permissions
                .Select(rp => new SimplifiedPermissionDto { Module = rp.ModuleKey, Actions = rp.Actions })
                .ToList();
        }

        await _roleRepository.SaveChangesAsync();

        return StaticResponseBuilder<RolePainelDto>.BuildOk(new RolePainelDto
        {
            Id = role.Id,
            Name = role.Name,
            Description = role.Description,
            Code = role.Code,
            IsDefault = role.IsSystem,
            IsActive = role.IsActive,
            Permissions = resultingPermissions
        });
    }

    public async Task<ResponseDTO<bool>> DeleteRolePainelAsync(Guid tenantId, Guid id)
    {
        var role = await _roleRepository.GetByIdForTenantTrackedAsync(tenantId, id);
        if (role == null)
        {
            return StaticResponseBuilder<bool>.BuildOk(true);
        }

        if (role.IsSystem)
        {
            return StaticResponseBuilder<bool>.BuildError("Role padrão não pode ser removida.");
        }

        role.IsActive = false;
        role.UpdatedAt = DateTime.UtcNow;

        await _roleRepository.SaveChangesAsync();
        return StaticResponseBuilder<bool>.BuildOk(true);
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

