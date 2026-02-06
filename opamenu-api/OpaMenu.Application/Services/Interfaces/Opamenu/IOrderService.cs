using OpaMenu.Application.DTOs;
using OpaMenu.Commons.Api.DTOs;
using OpaMenu.Domain.DTOs;
using OpaMenu.Domain.DTOs.Order;
using OpaMenu.Infrastructure.Shared.Entities;
using OpaMenu.Infrastructure.Shared.Enums.Opamenu;
using OpaMenu.Web.Models.DTOs;

namespace OpaMenu.Application.Services.Interfaces.Opamenu
{
    /// <summary>
    /// Interface de serviço para gerenciamento de pedidos
    /// </summary>
    public interface IOrderService
    {
        Task<ResponseDTO<IEnumerable<OrderResponseDto>>> GetOrdersAsync(DateTime? date = null);
        Task<PagedResponseDTO<OrderResponseDto>> GetOrdersPagedAsync(int pageNumber, int pageSize);
        Task<ResponseDTO<OrderResponseDto>> GetOrderByIdAsync(Guid id);
        Task<ResponseDTO<OrderResponseDto>> GetPublicOrderByIdAsync(string slug, Guid id);
        Task<ResponseDTO<IEnumerable<OrderResponseDto>>> GetPublicOrdersByCustomerIdAsync(string slug, Guid customerId);
        /// <summary>
        /// Cria pedido para entrega
        /// </summary>
        /// <param name="requestDto"></param>
        /// <returns></returns>
        Task<ResponseDTO<OrderResponseDto>> CreateOrderDeliveryAsync(CreateOrderRequestDto requestDto);
        /// <summary>
        /// Cria pedido para retirada balcão
        /// </summary>
        /// <param name="requestDto"></param>
        /// <returns></returns>
        Task<ResponseDTO<OrderResponseDto>> CreateOrderPickupAsync(CreateOrderRequestDto requestDto);
        /// <summary>
        /// Cria pedido para consumo no local (mesa)
        /// </summary>
        /// <param name="requestDto"></param>
        /// <returns></returns>
        Task<ResponseDTO<OrderResponseDto>> CreateOrderDineInAsync(CreateOrderRequestDto requestDto);
        /// <summary>
        /// Cria um pedido via PDV
        /// </summary>
        /// <param name="requestDto"></param>
        /// <returns></returns>
        Task<ResponseDTO<OrderResponseDto>> CreateOrderPdv(CreateOrderRequestDto requestDto); 
        Task<ResponseDTO<OrderResponseDto>> CreatePublicOrderAsync(CreatePublicOrderRequestDto requestDto, string slug);
        Task<ResponseDTO<OrderResponseDto>> UpdateOrderAsync(Guid id, UpdateOrderRequestDto requestDto);
        Task<ResponseDTO<bool>> DeleteOrderAsync(Guid id);
        Task<ResponseDTO<OrderResponseDto>> UpdatEOrderStatusAsync(Guid id, UpdatEOrderStatusRequestDto requestDto);
        Task<ResponseDTO<IEnumerable<OrderResponseDto>>> GetOrdersByStatusAsync(EOrderStatus status);
        Task<ResponseDTO<IEnumerable<OrderResponseDto>>> GetOrdersByCustomerAsync(string customerPhone);
        Task<ResponseDTO<OrderResponseDto>> AcceptOrderAsync(Guid id, int estimatedPreparationMinutes, string? notes = null);
        Task<ResponseDTO<OrderResponseDto>> RejectOrderAsync(Guid id, string reason, string? notes = null, string? rejectedBy = null);
        Task<ResponseDTO<OrderResponseDto>> CancelOrderAsync(Guid id, CancelOrderRequestDto requestDto);
        Task<ResponseDTO<OrderResponseDto>> UpdateOrderPaymentMethodAsync(Guid id, UpdateOrderPaymentRequestDto requestDto);
        Task<ResponseDTO<OrderResponseDto>> UpdateOrderDeliveryTypeAsync(Guid id, UpdateOrderDeliveryTypeRequestDto requestDto);

        /// <summary>
        /// ObtÃ©m o pedido ativo de uma mesa
        /// </summary>
        Task<ResponseDTO<OrderResponseDto?>> GetActiveOrderByTableIdAsync(Guid tableId);

        /// <summary>
        /// Adiciona itens a um pedido existente
        /// </summary>
        Task<ResponseDTO<OrderResponseDto>> AddItemsToOrderAsync(Guid orderId, List<CreateOrderItemRequestDto> items);

        /// <summary>
        /// Fecha a conta da mesa (solicita fechamento)
        /// </summary>
        Task<ResponseDTO<OrderResponseDto>> CloseTableAccountAsync(Guid tableId);
    }
}

