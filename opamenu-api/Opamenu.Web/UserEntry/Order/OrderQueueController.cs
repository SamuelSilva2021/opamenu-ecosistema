using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Opamenu.Domain.DTOs;
using Opamenu.Infrastructure.Shared.Entities;
using Opamenu.Web.Utils;
using Opamenu.Web.UserEntry;
using Opamenu.Infrastructure.Shared.Enums.Opamenu;
using Opamenu.Infrastructure.Anotations;
using Opamenu.Infrastructure.Filters;
using Opamenu.Application.Services.Interfaces.Opamenu;

namespace Opamenu.Web.UserEntry.Order;

[ApiController]
[Route("api/[controller]")]
[Authorize]
[ServiceFilter(typeof(PermissionAuthorizationFilter))]
public class OrderQueueController : BaseController
{
    private readonly IOrderService _orderService;
    private readonly ILogger<OrderQueueController> _logger;

    public OrderQueueController(IOrderService orderService, ILogger<OrderQueueController> logger)
    {
        _orderService = orderService;
        _logger = logger;
    }

    /// <summary>
    /// Get orders in queue with optional filtering
    /// </summary>
    [HttpGet]
    [MapPermission(ORDER, OPERATION_SELECT)]
    public async Task<ActionResult<ApiResponse<IEnumerable<OrderResponseDto>>>> GetOrderQueue(
        [FromQuery] EOrderStatus? status = null,
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null,
        [FromQuery] string? searchTerm = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50)
    {
        var result = await _orderService.GetOrdersAsync();
        return BuildResponse(result);
    }

    /// <summary>
    /// Accept an order and add it to the preparation queue
    /// </summary>
    [HttpPost("{id}/accept")]
    [MapPermission(ORDER, OPERATION_UPDATE)]
    public async Task<ActionResult<ApiResponse<OrderResponseDto>>> AcceptOrder(Guid id, [FromBody] AcceptOrderRequestDto request)
    {
        var result = await _orderService.AcceptOrderAsync(id, request.EstimatedPreparationMinutes, request.Notes);
        return BuildResponse(result);
    }

    /// <summary>
    /// Reject an order with a reason
    /// </summary>
    [HttpPost("{id}/reject")]
    [MapPermission(ORDER, OPERATION_UPDATE)]
    public async Task<ActionResult<ApiResponse<OrderResponseDto>>> RejectOrder(Guid id, [FromBody] RejectOrderRequestDto request)
    {
        var result = await _orderService.RejectOrderAsync(id, request.Reason, request.Notes, request.RejectedBy);
        return BuildResponse(result);
    }

    /// <summary>
    /// Update order status in the preparation workflow
    /// </summary>
    [HttpPut("{id}/status")]
    [MapPermission(ORDER, OPERATION_UPDATE)]
    public async Task<ActionResult<ApiResponse<OrderResponseDto>>> UpdatEOrderStatus(Guid id, [FromBody] UpdatEOrderStatusRequestDto request)
    {
        var result = await _orderService.UpdatEOrderStatusAsync(id, request);
        return BuildResponse(result);
    }

}

