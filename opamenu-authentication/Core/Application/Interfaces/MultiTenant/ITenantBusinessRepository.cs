using OpaMenu.Infrastructure.Shared.Entities.MultiTenant.Tenant;

namespace Authenticator.API.Core.Application.Interfaces.MultiTenant
{
    public interface ITenantBusinessRepository : IBaseRepository<TenantBusinessEntity>
    {
        Task<TenantBusinessEntity?> GetByTenantIdAsync(Guid tenantId);
        Task<TenantBusinessEntity> DeleteAsync(Guid tenantId);
    }
}

