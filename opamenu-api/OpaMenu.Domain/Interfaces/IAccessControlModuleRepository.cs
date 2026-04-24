using OpaMenu.Infrastructure.Shared.Entities.AccessControl;

namespace OpaMenu.Domain.Interfaces;

public interface IAccessControlModuleRepository
{
    Task<IReadOnlyList<ModuleEntity>> GetActiveModulesWithKeyAsync();
}
