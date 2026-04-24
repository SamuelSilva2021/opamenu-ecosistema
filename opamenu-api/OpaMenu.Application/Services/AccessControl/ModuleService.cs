using OpaMenu.Application.Services.Interfaces.AccessControl;
using OpaMenu.Domain.DTOs.AccessControl;
using OpaMenu.Domain.Interfaces;
using OpaMenu.Infrastructure.Shared.Entities.AccessControl;

namespace OpaMenu.Application.Services.AccessControl;

public sealed class ModuleService(IModuleRepository moduleRepository) : IModuleService
{
    private readonly IModuleRepository _moduleRepository = moduleRepository;

    public async Task<PagedResultDto<ModuleDto>> GetModulesAsync(int page, int limit, string? search, bool? isActive, string? sortBy, string? sortOrder)
    {
        page = page <= 0 ? 1 : page;
        limit = limit <= 0 ? 10 : limit;

        var (items, total) = await _moduleRepository.GetPagedAsync(page, limit, search, isActive, sortBy, sortOrder);
        var totalPages = (int)Math.Ceiling(total / (double)limit);

        return new PagedResultDto<ModuleDto>
        {
            Items = items.Select(Map).ToList(),
            Page = page,
            Limit = limit,
            Total = total,
            TotalPages = totalPages
        };
    }

    public async Task<ModuleDto?> GetModuleByIdAsync(Guid id)
    {
        var module = await _moduleRepository.GetByIdWithApplicationAsync(id);
        return module == null ? null : Map(module);
    }

    public async Task<ModuleDto?> CreateAsync(CreateModuleRequestDto request)
    {
        if (string.IsNullOrWhiteSpace(request.Name) || string.IsNullOrWhiteSpace(request.Key))
        {
            return null;
        }

        var key = request.Key.Trim();
        var existsKey = await _moduleRepository.KeyExistsAsync(key);
        if (existsKey)
        {
            return null;
        }

        var now = DateTime.UtcNow;
        var entity = new ModuleEntity
        {
            Id = Guid.NewGuid(),
            Name = request.Name.Trim(),
            Description = request.Description ?? string.Empty,
            Url = request.Url ?? string.Empty,
            Key = key,
            Code = request.Code,
            ApplicationId = request.ApplicationId,
            IsActive = request.IsActive,
            CreatedAt = now,
            UpdatedAt = null
        };

        await _moduleRepository.AddAsync(entity);
        await _moduleRepository.SaveChangesAsync();

        var module = await _moduleRepository.GetByIdWithApplicationAsync(entity.Id);
        return module == null ? null : Map(module);
    }

    public async Task<(ModuleDto? Module, bool NotFound)> UpdateAsync(Guid id, UpdateModuleRequestDto request)
    {
        var entity = await _moduleRepository.GetByIdTrackedAsync(id);
        if (entity == null)
        {
            return (null, true);
        }

        if (string.IsNullOrWhiteSpace(request.Name) || string.IsNullOrWhiteSpace(request.Key))
        {
            return (null, false);
        }

        var key = request.Key.Trim();
        var existsKey = await _moduleRepository.KeyExistsAsync(key, excludeId: id);
        if (existsKey)
        {
            return (null, false);
        }

        entity.Name = request.Name.Trim();
        entity.Description = request.Description ?? string.Empty;
        entity.Url = request.Url ?? string.Empty;
        entity.Key = key;
        entity.Code = request.Code;
        entity.ApplicationId = request.ApplicationId;
        entity.IsActive = request.IsActive;
        entity.UpdatedAt = DateTime.UtcNow;

        await _moduleRepository.SaveChangesAsync();

        var module = await _moduleRepository.GetByIdWithApplicationAsync(id);
        return (module == null ? null : Map(module), false);
    }

    public async Task<(ModuleDto? Module, bool NotFound)> ToggleStatusAsync(Guid id)
    {
        var entity = await _moduleRepository.GetByIdTrackedAsync(id);
        if (entity == null)
        {
            return (null, true);
        }

        entity.IsActive = !entity.IsActive;
        entity.UpdatedAt = DateTime.UtcNow;
        await _moduleRepository.SaveChangesAsync();

        var module = await _moduleRepository.GetByIdWithApplicationAsync(id);
        return (module == null ? null : Map(module), false);
    }

    public async Task DeleteAsync(Guid id)
    {
        var entity = await _moduleRepository.GetByIdTrackedAsync(id);
        if (entity == null)
        {
            return;
        }

        await _moduleRepository.DeleteAsync(entity);
        await _moduleRepository.SaveChangesAsync();
    }

    private static ModuleDto Map(ModuleEntity m)
    {
        return new ModuleDto
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
        };
    }
}

