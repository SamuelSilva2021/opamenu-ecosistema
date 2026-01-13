using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using OpaMenu.Application.Services.Interfaces;
using OpaMenu.Domain.DTOs;
using OpaMenu.Domain.DTOs.Product;
using OpaMenu.Infrastructure.Shared.Entities;

namespace OpaMenu.Application.Services;

/// <summary>
/// ImplementaÃ§Ã£o do serviÃ§o de notificaÃ§Ãµes em tempo real via SignalR
/// Centraliza todas as notificaÃ§Ãµes da aplicaÃ§Ã£o seguindo princÃ­pios SOLID
/// </summary>
public class NotificationService : INotificationService
{
    private readonly IHubContext<Hub> _hubContext;
    private readonly ILogger<NotificationService> _logger;

    public NotificationService(
        IHubContext<Hub> hubContext,
        ILogger<NotificationService> logger)
    {
        _hubContext = hubContext;
        _logger = logger;
    }

    /// <summary>
    /// Notifica administradores sobre novo pedido criado
    /// </summary>
    public async Task NotifyNewOrderAsync(OrderResponseDto order)
    {
        try
        {
            _logger.LogInformation("Iniciando envio de notificaÃ§Ã£o para novo pedido #{OrderId}", order.Id);
            
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

            var groupClients = _hubContext.Clients.Group("Administrators");
            _logger.LogInformation("Enviando para grupo 'Administrators' - MÃ©todo: NewOrderReceived");
            
            await groupClients.SendAsync("NewOrderReceived", notification);

            _logger.LogInformation("NotificaÃ§Ã£o de novo pedido enviada para administradores: Pedido #{OrderId}", 
                order.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao enviar notificaÃ§Ã£o de novo pedido {OrderId}", order.Id);
        }
    }

    /// <summary>
    /// Notifica cliente sobre mudanÃ§a de status do pedido
    /// </summary>
    public async Task NotifyOrderStatusChangedAsync(int orderId, OrderStatus oldStatus, OrderStatus newStatus, string? notes = null)
    {
        try
        {
            var notification = new
            {
                Type = "OrderStatusChanged",
                OrderId = orderId,
                OldStatus = oldStatus.ToString(),
                NewStatus = newStatus.ToString(),
                StatusMessage = GetStatusMessage(newStatus),
                Notes = notes,
                Timestamp = DateTime.UtcNow
            };

            await _hubContext.Clients.Group($"Order_{orderId}")
                .SendAsync("OrderStatusUpdated", notification);

            await _hubContext.Clients.Group("Administrators")
                .SendAsync("OrderStatusChanged", notification);

            _logger.LogInformation("NotificaÃ§Ã£o de mudanÃ§a de status enviada: Pedido #{OrderId} - {OldStatus} â†’ {NewStatus}", 
                orderId, oldStatus, newStatus);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao enviar notificaÃ§Ã£o de mudanÃ§a de status do pedido {OrderId}", orderId);
        }
    }

    /// <summary>
    /// Notifica administradores sobre pedido aceito
    /// </summary>
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

            _logger.LogInformation("NotificaÃ§Ã£o de pedido aceito enviada: Pedido #{OrderId}", order.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao enviar notificaÃ§Ã£o de pedido aceito {OrderId}", order.Id);
        }
    }

    /// <summary>
    /// Notifica administradores sobre pedido rejeitado
    /// </summary>
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

            _logger.LogInformation("NotificaÃ§Ã£o de pedido rejeitado enviada: Pedido #{OrderId} - Motivo: {Reason}", 
                order.Id, reason);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao enviar notificaÃ§Ã£o de pedido rejeitado {OrderId}", order.Id);
        }
    }

    /// <summary>
    /// Notifica cliente sobre pedido pronto para retirada/entrega
    /// </summary>
    public async Task NotifyOrderReadyAsync(int orderId)
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

            _logger.LogInformation("NotificaÃ§Ã£o de pedido pronto enviada: Pedido #{OrderId}", orderId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao enviar notificaÃ§Ã£o de pedido pronto {OrderId}", orderId);
        }
    }

    /// <summary>
    /// Notifica cliente sobre pedido entregue/concluÃ­do
    /// </summary>
    public async Task NotifyOrderCompletedAsync(int orderId)
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

    /// <summary>
    /// Notifica todos os clientes sobre atualizaÃ§Ã£o do cardÃ¡pio
    /// </summary>
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

    /// <summary>
    /// Notifica sobre novo produto adicionado ao cardÃ¡pio
    /// </summary>
    public async Task NotifyProductAddedAsync(ProductDto product)
    {
        await NotifyMenuUpdatedAsync("ProductAdded", new { 
            ProductId = product.Id, 
            ProductName = product.Name,
            CategoryName = product.CategoryName,
            Price = product.Price
        });
    }

    /// <summary>
    /// Notifica sobre produto removido do cardÃ¡pio
    /// </summary>
    public async Task NotifyProductRemovedAsync(int productId, string productName)
    {
        await NotifyMenuUpdatedAsync("ProductRemoved", new { 
            ProductId = productId, 
            ProductName = productName 
        });
    }

    /// <summary>
    /// Notifica sobre mudanÃ§a de preÃ§o de produto
    /// </summary>
    public async Task NotifyProductPriceChangedAsync(int productId, string productName, decimal oldPrice, decimal newPrice)
    {
        await NotifyMenuUpdatedAsync("ProductPriceChanged", new { 
            ProductId = productId, 
            ProductName = productName,
            OldPrice = oldPrice,
            NewPrice = newPrice
        });
    }

    /// <summary>
    /// Notifica sobre produto indisponÃ­vel
    /// </summary>
    public async Task NotifyProductUnavailableAsync(int productId, string productName)
    {
        await NotifyMenuUpdatedAsync("ProductUnavailable", new { 
            ProductId = productId, 
            ProductName = productName 
        });
    }

    /// <summary>
    /// Envia notificaÃ§Ã£o personalizada para um grupo especÃ­fico
    /// </summary>
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

    /// <summary>
    /// Envia notificaÃ§Ã£o personalizada para uma conexÃ£o especÃ­fica
    /// </summary>
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

    /// <summary>
    /// Envia notificaÃ§Ã£o para todos os clientes conectados
    /// </summary>
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

    /// <summary>
    /// Verifica quantos clientes estÃ£o conectados (aproximado)
    /// </summary>
    public async Task<int> GetConnectedClientsCountAsync()
    {
        // Nota: SignalR nÃ£o oferece uma forma direta de contar conexÃµes
        // Esta implementaÃ§Ã£o bÃ¡sica pode ser expandida com Redis ou cache para precisÃ£o
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

    /// <summary>
    /// ObtÃ©m mensagem amigÃ¡vel para o status do pedido
    /// </summary>
    private static string GetStatusMessage(OrderStatus status) => status switch
    {
        OrderStatus.Pending => "Aguardando confirmaÃ§Ã£o",
        OrderStatus.Confirmed => "Pedido confirmado",
        OrderStatus.Preparing => "Em preparaÃ§Ã£o",
        OrderStatus.Ready => "Pronto para retirada",
        OrderStatus.OutForDelivery => "Saiu para entrega",
        OrderStatus.Delivered => "Entregue",
        OrderStatus.Cancelled => "Cancelado",
        OrderStatus.Rejected => "Rejeitado",
        _ => "Status desconhecido"
    };
}
