using OpaMenu.Application.DTOs;
using OpaMenu.Commons.Api.DTOs;
using OpaMenu.Domain.DTOs;
using OpaMenu.Domain.DTOs.Order;
using OpaMenu.Infrastructure.Shared.Entities;
using OpaMenu.Web.Models.DTOs;

namespace OpaMenu.Application.Services.Interfaces
{
    /// <summary>
    /// Interface para serviÃ§os de pedidos seguindo princÃ­pios SOLID
    /// </summary>
    public interface IOrderService
    {
        Task<ResponseDTO<IEnumerable<OrderResponseDto>>> GetOrdersAsync();
        Task<PagedResponseDTO<OrderResponseDto>> GetOrdersPagedAsync(int pageNumber, int pageSize);
        Task<ResponseDTO<OrderResponseDto>> GetOrderByIdAsync(int id);
        Task<ResponseDTO<OrderResponseDto>> GetPublicOrderByIdAsync(string slug, int id);
        Task<ResponseDTO<IEnumerable<OrderResponseDto>>> GetPublicOrdersByCustomerIdAsync(string slug, Guid customerId);
        Task<ResponseDTO<OrderResponseDto>> CreateOrderAsync(CreateOrderRequestDto requestDto);
        Task<ResponseDTO<OrderResponseDto>> CreatePublicOrderAsync(CreatePublicOrderRequestDto requestDto, string slug);
        Task<ResponseDTO<OrderResponseDto>> UpdateOrderAsync(int id, UpdateOrderRequestDto requestDto);
        Task<ResponseDTO<bool>> DeleteOrderAsync(int id);
        Task<ResponseDTO<OrderResponseDto>> UpdateOrderStatusAsync(int id, UpdateOrderStatusRequestDto requestDto);
        Task<ResponseDTO<IEnumerable<OrderResponseDto>>> GetOrdersByStatusAsync(OrderStatus status);
        Task<ResponseDTO<IEnumerable<OrderResponseDto>>> GetOrdersByCustomerAsync(string customerPhone);
        Task<ResponseDTO<OrderResponseDto>> AcceptOrderAsync(int id, int estimatedPreparationMinutes, string? notes = null, string? userId = null);
        Task<ResponseDTO<OrderResponseDto>> RejectOrderAsync(int id, string reason, string? notes = null, string? rejectedBy = null);
        Task<ResponseDTO<OrderResponseDto>> CancelOrderAsync(int id, CancelOrderRequestDto requestDto);
        Task<ResponseDTO<OrderResponseDto>> UpdateOrderPaymentMethodAsync(int id, UpdateOrderPaymentRequestDto requestDto);
        Task<ResponseDTO<OrderResponseDto>> UpdateOrderDeliveryTypeAsync(int id, UpdateOrderDeliveryTypeRequestDto requestDto);

        /// <summary>
        /// ObtÃ©m o pedido ativo de uma mesa
        /// </summary>
        Task<ResponseDTO<OrderResponseDto?>> GetActiveOrderByTableIdAsync(int tableId);

        /// <summary>
        /// Adiciona itens a um pedido existente
        /// </summary>
        Task<ResponseDTO<OrderResponseDto>> AddItemsToOrderAsync(int orderId, List<CreateOrderItemRequestDto> items);

        /// <summary>
        /// Fecha a conta da mesa (solicita fechamento)
        /// </summary>
        Task<ResponseDTO<OrderResponseDto>> CloseTableAccountAsync(int tableId);
    }
}

