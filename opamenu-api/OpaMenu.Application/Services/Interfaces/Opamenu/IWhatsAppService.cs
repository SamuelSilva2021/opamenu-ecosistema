using OpaMenu.Infrastructure.Shared.Enums.Opamenu;

namespace OpaMenu.Application.Services.Interfaces.Opamenu;

/// <summary>
/// Interface para o motor de WhatsApp (Envio de mensagens e automação)
/// </summary>
public interface IWhatsAppService
{
    /// <summary>
    /// Envia uma mensagem de texto simples
    /// </summary>
    Task<bool> SendTextMessageAsync(Guid tenantId, string phoneNumber, string message);

    /// <summary>
    /// Envia o link do cardápio digital
    /// </summary>
    Task<bool> SendMenuLinkAsync(Guid tenantId, string phoneNumber);

    /// <summary>
    /// Envia uma atualização de status de pedido
    /// </summary>
    Task<bool> SendOrderStatusUpdateAsync(Guid tenantId, string phoneNumber, string orderNumber, EOrderStatus newStatus);

    /// <summary>
    /// Processa uma mensagem recebida (Recepção de Webhook)
    /// </summary>
    Task ProcessIncomingMessageAsync(Guid tenantId, string phoneNumber, string message);

    /// <summary>
    /// Verifica se a instância do WhatsApp está conectada
    /// </summary>
    Task<bool> IsInstanceConnectedAsync(Guid tenantId);
}
