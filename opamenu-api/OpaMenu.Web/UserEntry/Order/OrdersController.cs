using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpaMenu.Commons.Api.DTOs;
using OpaMenu.Domain.DTOs;
using OpaMenu.Infrastructure.Shared.Entities;
using OpaMenu.Infrastructure.Shared.Enums.Opamenu;
using OpaMenu.Web.UserEntry;
using OpaMenu.Infrastructure.Anotations;
using OpaMenu.Infrastructure.Filters;
using OpaMenu.Application.Services.Interfaces.Opamenu;

namespace OpaMenu.Web.UserEntry.Order;

[ApiController]
[Route("api/orders")]
[Authorize]
[ServiceFilter(typeof(PermissionAuthorizationFilter))]
public class OrdersController(IOrderService orderService, ILogger<OrdersController> logger) : BaseController
{
    private readonly IOrderService _orderService = orderService;
    private readonly ILogger<OrdersController> _logger = logger;

    /// <summary>
    /// Obtém uma lista de pedidos, com suporte para paginação e filtragem por data
    /// </summary>
    [HttpGet]
    [MapPermission([ORDER, MODULE_PDV], OPERATION_SELECT)]
    public async Task<ActionResult<ApiResponse<IEnumerable<OrderResponseDto>>>> GetOrders([FromQuery] int? page = null, [FromQuery] int? pageSize = null, [FromQuery] DateTime? date = null)
    {
        if (page.HasValue && pageSize.HasValue)
        {
            var pagedResponse = await _orderService.GetOrdersPagedAsync(page.Value, pageSize.Value);
            return BuildResponse(pagedResponse);
        }

        var serviceResponse = await _orderService.GetOrdersAsync(date);
        return BuildResponse(serviceResponse);
    }

    /// <summary>
    /// Obtém o próximo número de pedido disponível
    /// </summary>
    [HttpGet("next-number")]
    [MapPermission([ORDER, MODULE_PDV], OPERATION_SELECT)]
    public async Task<ActionResult<ApiResponse<int>>> GetNextOrderNumber()
    {
        var result = await _orderService.GetNextOrderNumberAsync();
        return BuildResponse(result);
    }

    /// <summary>
    /// Obtém um pedido por ID
    /// </summary>
    [HttpGet("{id}")]
    [MapPermission([ORDER, MODULE_PDV], OPERATION_SELECT)]
    public async Task<ActionResult<ApiResponse<OrderResponseDto>>> GetOrder(Guid id)
    {
        var result = await _orderService.GetOrderByIdAsync(id);
        return BuildResponse(result);
    }

    [HttpPost]
    [MapPermission([ORDER, MODULE_PDV], OPERATION_INSERT)]
    public async Task<ActionResult<ApiResponse<OrderResponseDto>>> CreateOrder(CreateOrderRequestDto request)
    {
        ResponseDTO<OrderResponseDto> result;
        
        if (request.OrderType == EOrderType.Table)
            result = await _orderService.CreateOrderDineInAsync(request);
        else if (request.OrderType == EOrderType.Counter)
            result = await _orderService.CreateOrderPickupAsync(request);
        else
            result = await _orderService.CreateOrderDeliveryAsync(request);
            
        return BuildResponse(result);
    }
    /// <summary>
    /// Cria um pedido pelo PDV
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    //[HttpPost]
    //[MapPermission(ORDER, OPERATION_INSERT)]
    //public async Task<ActionResult<ApiResponse<OrderResponseDto>>> CreateOrderPdv(CreateOrderRequestDto request)
    //{
    //    var result = await _orderService.CreateOrderPickupAsync(request);
    //    return BuildResponse(result);
    //}

    /// <summary>
    /// Adiciona itens a um pedido existente
    /// </summary>
    [HttpPost("{id}/items")]
    [MapPermission(ORDER, OPERATION_UPDATE)]
    public async Task<ActionResult<ApiResponse<OrderResponseDto>>> AddItems(Guid id, [FromBody] List<CreateOrderItemRequestDto> items)
    {
        var result = await _orderService.AddItemsToOrderAsync(id, items);
        return BuildResponse(result);
    }

    /// <summary>
    /// Atualiza o status de um pedido
    /// </summary>
    [HttpPut("{id}/status")]
    [MapPermission(ORDER, OPERATION_UPDATE)]
    public async Task<ActionResult<ApiResponse<OrderResponseDto>>> UpdatEOrderStatus(Guid id, UpdatEOrderStatusRequestDto request)
    {
        var result = await _orderService.UpdatEOrderStatusAsync(id, request);
        return BuildResponse(result);
    }

