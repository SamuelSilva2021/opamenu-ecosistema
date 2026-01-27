using Microsoft.EntityFrameworkCore;
using OpaMenu.Infrastructure.Shared.Entities;
using OpaMenu.Domain.Interfaces;
using OpaMenu.Infrastructure.Shared.Data.Context;
using OpaMenu.Infrastructure.Shared.Data.Context.Opamenu;
using OpaMenu.Infrastructure.Repositories;
using OpaMenu.Infrastructure.Shared.Enums.Opamenu;
using OpaMenu.Infrastructure.Shared.Entities.Opamenu;

namespace OpaMenu.Infrastructure.Repositories;

public class OrderRepository(OpamenuDbContext context) : OpamenuRepository<OrderEntity>(context), IOrderRepository
{

    /// <summary>
    /// ObtÃ©m pedidos ativos que contÃªm um produto especÃ­fico
    /// </summary>
    /// <param name="productId">ID do produto</param>
    /// <returns>ColeÃ§Ã£o de pedidos ativos com o produto</returns>
    public async Task<IEnumerable<OrderEntity>> GetActiveOrdersWithProductAsync(Guid productId)
    {
        return await _dbSet
            .Where(o =>o.Status != EOrderStatus.Cancelled && 
                       o.Status != EOrderStatus.Delivered &&
                       o.Items.Any(i => i.ProductId == productId))
            .Include(o => o.Items)
            .ThenInclude(i => i.Product)
            .Include(o => o.StatusHistory)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<OrderEntity>> GetByCustomerIdAndTenantIdAsync(Guid tenantId, Guid customerId)
    {
        return await _dbSet
            .Where(o => o.TenantId == tenantId &&
                        o.CustomerId == customerId)
            .Include(o => o.Items)
            .ThenInclude(i => i.Product)
            .Include(o => o.StatusHistory)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();
    }

    /// <summary>
    /// ObtÃ©m pedidos por status
    /// </summary>
    /// <param name="status">Status do pedido</param>
    /// <returns>ColeÃ§Ã£o de pedidos com o status especificado</returns>
    public async Task<IEnumerable<OrderEntity>> GetOrdersByStatusAsync(EOrderStatus status)
    {
        return await _dbSet
            .Where(o => o.Status == status)
            .Include(o => o.Items)
            .ThenInclude(i => i.Product)
            .Include(o => o.StatusHistory)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();
    }
    
    /// <summary>
    /// ObtÃ©m pedidos de um perÃ­odo especÃ­fico
    /// </summary>
    /// <param name="startDate">Data inicial</param>
    /// <param name="endDate">Data final</param>
    /// <returns>ColeÃ§Ã£o de pedidos do perÃ­odo</returns>
    public async Task<IEnumerable<OrderEntity>> GetOrdersByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        return await _dbSet
            .Where(o => o.CreatedAt >= startDate && 
                       o.CreatedAt <= endDate)
            .Include(o => o.Items)
            .ThenInclude(i => i.Product)
            .Include(o => o.StatusHistory)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();
    }
    
    /// <summary>
    /// ObtÃ©m pedidos ativos que contÃªm um ProductAddonGroup especÃ­fico
    /// </summary>
    /// <param name="productAddonGroupId">ID do ProductAddonGroup</param>
    /// <returns>ColeÃ§Ã£o de pedidos ativos com o ProductAddonGroup</returns>
    public async Task<IEnumerable<OrderEntity>> GetActiveOrdersWithProductAddonGroupAsync(Guid productAddonGroupId)
    {
        return await _dbSet
            .Where(o => o.Status != EOrderStatus.Cancelled && 
                       o.Status != EOrderStatus.Delivered &&
                       o.Items.Any(i => i.Addons.Any(a => a.Addon.AddonGroupId == productAddonGroupId)))
            .Include(o => o.Items)
                .ThenInclude(i => i.Product)
            .Include(o => o.Items)
                .ThenInclude(i => i.Addons)
                    .ThenInclude(a => a.Addon)
            .Include(o => o.StatusHistory)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<OrderEntity>> GetPagedByTenantIdWithDetailsAsync(Guid tenantId, int pageNumber, int pageSize)
    {
        return await _dbSet
            .Where(o => o.TenantId == tenantId)
            .Include(o => o.Items)
                .ThenInclude(i => i.Product)
            .Include(o => o.StatusHistory)
            .Include(o => o.Rejection)
            .OrderByDescending(o => o.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }
}
