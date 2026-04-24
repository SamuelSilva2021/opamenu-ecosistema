using OpaMenu.Infrastructure.Shared.Entities.AccessControl;

namespace OpaMenu.Domain.Interfaces;

public interface IModuleRepository
{
    Task<string?> GetKeyByIdAsync(Guid id);
    Task<ModuleEntity?> GetByIdAsync(Guid id);
    Task<ModuleEntity?> GetByIdWithApplicationAsync(Guid id);
    Task<ModuleEntity?> GetByIdTrackedAsync(Guid id);
    Task<IReadOnlyList<ModuleEntity>> GetByIdsWithApplicationAsync(IReadOnlyCollection<Guid> ids);
    Task<(IReadOnlyList<ModuleEntity> Items, int Total)> GetPagedAsync(int page, int limit, string? search, bool? isActive, string? sortBy, string? sortOrder);
    Task<bool> KeyExistsAsync(string key, Guid? excludeId = null);
    Task AddAsync(ModuleEntity entity);
    Task DeleteAsync(ModuleEntity entity);
    Task SaveChangesAsync();
    Task<(Guid Id, string Name)?> GetIdAndNameByKeyAsync(string key);
    Task<IReadOnlyList<(Guid Id, string Key, string Name)>> GetIdAndNameByKeysAsync(IReadOnlyCollection<string> keys);
}
