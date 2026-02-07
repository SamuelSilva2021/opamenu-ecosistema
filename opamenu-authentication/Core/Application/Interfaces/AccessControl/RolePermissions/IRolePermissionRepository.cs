using Authenticator.API.Core.Application.Interfaces;
using OpaMenu.Infrastructure.Shared.Entities.AccessControl;

namespace Authenticator.API.Core.Application.Interfaces.AccessControl.RolePermissions
{
    /// <summary>
    /// Interface do repositório de relações Role-Permission
    /// </summary>
    public interface IRolePermissionRepository : IBaseRepository<RolePermissionEntity>
    {
        /// <summary>
        /// Busca relações por ID do role
        /// </summary>
        Task<IEnumerable<RolePermissionEntity>> GetAllRolePermissionsByRoleIdAsync(Guid roleId);

        /// <summary>
        /// Busca uma relação específica entre role e módulo
        /// </summary>
        Task<RolePermissionEntity?> GetByRoleAndModuleAsync(Guid roleId, string moduleKey);

        /// <summary>
        /// Remove todas as relações de um role (soft delete)
        /// </summary>
        Task<bool> RemoveAllByRoleIdAsync(Guid roleId);

        /// <summary>
        /// Remove relações específicas por módulo (soft delete)
        /// </summary>
        Task<bool> RemoveByRoleAndModulesAsync(Guid roleId, IEnumerable<string> moduleKeys);
    }
}
