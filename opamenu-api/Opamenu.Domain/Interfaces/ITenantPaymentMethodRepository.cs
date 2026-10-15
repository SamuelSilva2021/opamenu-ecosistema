using Opamenu.Infrastructure.Shared.Entities;
using Opamenu.Infrastructure.Shared.Entities.Opamenu;

namespace Opamenu.Domain.Interfaces;

public interface ITenantPaymentMethodRepository : IRepository<TenantPaymentMethodEntity>
{
    Task<IEnumerable<TenantPaymentMethodEntity>> GetAllByTenantWithPaymentMethodAsync(Guid tenantId);
    Task<TenantPaymentMethodEntity?> GetByIdWithPaymentMethodAsync(Guid id, Guid tenantId);
}

