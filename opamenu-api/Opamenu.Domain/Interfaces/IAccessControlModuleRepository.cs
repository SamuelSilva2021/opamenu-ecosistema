using Opamenu.Infrastructure.Shared.Entities.AccessControl;

namespace Opamenu.Domain.Interfaces;

public interface IAccessControlModuleRepository
{
    Task<IReadOnlyList<ModuleEntity>> GetActiveModulesWithKeyAsync();
}
