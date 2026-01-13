using Authenticator.API.Core.Application.Interfaces;
using Authenticator.API.Core.Application.Interfaces.AccessControl.Roles;
using OpaMenu.Infrastructure.Shared.Entities.AccessControl;

namespace Authenticator.API.Infrastructure.Repositories.AccessControl.Roles
{
    /// <summary>
    /// Repositório para gerenciamento de Roles
    /// </summary>
    /// <param name="dbContextProvider"></param>
    public class RoleRepository(
        IDbContextProvider dbContextProvider
    ) : BaseRepository<RoleEntity>(dbContextProvider), IRoleRepository
    {
        /// <summary>
        /// Obtém roles por tenant
        /// </summary>
        public async Task<IEnumerable<RoleEntity>> GetAllByTenantAsync(Guid tenantId) =>
            await FindAsync(r => r.TenantId == tenantId);
    }
}
