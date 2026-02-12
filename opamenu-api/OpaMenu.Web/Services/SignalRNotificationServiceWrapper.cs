using Microsoft.AspNetCore.SignalR;
using OpaMenu.Application.Services.Interfaces.Opamenu;
using OpaMenu.Domain.DTOs;
using OpaMenu.Domain.DTOs.Product;
using OpaMenu.Infrastructure.Shared.Entities;
using OpaMenu.Infrastructure.Shared.Enums.Opamenu;
using OpaMenu.Web.Hubs;

namespace OpaMenu.Web.Services;

/// <summary>
/// Wrapper para o NotificationService que usa o Hub específico do SignalR para garantir que as mensagens sejam enviadas corretamente aos clientes conectados.
/// Garante que as notificações sejam enviadas atravÃ©s do OrderNotificationHub correto
/// </summary>
public class SignalRNotificationServiceWrapper : INotificationService
{
    private readonly IHubContext<OrderNotificationHub> _hubContext;
    private readonly ILogger<SignalRNotificationServiceWrapper> _logger;

    public SignalRNotificationServiceWrapper(
        IHubContext<OrderNotificationHub> hubContext,
        ILogger<SignalRNotificationServiceWrapper> logger)
    {
        _hubContext = hubContext;
        _logger = logger;
    }

    public async Task NotifyNewOrderAsync(OrderResponseDto order)
    {
        try
        {
            _logger.LogInformation("[WRAPPER] pedido {orderId}", order.Id);
            
            var notification = new
            {
                Type = "NewOrder",
                OrderId = order.Id,
                CustomerName = order.CustomerName,
                CustomerPhone = order.CustomerPhone,
                TotalAmount = order.Total,
                ItemsCount = order.Items?.Count() ?? 0,
                CreatedAt = order.CreatedAt,
                Message = $"Novo pedido #{order.Id} de {order.CustomerName}",
                Timestamp = DateTime.UtcNow
            };

            _logger.LogInformation("[WRAPPER] Enviando para grupo 'Administrators': {Notification}", 
                System.Text.Json.JsonSerializer.Serialize(notification));

            await _hubContext.Clients.Group("Administrators")
                .SendAsync("NewOrderReceived", notification);

            _logger.LogInformation("âœ… [WRAPPER] Notificação enviada com sucesso: Pedido #{OrderId}", order.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "âŒ [WRAPPER] Erro ao enviar notificação de novo pedido {OrderId}", order.Id);
        }
    }

    public async Task NotifyEOrderStatusChangedAsync(Guid orderId, EOrderStatus oldStatus, EOrderStatus newStatus, string? notes = null)
    {
        try
        {
            var notification = new
            {
                Type = "EOrderStatusChanged",
                OrderId = orderId,
                OldStatus = oldStatus.ToString(),
                NewStatus = newStatus.ToString(),
                StatusMessage = GetStatusMessage(newStatus),
                Notes = notes,
                Timestamp = DateTime.UtcNow
            };

            await _hubContext.Clients.Group($"Order_{orderId}")
                .SendAsync("EOrderStatusUpdated", notification);

            await _hubContext.Clients.Group("Administrators")
                .SendAsync("EOrderStatusChanged", notification);

            _logger.LogInformation("Notificação de mudança de status enviada: Pedido #{OrderId} - {OldStatus} â†’ {NewStatus}", 
                orderId, oldStatus, newStatus);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao enviar notificação de mudança de status do pedido {OrderId}", orderId);
        }
    }

