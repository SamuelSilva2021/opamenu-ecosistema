using OpaMenu.Infrastructure.Shared.Entities;

namespace OpaMenu.Domain.Interfaces;

public interface ITenantPaymentMethodRepository : IRepository<TenantPaymentMethodEntity>
{
    Task<IEnumerable<TenantPaymentMethodEntity>> GetAllByTenantWithPaymentMethodAsync(Guid tenantId);
    Task<TenantPaymentMethodEntity?> GetByIdWithPaymentMethodAsync(int id, Guid tenantId);
}

