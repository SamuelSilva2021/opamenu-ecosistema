
using Opamenu.Infrastructure.Shared.Entities;
using Opamenu.Infrastructure.Shared.Entities.Opamenu;
using Opamenu.Infrastructure.Shared.Enums.Opamenu;

namespace Opamenu.Domain.Interfaces;

public interface IOrderRepository : IRepository<OrderEntity>
{
    Task<IEnumerable<OrderEntity>> GetActiveOrdersWithProductAsync(Guid productId);

    Task<IEnumerable<OrderEntity>> GetByCustomerIdAndTenantIdAsync(Guid tenantId, Guid customerId);

    Task<IEnumerable<OrderEntity>> GetOrdersByStatusAsync(EOrderStatus status);
    
    Task<IEnumerable<OrderEntity>> GetOrdersByDateRangeAsync(Guid tenantId, DateTime startDate, DateTime endDate);

    Task<IEnumerable<OrderEntity>> GetDeliveryBoardOrdersAsync(Guid tenantId, DateTime todayStart, DateTime todayEnd);

    Task<IEnumerable<OrderEntity>> GetActiveOrdersWithProductAditionalGroupAsync(Guid productAditionalGroupId);

    Task<IEnumerable<OrderEntity>> GetPagedByTenantIdWithDetailsAsync(Guid tenantId, int pageNumber, int pageSize);
    Task<int?> GetLastOrderNumberAsync(Guid tenantId, DateTime date);
    Task<OrderEntity?> GetByIdForLoyaltyAsync(Guid id, Guid tenantId);
    Task<IEnumerable<OrderEntity>> GetByTabIdAsync(Guid tenantId, Guid tabId);
}