    public async Task NotifyOrderAcceptedAsync(OrderResponseDto order)
    {
        try
        {
            var notification = new
            {
                Type = "OrderAccepted",
                OrderId = order.Id,
                CustomerName = order.CustomerName,
                EstimatedPreparationTime = order.EstimatedPreparationMinutes,
                Message = $"Pedido #{order.Id} aceito - Tempo estimado: {order.EstimatedPreparationMinutes} min",
                Timestamp = DateTime.UtcNow
            };

            await _hubContext.Clients.Group($"Order_{order.Id}")
                .SendAsync("OrderAccepted", notification);

            _logger.LogInformation("Notificação de pedido aceito enviada: Pedido #{OrderId}", order.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao enviar notificação de pedido aceito {OrderId}", order.Id);
        }
    }

    public async Task NotifyOrderRejectedAsync(OrderResponseDto order, string reason)
    {
        try
        {
            var notification = new
            {
                Type = "OrderRejected",
                OrderId = order.Id,
                CustomerName = order.CustomerName,
                Reason = reason,
                Message = $"Pedido #{order.Id} foi rejeitado",
                Timestamp = DateTime.UtcNow
            };

            await _hubContext.Clients.Group($"Order_{order.Id}")
                .SendAsync("OrderRejected", notification);

            _logger.LogInformation("Notificação de pedido rejeitado enviada: Pedido #{OrderId} - Motivo: {Reason}", 
                order.Id, reason);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao enviar Notificação de pedido rejeitado {OrderId}", order.Id);
        }
    }

    public async Task NotifyOrderReadyAsync(Guid orderId)
    {
        try
        {
            var notification = new
            {
                Type = "OrderReady",
                OrderId = orderId,
                Message = $"Seu pedido #{orderId} estÃ¡ pronto!",
                Timestamp = DateTime.UtcNow
            };

            await _hubContext.Clients.Group($"Order_{orderId}")
                .SendAsync("OrderReady", notification);

            _logger.LogInformation("Notificação de pedido pronto enviada: Pedido #{OrderId}", orderId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao enviar notificação de pedido pronto {OrderId}", orderId);
        }
    }

    public async Task NotifyOrderCompletedAsync(Guid orderId)
    {
        try
        {
            var notification = new
            {
                Type = "OrderCompleted",
                OrderId = orderId,
                Message = $"Pedido #{orderId} foi concluÃ­do com sucesso!",
                Timestamp = DateTime.UtcNow
            };

            await _hubContext.Clients.Group($"Order_{orderId}")
                .SendAsync("OrderCompleted", notification);

            _logger.LogInformation("NotificaÃ§Ã£o de pedido concluÃ­do enviada: Pedido #{OrderId}", orderId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao enviar notificaÃ§Ã£o de pedido concluÃ­do {OrderId}", orderId);
        }
    }

    public async Task NotifyMenuUpdatedAsync(string changeType, object changedItem)
    {
        try
        {
            var notification = new
            {
                Type = "MenuUpdated",
                ChangeType = changeType,
                ChangedItem = changedItem,
                Message = $"CardÃ¡pio atualizado: {changeType}",
                Timestamp = DateTime.UtcNow
            };

            await _hubContext.Clients.Group("MenuUpdates")
                .SendAsync("MenuUpdated", notification);

            _logger.LogInformation("NotificaÃ§Ã£o de atualizaÃ§Ã£o do cardÃ¡pio enviada: {ChangeType}", changeType);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao enviar notificaÃ§Ã£o de atualizaÃ§Ã£o do cardÃ¡pio: {ChangeType}", changeType);
        }
    }

    public async Task NotifyProductAddedAsync(ProductDto product)
    {
        await NotifyMenuUpdatedAsync("ProductAdded", new { 
            ProductId = product.Id, 
            ProductName = product.Name,
            CategoryName = product.CategoryName,
            Price = product.Price
        });
    }

    public async Task NotifyProductRemovedAsync(Guid productId, string productName)
    {
        await NotifyMenuUpdatedAsync("ProductRemoved", new { 
            ProductId = productId, 
            ProductName = productName 
        });
    }

    public async Task NotifyProductPriceChangedAsync(Guid productId, string productName, decimal oldPrice, decimal newPrice)
    {
        await NotifyMenuUpdatedAsync("ProductPriceChanged", new { 
            ProductId = productId, 
            ProductName = productName,
            OldPrice = oldPrice,
            NewPrice = newPrice
        });
    }

    public async Task NotifyProductUnavailableAsync(Guid productId, string productName)
    {
        await NotifyMenuUpdatedAsync("ProductUnavailable", new { 
            ProductId = productId, 
            ProductName = productName 
        });
    }

    public async Task SendToGroupAsync(string groupName, string method, object data)
    {
        try
        {
            await _hubContext.Clients.Group(groupName).SendAsync(method, data);
            _logger.LogInformation("NotificaÃ§Ã£o personalizada enviada para grupo {GroupName}: {Method}", 
                groupName, method);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao enviar notificaÃ§Ã£o para grupo {GroupName}", groupName);
        }
    }

    public async Task SendToConnectionAsync(string connectionId, string method, object data)
    {
        try
        {
            await _hubContext.Clients.Client(connectionId).SendAsync(method, data);
            _logger.LogInformation("NotificaÃ§Ã£o personalizada enviada para conexÃ£o {ConnectionId}: {Method}", 
                connectionId, method);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao enviar notificaÃ§Ã£o para conexÃ£o {ConnectionId}", connectionId);
        }
    }

    public async Task SendToAllAsync(string method, object data)
    {
        try
        {
            await _hubContext.Clients.All.SendAsync(method, data);
            _logger.LogInformation("NotificaÃ§Ã£o broadcast enviada: {Method}", method);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao enviar notificaÃ§Ã£o broadcast: {Method}", method);
        }
    }

    public async Task<int> GetConnectedClientsCountAsync()
    {
        try
        {
            await _hubContext.Clients.All.SendAsync("Ping");
            return 1;
        }
        catch
        {
            return 0;
        }
    }

    private static string GetStatusMessage(EOrderStatus status) => status switch
    {
        EOrderStatus.Pending => "Aguardando confirmação",
        EOrderStatus.Preparing => "Em preparação",
        EOrderStatus.Ready => "Pronto para retirada",
        EOrderStatus.OutForDelivery => "Saiu para entrega",
        EOrderStatus.Delivered => "Entregue",
        EOrderStatus.Cancelled => "Cancelado",
        EOrderStatus.Rejected => "Rejeitado",
        _ => "Status desconhecido"
    };
}
