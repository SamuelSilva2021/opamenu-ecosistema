using OpaMenu.Infrastructure.Shared.Entities.MultiTenant.TenantModule;

namespace Authenticator.API.Core.Application.Interfaces.MultiTenant
{
    public interface ITenantModuleRepository : IBaseRepository<TenantModuleEntity>
    {
        Task<IEnumerable<TenantModuleEntity>> GetByTenantIdAsync(Guid tenantId);
        Task RemoveByTenantIdAsync(Guid tenantId);
        Task AddRangeAsync(IEnumerable<TenantModuleEntity> modules);
        Task<TenantModuleEntity?> GetByTenantAndModuleIdAsync(Guid tenantId, Guid moduleId);
    }
}
