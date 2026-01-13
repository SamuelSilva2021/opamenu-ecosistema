using Authenticator.API.Core.Application.Interfaces;
using Authenticator.API.Core.Application.Interfaces.AccessControl.AccessGroup;
using OpaMenu.Infrastructure.Shared.Entities.AccessControl;

namespace Authenticator.API.Infrastructure.Repositories.AccessControl.AccessGroup
{
    public class AccessGroupRepository(
        IDbContextProvider dbContextProvider
        ) : BaseRepository<AccessGroupEntity>(dbContextProvider), IAccessGroupRepository
    {
        public async Task<IEnumerable<AccessGroupEntity>> GetAllAsyncByTenantId(Guid tenantId) =>
            await FindAsync(ag => ag.TenantId == tenantId);
    }
}

