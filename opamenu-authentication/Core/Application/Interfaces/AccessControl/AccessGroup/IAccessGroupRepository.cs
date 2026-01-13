using OpaMenu.Infrastructure.Shared.Entities.AccessControl;

namespace Authenticator.API.Core.Application.Interfaces.AccessControl.AccessGroup
{
    public interface IAccessGroupRepository : IBaseRepository<AccessGroupEntity>
    {
        Task<IEnumerable<AccessGroupEntity>> GetAllAsyncByTenantId(Guid tenantId);
    }
}

