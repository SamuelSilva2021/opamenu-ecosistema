using OpaMenu.Infrastructure.Shared.Entities.Opamenu;

namespace OpaMenu.Domain.Interfaces;

public interface IDeliveryAreaRepository : IRepository<DeliveryAreaEntity>
{
    Task<DeliveryAreaEntity?> GetByLocationAsync(Guid tenantId, string city, string? neighborhood);
    Task<IEnumerable<DeliveryAreaEntity>> GetAllByTenantAsync(Guid tenantId);
}
