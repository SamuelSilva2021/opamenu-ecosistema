using OpaMenu.Infrastructure.Shared.Entities;
using OpaMenu.Domain.DTOs;

namespace OpaMenu.Application.Services.Interfaces;

/// <summary>
/// Interface para mapeamento entre entidades Order e DTOs
/// </summary>
public interface IOrderMapper
{
    /// <summary>
    /// Mapeia uma entidade Order para OrderResponseDto
    /// </summary>
    /// <param name="order">Entidade Order</param>
    /// <returns>OrderResponseDto mapeado</returns>
    OrderResponseDto MapToDto(OrderEntity order);
    
    /// <summary>
    /// Mapeia uma coleÃ§Ã£o de entidades Order para OrderResponseDto
    /// </summary>
    /// <param name="orders">ColeÃ§Ã£o de entidades Order</param>
    /// <returns>ColeÃ§Ã£o de OrderResponseDto mapeados</returns>
    IEnumerable<OrderResponseDto> MapToDtos(IEnumerable<OrderEntity> orders);
    
    /// <summary>
    /// Mapeia um CreateOrderRequestDto para entidade Order
    /// </summary>
    /// <param name="request">Request de criaÃ§Ã£o</param>
    /// <returns>Entidade Order</returns>
    OrderEntity MapToEntity(CreateOrderRequestDto request);
    
    /// <summary>
    /// Atualiza uma entidade Order com dados do UpdateOrderRequestDto
    /// </summary>
    /// <param name="request">Request de atualizaÃ§Ã£o</param>
    /// <param name="order">Entidade Order a ser atualizada</param>
    void MapToEntity(UpdateOrderRequestDto request, OrderEntity order);
    
    /// <summary>
    /// Mapeia uma entidade OrderItem para OrderItemResponseDto
    /// </summary>
    /// <param name="orderItem">Entidade OrderItem</param>
    /// <returns>OrderItemResponseDto mapeado</returns>
    OrderItemResponseDto MapToOrderItemDto(OrderItemEntity orderItem);
    
    /// <summary>
    /// Mapeia uma coleÃ§Ã£o de entidades OrderItem para OrderItemResponseDto
    /// </summary>
    /// <param name="orderItems">ColeÃ§Ã£o de entidades OrderItem</param>
    /// <returns>ColeÃ§Ã£o de OrderItemResponseDto mapeados</returns>
    IEnumerable<OrderItemResponseDto> MapToOrderItemDtos(IEnumerable<OrderItemEntity> orderItems);
    
    /// <summary>
    /// Mapeia uma entidade OrderItemAddon para OrderItemAddonResponseDto
    /// </summary>
    /// <param name="orderItemAddon">Entidade OrderItemAddon</param>
    /// <returns>OrderItemAddonResponseDto mapeado</returns>
    OrderItemAddonResponseDto MapToOrderItemAddonDto(OrderItemAddonEntity orderItemAddon);
    
    /// <summary>
    /// Mapeia uma coleÃ§Ã£o de entidades OrderItemAddon para OrderItemAddonResponseDto
    /// </summary>
    /// <param name="orderItemAddons">ColeÃ§Ã£o de entidades OrderItemAddon</param>
    /// <returns>ColeÃ§Ã£o de OrderItemAddonResponseDto mapeados</returns>
    IEnumerable<OrderItemAddonResponseDto> MapToOrderItemAddonDtos(IEnumerable<OrderItemAddonEntity> orderItemAddons);
    
    /// <summary>
    /// Mapeia um CreateOrderItemRequestDto para entidade OrderItem
    /// </summary>
    /// <param name="request">Request de criaÃ§Ã£o do item</param>
    /// <param name="orderId">ID do pedido</param>
    /// <returns>Entidade OrderItem</returns>
    OrderItemEntity MapToOrderItemEntity(CreateOrderItemRequestDto request, Guid orderId);
    
    /// <summary>
    /// Mapeia uma coleÃ§Ã£o de CreateOrderItemRequestDto para entidades OrderItem
    /// </summary>
    /// <param name="requests">ColeÃ§Ã£o de requests de criaÃ§Ã£o</param>
    /// <param name="orderId">ID do pedido</param>
    /// <returns>ColeÃ§Ã£o de entidades OrderItem</returns>
    IEnumerable<OrderItemEntity> MapToOrderItemEntities(IEnumerable<CreateOrderItemRequestDto> requests, Guid orderId);
}