    /// <summary>
    /// Cancela um pedido (apenas se pendente)
    /// </summary>
    [HttpPut("{id}/cancel")]
    [MapPermission(ORDER, OPERATION_CANCELLATION)]
    public async Task<ActionResult<ApiResponse<OrderResponseDto>>> CancelOrder(Guid id, [FromBody] CancelOrderRequestDto request)
    {
        var result = await _orderService.CancelOrderAsync(id, request);
        return BuildResponse(result);
    }

    /// <summary>
    /// Atualiza o mÃ©todo de pagamento (apenas se pendente)
    /// </summary>
    [HttpPut("{id}/payment")]
    [MapPermission(ORDER, OPERATION_UPDATE)]
    public async Task<ActionResult<ApiResponse<OrderResponseDto>>> UpdatePaymentMethod(Guid id, [FromBody] UpdateOrderPaymentRequestDto request)
    {
        var result = await _orderService.UpdateOrderPaymentMethodAsync(id, request);
        return BuildResponse(result);
    }

    /// <summary>
    /// Atualiza o tipo de entrega (apenas se pendente)
    /// </summary>
    [HttpPut("{id}/delivery-type")]
    [MapPermission(ORDER, OPERATION_UPDATE)]
    public async Task<ActionResult<ApiResponse<OrderResponseDto>>> UpdateDeliveryType(Guid id, [FromBody] UpdateOrderDeliveryTypeRequestDto request)
    {
        var result = await _orderService.UpdateOrderDeliveryTypeAsync(id, request);
        return BuildResponse(result);
    }

    /// <summary>
    /// Aceita um pedido
    /// </summary>
    [HttpPost("{id}/accept")]
    [MapPermission(ORDER, OPERATION_UPDATE)]
    public async Task<ActionResult<ApiResponse<OrderResponseDto>>> AcceptOrder(Guid id, AcceptOrderRequestDto request)
    {
        var result = await _orderService.AcceptOrderAsync(id, request.EstimatedPreparationMinutes, request.Notes);
        return BuildResponse(result);
    }

    /// <summary>
    /// Rejeita um pedido
    /// </summary>
    [HttpPost("{id}/reject")]
    [MapPermission(ORDER, OPERATION_UPDATE)]
    public async Task<ActionResult<ApiResponse<OrderResponseDto>>> RejectOrder(Guid id, RejectOrderRequestDto request)
    {
        var result = await _orderService.RejectOrderAsync(id, request.Reason, request.Notes, request.RejectedBy);
        return BuildResponse(result);
    }

    /// <summary>
    /// Exclui um pedido
    /// </summary>
    [HttpDelete("{id}")]
    [MapPermission(ORDER, OPERATION_DELETE)]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteOrder(Guid id)
    {
        var result = await _orderService.DeleteOrderAsync(id);
        return BuildResponse(result);
    }

    /// <summary>
    /// ObtÃ©m pedidos por status
    /// </summary>
    [HttpGet("status/{status}")]
    [MapPermission(ORDER, OPERATION_SELECT)]
    public async Task<ActionResult<ApiResponse<IEnumerable<OrderResponseDto>>>> GetOrdersByStatus(EOrderStatus status)
    {
        var result = await _orderService.GetOrdersByStatusAsync(status);
        return BuildResponse(result);
    }

    /// <summary>
    /// ObtÃ©m pedidos por telefone do cliente
    /// </summary>
    [HttpGet("customer/{customerPhone}")]
    [MapPermission(ORDER, OPERATION_SELECT)]
    public async Task<ActionResult<ApiResponse<IEnumerable<OrderResponseDto>>>> GetOrdersByCustomer(string customerPhone)
    {
        var result = await _orderService.GetOrdersByCustomerAsync(customerPhone);
        return BuildResponse(result);
    }

    /// <summary>
    /// Atualiza informaÃ§Ãµes bÃ¡sicas de um pedido
    /// </summary>
    [HttpPut("{id}")]
    [MapPermission(ORDER, OPERATION_UPDATE)]
    public async Task<ActionResult<ApiResponse<OrderResponseDto>>> UpdateOrder(Guid id, UpdateOrderRequestDto request)
    {
        var result = await _orderService.UpdateOrderAsync(id, request);
        return BuildResponse(result);
    }

    /// <summary>
    /// Atualiza o status de um item de pedido
    /// </summary>
    [HttpPut("items/{orderItemId}/status")]
    [MapPermission(ORDER, OPERATION_UPDATE)]
    public async Task<ActionResult<ApiResponse<OrderResponseDto>>> UpdateOrderItemStatus(Guid orderItemId, [FromBody] UpdatEOrderStatusRequestDto request)
    {
        var result = await _orderService.UpdateOrderItemStatusAsync(orderItemId, request.Status);
        return BuildResponse(result);
    }
}
