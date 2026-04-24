using OpaMenu.Infrastructure.Shared.Entities.AccessControl;

namespace OpaMenu.Domain.Interfaces;

public interface IModuleRepository
{
    Task<string?> GetKeyByIdAsync(Guid id);
    Task<ModuleEntity?> GetByIdAsync(Guid id);
    Task<(Guid Id, string Name)?> GetIdAndNameByKeyAsync(string key);
    Task<IReadOnlyList<(Guid Id, string Key, string Name)>> GetIdAndNameByKeysAsync(IReadOnlyCollection<string> keys);
}

