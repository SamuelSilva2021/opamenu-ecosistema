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
    /// Obtém pedidos por status (filtra apenas do dia atual se não for Pending)
    /// </summary>
    /// <param name="status">Status do pedido</param>
    /// <returns>Coleção de pedidos com o status especificado</returns>
    public async Task<IEnumerable<OrderEntity>> GetOrdersByStatusAsync(EOrderStatus status)
    {
        var query = _dbSet.Where(o => o.Status == status);

        // Se o status for finalizado (Delivered/Cancelled/Rejected), filtra pelo dia atual
        if (status == EOrderStatus.Delivered || status == EOrderStatus.Cancelled || status == EOrderStatus.Rejected)
        {
            var today = DateTime.UtcNow.Date;
            query = query.Where(o => o.CreatedAt >= today);
        }
        
        // Se for status de produção (Ready/Preparing), também foca no dia atual para evitar lixo antigo
        if (status == EOrderStatus.Ready || status == EOrderStatus.Preparing || status == EOrderStatus.OutForDelivery)
        {
            var today = DateTime.UtcNow.Date;
            query = query.Where(o => o.CreatedAt >= today);
        }

        return await query
            .Include(o => o.Items)
            .ThenInclude(i => i.Product)
            .Include(o => o.StatusHistory)
            .Include(o => o.Driver) // Inclui dados do Colaborador
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();
    }
    
    /// <summary>
    /// Obtém pedidos por período de criação
    /// </summary>
    /// <param name="startDate">Data inicial</param>
    /// <param name="endDate">Data final</param>
    /// <returns>Coleção de pedidos do período</returns>    
    public async Task<IEnumerable<OrderEntity>> GetOrdersByDateRangeAsync(Guid tenantId, DateTime startDate, DateTime endDate)
    {
        return await _dbSet
            .Where(o => o.TenantId == tenantId &&
                       o.CreatedAt >= startDate && 
                       o.CreatedAt <= endDate)
            .Include(o => o.Items)
            .ThenInclude(i => i.Product)
            .Include(o => o.StatusHistory)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();
    }
    
    /// <summary>
    /// Obtém pedidos ativos que contêm um ProductAditionalGroup específico
    /// </summary>
    /// <param name="productAditionalGroupId">ID do ProductAditionalGroup</param>
    /// <returns>Coleção de pedidos ativos com o ProductAditionalGroup</returns>    
    public async Task<IEnumerable<OrderEntity>> GetActiveOrdersWithProductAditionalGroupAsync(Guid productAditionalGroupId)
    {
        return await _dbSet
            .Where(o => o.Status != EOrderStatus.Cancelled && 
                       o.Status != EOrderStatus.Delivered &&
                       o.Items.Any(i => i.Aditionals.Any(a => a.Aditional.AditionalGroupId == productAditionalGroupId)))
            .Include(o => o.Items)
                .ThenInclude(i => i.Product)
            .Include(o => o.Items)
                .ThenInclude(i => i.Aditionals)
                    .ThenInclude(a => a.Aditional)
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

    public static async Task<int?> GetLastOrderNumberAsync(Guid tenantId, DateTime date)
    {
        return await _dbSet
            .Where(o => o.TenantId == tenantId && 
                        o.CreatedAt.Date == date.Date)
            .MaxAsync(o => (int?)o.OrderNumber);
    }
}
