using Authenticator.API.Core.Application.Interfaces;
using OpaMenu.Infrastructure.Shared.Entities.AccessControl;

namespace Authenticator.API.Core.Application.Interfaces.AccessControl.RoleAccessGroups
{
    /// <summary>
    /// Interface do repositório de relações Role-AccessGroup
    /// </summary>
    public interface IRoleAccessGroupRepository : IBaseRepository<RoleAccessGroupEntity>
    {
        /// <summary>
        /// Busca relações por ID do role
        /// </summary>
        Task<IEnumerable<RoleAccessGroupEntity>> GetByRoleIdAsync(Guid roleId);

        /// <summary>
        /// Busca relações por ID do grupo de acesso
        /// </summary>
        Task<IEnumerable<RoleAccessGroupEntity>> GetByAccessGroupIdAsync(Guid accessGroupId);

        /// <summary>
        /// Busca uma relação específica entre role e grupo
        /// </summary>
        Task<RoleAccessGroupEntity?> GetByRoleAndGroupAsync(Guid roleId, Guid accessGroupId);

        /// <summary>
        /// Remove todas as relações de um role (soft delete)
        /// </summary>
        Task<bool> RemoveAllByRoleIdAsync(Guid roleId);

        /// <summary>
        /// Remove relações específicas (soft delete)
        /// </summary>
        Task<bool> RemoveByRoleAndGroupsAsync(Guid roleId, IEnumerable<Guid> groupIds);
    }
}
