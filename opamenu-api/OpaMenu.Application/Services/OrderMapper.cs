using OpaMenu.Application.Services.Interfaces;
using OpaMenu.Domain.DTOs;
using OpaMenu.Infrastructure.Shared.Entities;

namespace OpaMenu.Application.Services;

/// <summary>
/// ImplementaÃ§Ã£o do mapeador para entidades Order e DTOs
/// </summary>
public class OrderMapper : IOrderMapper
{
    /// <summary>
    /// Mapeia uma entidade Order para OrderResponseDto
    /// </summary>
    /// <param name="order">Entidade Order</param>
    /// <returns>OrderResponseDto mapeado</returns>
    public OrderResponseDto MapToDto(OrderEntity order)
    {
        if (order == null)
            throw new ArgumentNullException(nameof(order));

        return new OrderResponseDto
        {
            Id = order.Id,
            CustomerName = order.CustomerName,
            CustomerPhone = order.CustomerPhone,
            Status = order.Status,
            Total = order.Total,
            CreatedAt = order.CreatedAt,
            UpdatedAt = order.UpdatedAt,
            Notes = order.Notes,
            QueuePosition = order.QueuePosition,
            EstimatedPreparationMinutes = order.EstimatedPreparationMinutes,
            EstimatedDeliveryTime = order.EstimatedDeliveryTime,
            // Propriedades removidas pois nÃ£o existem na entidade Order
            Items = order.Items?.Select(MapToOrderItemDto).ToList() ?? new List<OrderItemResponseDto>()
        };
    }

    /// <summary>
    /// Mapeia uma coleÃ§Ã£o de entidades Order para OrderResponseDto
    /// </summary>
    /// <param name="orders">ColeÃ§Ã£o de entidades Order</param>
    /// <returns>ColeÃ§Ã£o de OrderResponseDto mapeados</returns>
    public IEnumerable<OrderResponseDto> MapToDtos(IEnumerable<OrderEntity> orders)
    {
        if (orders == null)
            return Enumerable.Empty<OrderResponseDto>();

        return orders.Select(MapToDto);
    }

    /// <summary>
    /// Mapeia um CreateOrderRequestDto para entidade Order
    /// </summary>
    /// <param name="request">Request de criaÃ§Ã£o</param>
    /// <returns>Entidade Order</returns>
    public OrderEntity MapToEntity(CreateOrderRequestDto request)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        var order = new OrderEntity
        {
            CustomerName = request.CustomerName,
            CustomerPhone = request.CustomerPhone,
            Status = OrderStatus.Pending,
            Total = 0, // SerÃ¡ calculado apÃ³s adicionar os itens
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Notes = request.Notes,
            Items = new List<OrderItemEntity>()
        };

        // Mapear itens
        if (request.Items?.Any() == true)
        {
            foreach (var itemRequest in request.Items)
            {
                var orderItem = MapToOrderItemEntity(itemRequest, 0); // OrderId serÃ¡ definido apÃ³s salvar
                order.Items.Add(orderItem);
            }
        }

