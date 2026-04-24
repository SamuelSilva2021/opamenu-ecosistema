using OpaMenu.Domain.DTOs.AccessControl;

namespace OpaMenu.Application.Services.Interfaces.AccessControl;

public interface IModuleService
{
    Task<PagedResultDto<ModuleDto>> GetModulesAsync(int page, int limit, string? search, bool? isActive, string? sortBy, string? sortOrder);
    Task<ModuleDto?> GetModuleByIdAsync(Guid id);
    Task<ModuleDto?> CreateAsync(CreateModuleRequestDto request);
    Task<(ModuleDto? Module, bool NotFound)> UpdateAsync(Guid id, UpdateModuleRequestDto request);
    Task<(ModuleDto? Module, bool NotFound)> ToggleStatusAsync(Guid id);
    Task DeleteAsync(Guid id);
}

