using OpaMenu.Application.Services.Interfaces.AccessControl;
using OpaMenu.Domain.DTOs.AccessControl;
using OpaMenu.Domain.Interfaces;
using OpaMenu.Infrastructure.Shared.Entities.AccessControl;

namespace OpaMenu.Application.Services.AccessControl;

public sealed class PermissionService(
    IRolePermissionRepository rolePermissionRepository,
    IRoleRepository roleRepository,
    IModuleRepository moduleRepository) : IPermissionService
{
    private readonly IRolePermissionRepository _rolePermissionRepository = rolePermissionRepository;
    private readonly IRoleRepository _roleRepository = roleRepository;
    private readonly IModuleRepository _moduleRepository = moduleRepository;

    public async Task<PagedResultDto<PermissionDto>> GetPermissionsAsync(int page, int limit, string? search, Guid? moduleId, Guid? tenantId)
    {
        page = page <= 0 ? 1 : page;
        limit = limit <= 0 ? 10 : limit;

        string? moduleKey = null;
        if (moduleId.HasValue && moduleId.Value != Guid.Empty)
        {
            moduleKey = await _moduleRepository.GetKeyByIdAsync(moduleId.Value);
            if (string.IsNullOrWhiteSpace(moduleKey))
            {
                return new PagedResultDto<PermissionDto>
                {
                    Items = [],
                    Page = page,
                    Limit = limit,
                    Total = 0,
                    TotalPages = 0
                };
            }
        }

        var (items, total) = await _rolePermissionRepository.GetPagedAsync(page, limit, search, tenantId, moduleKey);
        var totalPages = (int)Math.Ceiling(total / (double)limit);

        var moduleKeys = items.Select(d => d.ModuleKey).Distinct(StringComparer.OrdinalIgnoreCase).ToList();
        var modules = await _moduleRepository.GetIdAndNameByKeysAsync(moduleKeys);
        var moduleByKey = modules.ToDictionary(m => m.Key, m => (m.Id, m.Name), StringComparer.OrdinalIgnoreCase);

        return new PagedResultDto<PermissionDto>
        {
            Items = items
                .OrderByDescending(rp => rp.CreatedAt)
                .Select(rp => ToDto(rp, moduleByKey.TryGetValue(rp.ModuleKey, out var m) ? m : null))
                .ToList(),
            Page = page,
            Limit = limit,
            Total = total,
            TotalPages = totalPages
        };
    }

    public async Task<PermissionDto?> GetPermissionByIdAsync(Guid id)
    {
        var rp = await _rolePermissionRepository.GetByIdWithRoleAsync(id);
        if (rp == null)
        {
            return null;
        }

        var module = await _moduleRepository.GetIdAndNameByKeyAsync(rp.ModuleKey);
        return ToDto(rp, module);
    }

    public async Task<PermissionDto?> CreateAsync(CreatePermissionRequestDto request)
    {
        if (!request.RoleId.HasValue || request.RoleId.Value == Guid.Empty)
        {
            return null;
        }

        if (request.ModuleId == Guid.Empty)
        {
            return null;
        }

        var role = await _roleRepository.GetByIdAsync(request.RoleId.Value);
        if (role == null)
        {
            return null;
        }

        var module = await _moduleRepository.GetByIdAsync(request.ModuleId);
        if (module == null || string.IsNullOrWhiteSpace(module.Key))
        {
            return null;
        }

        var actions = NormalizeOperationIds(request.OperationIds);
        if (actions.Count == 0)
        {
            actions = ["READ"];
        }

        var existing = await _rolePermissionRepository.GetByRoleIdAndModuleKeyTrackedAsync(request.RoleId.Value, module.Key);
        if (existing != null)
        {
            existing.Actions = actions;
            existing.IsActive = request.IsActive ?? true;
            existing.UpdatedAt = DateTime.UtcNow;
            await _rolePermissionRepository.SaveChangesAsync();
            return await GetPermissionByIdAsync(existing.Id);
        }

        var rpEntity = new RolePermissionEntity
        {
            Id = Guid.NewGuid(),
            RoleId = request.RoleId.Value,
            ModuleKey = module.Key,
            Actions = actions,
            IsActive = request.IsActive ?? true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = null
        };

        await _rolePermissionRepository.AddAsync(rpEntity);
        await _rolePermissionRepository.SaveChangesAsync();
        return await GetPermissionByIdAsync(rpEntity.Id);
    }

    public async Task<(PermissionDto? Permission, bool NotFound)> UpdateAsync(Guid id, UpdatePermissionRequestDto request)
    {
        var rp = await _rolePermissionRepository.GetByIdTrackedAsync(id);
        if (rp == null)
        {
            return (null, true);
        }

        if (request.ModuleId == Guid.Empty)
        {
            return (null, false);
        }

        var module = await _moduleRepository.GetByIdAsync(request.ModuleId);
        if (module == null || string.IsNullOrWhiteSpace(module.Key))
        {
            return (null, false);
        }

        if (request.RoleId.HasValue && request.RoleId.Value != Guid.Empty)
        {
            var roleExists = await _roleRepository.GetByIdAsync(request.RoleId.Value);
            if (roleExists == null)
            {
                return (null, false);
            }

            rp.RoleId = request.RoleId.Value;
        }

        rp.ModuleKey = module.Key;
        rp.Actions = NormalizeOperationIds(request.OperationIds);
        rp.IsActive = request.IsActive ?? rp.IsActive;
        rp.UpdatedAt = DateTime.UtcNow;

        await _rolePermissionRepository.SaveChangesAsync();
        return (await GetPermissionByIdAsync(id), false);
    }

    public async Task DeleteAsync(Guid id)
    {
        var rp = await _rolePermissionRepository.GetByIdTrackedAsync(id);
        if (rp == null)
        {
            return;
        }

        await _rolePermissionRepository.DeleteAsync(rp);
        await _rolePermissionRepository.SaveChangesAsync();
    }

    private static PermissionDto ToDto(RolePermissionEntity rp, (Guid Id, string Name)? module)
    {
        var roleCode = rp.Role?.Code ?? rp.Role?.Name ?? string.Empty;
        return new PermissionDto
        {
            Id = rp.Id,
            Name = $"{roleCode}:{rp.ModuleKey}",
            Description = null,
            Code = null,
            TenantId = rp.Role?.TenantId,
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
