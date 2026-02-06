
using OpaMenu.Infrastructure.Shared.Entities;
using OpaMenu.Infrastructure.Shared.Entities.Opamenu;
using OpaMenu.Infrastructure.Shared.Enums.Opamenu;

namespace OpaMenu.Domain.Interfaces;

public interface IOrderRepository : IRepository<OrderEntity>
{
    Task<IEnumerable<OrderEntity>> GetActiveOrdersWithProductAsync(Guid productId);

    Task<IEnumerable<OrderEntity>> GetByCustomerIdAndTenantIdAsync(Guid tenantId, Guid customerId);

    Task<IEnumerable<OrderEntity>> GetOrdersByStatusAsync(EOrderStatus status);
    
    Task<IEnumerable<OrderEntity>> GetOrdersByDateRangeAsync(Guid tenantId, DateTime startDate, DateTime endDate);

    Task<IEnumerable<OrderEntity>> GetActiveOrdersWithProductAddonGroupAsync(Guid productAddonGroupId);

    Task<IEnumerable<OrderEntity>> GetPagedByTenantIdWithDetailsAsync(Guid tenantId, int pageNumber, int pageSize);
    Task<int?> GetLastOrderNumberAsync(Guid tenantId, DateTime date);
}

