using Authenticator.API.Core.Application.Interfaces;
using OpaMenu.Infrastructure.Shared.Entities.AccessControl;

namespace Authenticator.API.Core.Application.Interfaces.AccessControl.Roles
{
    /// <summary>
    /// Interface para o repositório de Roles
    /// </summary>
    public interface IRoleRepository : IBaseRepository<RoleEntity>
    {
        /// <summary>
        /// Obtém roles por tenant
        /// </summary>
        /// <param name="tenantId"></param>
        /// <returns></returns>
        Task<IEnumerable<RoleEntity>> GetAllByTenantAsync(Guid tenantId);
    }
}