        return order;
    }

    /// <summary>
    /// Atualiza uma entidade Order com dados do UpdateOrderRequestDto
    /// </summary>
    /// <param name="request">Request de atualizaÃ§Ã£o</param>
    /// <param name="order">Entidade Order a ser atualizada</param>
    public void MapToEntity(UpdateOrderRequestDto request, OrderEntity order)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));
        if (order == null)
            throw new ArgumentNullException(nameof(order));

        if (!string.IsNullOrWhiteSpace(request.CustomerName))
            order.CustomerName = request.CustomerName;

        if (!string.IsNullOrWhiteSpace(request.CustomerPhone))
            order.CustomerPhone = request.CustomerPhone;

        if (!string.IsNullOrWhiteSpace(request.Notes))
            order.Notes = request.Notes;

        order.UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Mapeia uma entidade OrderItem para OrderItemResponseDto
    /// </summary>
    /// <param name="orderItem">Entidade OrderItem</param>
    /// <returns>OrderItemResponseDto mapeado</returns>
    public OrderItemResponseDto MapToOrderItemDto(OrderItemEntity orderItem)
    {
        if (orderItem == null)
            throw new ArgumentNullException(nameof(orderItem));

        return new OrderItemResponseDto
        {
            Id = orderItem.Id,
            ProductId = orderItem.ProductId,
            ProductName = orderItem.Product?.Name ?? string.Empty,
            Quantity = orderItem.Quantity,
            UnitPrice = orderItem.UnitPrice,
            Subtotal = orderItem.Subtotal,
            Notes = orderItem.Notes,
            ImageUrl = orderItem.Product?.ImageUrl,
            Addons = orderItem.Addons?.Select(MapToOrderItemAddonDto).ToList() ?? new List<OrderItemAddonResponseDto>()
        };
    }

    /// <summary>
    /// Mapeia uma coleÃ§Ã£o de entidades OrderItem para OrderItemResponseDto
    /// </summary>
    /// <param name="orderItems">ColeÃ§Ã£o de entidades OrderItem</param>
    /// <returns>ColeÃ§Ã£o de OrderItemResponseDto mapeados</returns>
    public IEnumerable<OrderItemResponseDto> MapToOrderItemDtos(IEnumerable<OrderItemEntity> orderItems)
    {
        if (orderItems == null)
            return Enumerable.Empty<OrderItemResponseDto>();

        return orderItems.Select(MapToOrderItemDto);
    }

    /// <summary>
    /// Mapeia uma entidade OrderItemAddon para OrderItemAddonResponseDto
    /// </summary>
    /// <param name="orderItemAddon">Entidade OrderItemAddon</param>
    /// <returns>OrderItemAddonResponseDto mapeado</returns>
    public OrderItemAddonResponseDto MapToOrderItemAddonDto(OrderItemAddon orderItemAddon)
    {
        if (orderItemAddon == null)
            throw new ArgumentNullException(nameof(orderItemAddon));

        return new OrderItemAddonResponseDto
        {
            Id = orderItemAddon.Id,
            AddonId = orderItemAddon.AddonId,
            AddonName = orderItemAddon.AddonName,
            UnitPrice = orderItemAddon.UnitPrice,
            Quantity = orderItemAddon.Quantity,
            Subtotal = orderItemAddon.Subtotal
        };
    }

    /// <summary>
    /// Mapeia uma coleÃ§Ã£o de entidades OrderItemAddon para OrderItemAddonResponseDto
    /// </summary>
    /// <param name="orderItemAddons">ColeÃ§Ã£o de entidades OrderItemAddon</param>
    /// <returns>ColeÃ§Ã£o de OrderItemAddonResponseDto mapeados</returns>
    public IEnumerable<OrderItemAddonResponseDto> MapToOrderItemAddonDtos(IEnumerable<OrderItemAddon> orderItemAddons)
    {
        if (orderItemAddons == null)
            return Enumerable.Empty<OrderItemAddonResponseDto>();

        return orderItemAddons.Select(MapToOrderItemAddonDto);
    }

    /// <summary>
    /// Mapeia um CreateOrderItemRequestDto para entidade OrderItem
    /// </summary>
    /// <param name="request">Request de criaÃ§Ã£o do item</param>
    /// <param name="orderId">ID do pedido</param>
    /// <returns>Entidade OrderItem</returns>
    public OrderItemEntity MapToOrderItemEntity(CreateOrderItemRequestDto request, int orderId)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        var orderItem = new OrderItemEntity
        {
            OrderId = orderId,
            ProductId = request.ProductId,
            Quantity = request.Quantity,
            UnitPrice = 0, // SerÃ¡ definido no serviÃ§o baseado no produto
            Subtotal = 0, // SerÃ¡ calculado no serviÃ§o
            Notes = request.Notes,
            Addons = new List<OrderItemAddon>()
        };

        // Mapear addons
        if (request.Addons?.Any() == true)
        {
            foreach (var addonRequest in request.Addons)
            {
                var addon = new OrderItemAddon
                {
                    OrderItemId = 0, // SerÃ¡ definido apÃ³s salvar o OrderItem
                    AddonId = addonRequest.AddonId,
                    Quantity = addonRequest.Quantity,
                    UnitPrice = 0, // SerÃ¡ calculado no serviÃ§o baseado no Addon
                    Subtotal = 0 // SerÃ¡ calculado no serviÃ§o
                };
                orderItem.Addons.Add(addon);
            }
        }

        return orderItem;
    }

    /// <summary>
    /// Mapeia uma coleÃ§Ã£o de CreateOrderItemRequestDto para entidades OrderItem
    /// </summary>
    /// <param name="requests">ColeÃ§Ã£o de requests de criaÃ§Ã£o</param>
    /// <param name="orderId">ID do pedido</param>
    /// <returns>ColeÃ§Ã£o de entidades OrderItem</returns>
    public IEnumerable<OrderItemEntity> MapToOrderItemEntities(IEnumerable<CreateOrderItemRequestDto> requests, int orderId)
    {
        if (requests == null)
            return Enumerable.Empty<OrderItemEntity>();

        return requests.Select(request => MapToOrderItemEntity(request, orderId));
    }
}
