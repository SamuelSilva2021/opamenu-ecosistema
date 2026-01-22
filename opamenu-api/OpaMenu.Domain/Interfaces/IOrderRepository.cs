
using OpaMenu.Infrastructure.Shared.Entities;
using OpaMenu.Infrastructure.Shared.Enums.Opamenu;

namespace OpaMenu.Domain.Interfaces;

/// <summary>
/// Interface do repositÃ³rio de pedidos
/// </summary>
public interface IOrderRepository : IRepository<OrderEntity>
{
    /// <summary>
    /// ObtÃ©m pedidos ativos que contÃªm um produto especÃ­fico
    /// </summary>
    /// <param name="productId">ID do produto</param>
    /// <returns>ColeÃ§Ã£o de pedidos ativos com o produto</returns>
    Task<IEnumerable<OrderEntity>> GetActiveOrdersWithProductAsync(Guid productId);

    /// <summary>
    /// ObtÃ©m pedidos por ID do cliente e ID do locatÃ¡rio
    /// </summary>
    /// <param name="productId"></param>
    /// <returns></returns>
    Task<IEnumerable<OrderEntity>> GetByCustomerIdAndTenantIdAsync(Guid tenantId, Guid customerId);

    /// <summary>
    /// ObtÃ©m pedidos por status
    /// </summary>
    /// <param name="status">Status do pedido</param>
    /// <returns>ColeÃ§Ã£o de pedidos com o status especificado</returns>
    Task<IEnumerable<OrderEntity>> GetOrdersByStatusAsync(EOrderStatus status);
    
    /// <summary>
    /// ObtÃ©m pedidos de um perÃ­odo especÃ­fico
    /// </summary>
    /// <param name="startDate">Data inicial</param>
    /// <param name="endDate">Data final</param>
    /// <returns>ColeÃ§Ã£o de pedidos do perÃ­odo</returns>
    Task<IEnumerable<OrderEntity>> GetOrdersByDateRangeAsync(DateTime startDate, DateTime endDate);
    
    /// <summary>
    /// ObtÃ©m pedidos ativos que contÃªm um ProductAddonGroup especÃ­fico
    /// </summary>
    /// <param name="productAddonGroupId">ID do ProductAddonGroup</param>
    /// <returns>ColeÃ§Ã£o de pedidos ativos com o ProductAddonGroup</returns>
    Task<IEnumerable<OrderEntity>> GetActiveOrdersWithProductAddonGroupAsync(Guid productAddonGroupId);

    /// <summary>
    /// ObtÃ©m pedidos paginados por TenantId com todos os detalhes (Items, Product, StatusHistory, Rejection)
    /// </summary>
    /// <param name="tenantId">ID do Tenant</param>
    /// <param name="pageNumber">NÃºmero da pÃ¡gina</param>
    /// <param name="pageSize">Tamanho da pÃ¡gina</param>
    /// <returns>ColeÃ§Ã£o de pedidos paginados</returns>
    Task<IEnumerable<OrderEntity>> GetPagedByTenantIdWithDetailsAsync(Guid tenantId, int pageNumber, int pageSize);
}

