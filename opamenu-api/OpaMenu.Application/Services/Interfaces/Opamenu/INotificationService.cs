using OpaMenu.Domain.DTOs;
using OpaMenu.Domain.DTOs.Product;
using OpaMenu.Infrastructure.Shared.Entities;
using OpaMenu.Infrastructure.Shared.Enums.Opamenu;

namespace OpaMenu.Application.Services.Interfaces.Opamenu;

/// <summary>
/// Interface para serviÃ§o de notificaÃ§Ãµes em tempo real via SignalR
/// </summary>
public interface INotificationService
{
    /// <summary>
    /// Notifica administradores sobre novo pedido criado
    /// </summary>
    Task NotifyNewOrderAsync(OrderResponseDto order);

    /// <summary>
    /// Notifica cliente sobre mudanÃ§a de status do pedido
    /// </summary>
    Task NotifyEOrderStatusChangedAsync(Guid orderId, EOrderStatus oldStatus, EOrderStatus newStatus, string? notes = null);

    /// <summary>
    /// Notifica administradores sobre pedido aceito
    /// </summary>
    Task NotifyOrderAcceptedAsync(OrderResponseDto order);

    /// <summary>
    /// Notifica administradores sobre pedido rejeitado
    /// </summary>
    Task NotifyOrderRejectedAsync(OrderResponseDto order, string reason);

    /// <summary>
    /// Notifica cliente sobre pedido pronto para retirada/entrega
    /// </summary>
    Task NotifyOrderReadyAsync(Guid orderId);

    /// <summary>
    /// Notifica cliente sobre pedido entregue/concluÃ­do
    /// </summary>
    Task NotifyOrderCompletedAsync(Guid orderId);

    /// <summary>
    /// Notifica todos os clientes sobre atualizaÃ§Ã£o do cardÃ¡pio
    /// </summary>
    Task NotifyMenuUpdatedAsync(string changeType, object changedItem);

    /// <summary>
    /// Notifica sobre novo produto adicionado ao cardÃ¡pio
    /// </summary>
    Task NotifyProductAddedAsync(ProductDto product);

    /// <summary>
    /// Notifica sobre produto removido do cardÃ¡pio
    /// </summary>
    Task NotifyProductRemovedAsync(Guid productId, string productName);

    /// <summary>
    /// Notifica sobre mudanÃ§a de preÃ§o de produto
    /// </summary>
    Task NotifyProductPriceChangedAsync(Guid productId, string productName, decimal oldPrice, decimal newPrice);

    /// <summary>
    /// Notifica sobre produto indisponÃ­vel
    /// </summary>
    Task NotifyProductUnavailableAsync(Guid productId, string productName);

    /// <summary>
    /// Envia notificaÃ§Ã£o personalizada para um grupo especÃ­fico
    /// </summary>
    Task SendToGroupAsync(string groupName, string method, object data);

    /// <summary>
    /// Envia notificaÃ§Ã£o personalizada para uma conexÃ£o especÃ­fica
    /// </summary>
    Task SendToConnectionAsync(string connectionId, string method, object data);

    /// <summary>
    /// Envia notificaÃ§Ã£o para todos os clientes conectados
    /// </summary>
    Task SendToAllAsync(string method, object data);

    /// <summary>
    /// Verifica quantos clientes estÃ£o conectados
    /// </summary>
    Task<int> GetConnectedClientsCountAsync();
}
