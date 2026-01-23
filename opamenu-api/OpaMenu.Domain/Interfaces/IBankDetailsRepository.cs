using OpaMenu.Infrastructure.Shared.Entities.MultiTenant.Tenant;

namespace OpaMenu.Domain.Interfaces
{
    public interface IBankDetailsRepository
    {
        Task<BankDetailsEntity?> GetByIdAsync(Guid tenantId, Guid id);
        Task<IEnumerable<BankDetailsEntity>> GetByTenantIdAsync(Guid tenantId);
        Task<BankDetailsEntity?> GetByPixKeySelected(Guid tenantId, string pixKey);
        Task<BankDetailsEntity?> GetByPixKey(Guid tenantId, string pixKey);
        Task AddAsync(BankDetailsEntity entity);
        Task UpdateAsync(BankDetailsEntity entity);
        Task UpdateRangeAsync(IEnumerable<BankDetailsEntity> entities);
        Task DeleteAsync(BankDetailsEntity entity);
    }
}
