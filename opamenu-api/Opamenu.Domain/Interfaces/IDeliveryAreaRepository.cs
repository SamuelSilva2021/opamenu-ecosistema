using Opamenu.Infrastructure.Shared.Entities.Opamenu;

namespace Opamenu.Domain.Interfaces;

public interface IDeliveryAreaRepository : IRepository<DeliveryAreaEntity>
{
    Task<DeliveryAreaEntity?> GetByLocationAsync(Guid tenantId, string city, string? neighborhood);
    Task<IEnumerable<DeliveryAreaEntity>> GetAllByTenantAsync(Guid tenantId);
}
