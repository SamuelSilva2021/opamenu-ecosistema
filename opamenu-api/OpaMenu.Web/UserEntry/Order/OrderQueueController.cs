using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using OpaMenu.Application.Services.Interfaces;
using OpaMenu.Domain.DTOs;
using OpaMenu.Infrastructure.Shared.Entities;
using OpaMenu.Web.Utils;
using OpaMenu.Web.UserEntry;

namespace OpaMenu.Web.UserEntry.Order;

[ApiController]
[Route("api/[controller]")]
[Authorize]
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
    public async Task<ActionResult<ApiResponse<IEnumerable<OrderResponseDto>>>> GetOrderQueue(
        [FromQuery] OrderStatus? status = null,
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
    public async Task<ActionResult<ApiResponse<OrderResponseDto>>> AcceptOrder(int id, [FromBody] AcceptOrderRequestDto request)
    {
        var result = await _orderService.AcceptOrderAsync(id, request.EstimatedPreparationMinutes, request.Notes, request.UserId);
        return BuildResponse(result);
    }

    /// <summary>
    /// Reject an order with a reason
    /// </summary>
    [HttpPost("{id}/reject")]
    public async Task<ActionResult<ApiResponse<OrderResponseDto>>> RejectOrder(int id, [FromBody] RejectOrderRequestDto request)
    {
        var result = await _orderService.RejectOrderAsync(id, request.Reason, request.Notes, request.RejectedBy);
        return BuildResponse(result);
    }

    /// <summary>
    /// Update order status in the preparation workflow
    /// </summary>
    [HttpPut("{id}/status")]
    public async Task<ActionResult<ApiResponse<OrderResponseDto>>> UpdateOrderStatus(int id, [FromBody] UpdateOrderStatusRequestDto request)
    {
        var result = await _orderService.UpdateOrderStatusAsync(id, request);
        return BuildResponse(result);
    }

}

