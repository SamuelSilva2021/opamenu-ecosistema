using OpaMenu.Domain.DTOs;
using OpaMenu.Infrastructure.Shared.Entities;
using OpaMenu.Infrastructure.Shared.Enums.Opamenu;

namespace OpaMenu.Application.Services.Interfaces.Opamenu;

/// <summary>
/// Interface para validaÃ§Ãµes de negÃ³cio relacionadas a pedidos
/// </summary>
public interface IOrderValidationService
{
    /// <summary>
    /// Valida a criaÃ§Ã£o de um novo pedido
    /// </summary>
    /// <param name="request">Request de criaÃ§Ã£o do pedido</param>
    /// <returns>Resultado da validaÃ§Ã£o</returns>
    Task<ApiResponse<bool>> ValidateCreateOrderAsync(CreateOrderRequestDto request);
    
    /// <summary>
    /// Valida a atualizaÃ§Ã£o de um pedido
    /// </summary>
    /// <param name="orderId">ID do pedido</param>
    /// <param name="request">Request de atualizaÃ§Ã£o</param>
    /// <returns>Resultado da validaÃ§Ã£o</returns>
    Task<ApiResponse<bool>> ValidateUpdateOrderAsync(Guid orderId, UpdateOrderRequestDto request);
    
    /// <summary>
    /// Valida a mudanÃ§a de status de um pedido
    /// </summary>
    /// <param name="orderId">ID do pedido</param>
    /// <param name="newStatus">Novo status</param>
    /// <returns>Resultado da validaÃ§Ã£o</returns>
    Task<ApiResponse<bool>> ValidateStatusChangeAsync(Guid orderId, EOrderStatus newStatus);
    
    /// <summary>
    /// Valida se um pedido pode ser aceito
    /// </summary>
    /// <param name="orderId">ID do pedido</param>
    /// <param name="request">Request de aceitaÃ§Ã£o</param>
    /// <returns>Resultado da validaÃ§Ã£o</returns>
    Task<ApiResponse<bool>> ValidateAcceptOrderAsync(Guid orderId, AcceptOrderRequestDto request);
    
    /// <summary>
    /// Valida se um pedido pode ser rejeitado
    /// </summary>
    /// <param name="orderId">ID do pedido</param>
    /// <param name="request">Request de rejeiÃ§Ã£o</param>
    /// <returns>Resultado da validaÃ§Ã£o</returns>
    Task<ApiResponse<bool>> ValidateRejectOrderAsync(Guid orderId, RejectOrderRequestDto request);
    
    /// <summary>
    /// Valida se um pedido pode ser cancelado
    /// </summary>
    /// <param name="orderId">ID do pedido</param>
    /// <returns>Resultado da validaÃ§Ã£o</returns>
    Task<ApiResponse<bool>> ValidateCancelOrderAsync(Guid orderId);
    
    /// <summary>
    /// Valida se um pedido pode ser excluÃ­do
    /// </summary>
    /// <param name="orderId">ID do pedido</param>
    /// <returns>Resultado da validaÃ§Ã£o</returns>
    Task<ApiResponse<bool>> ValidateDeleteOrderAsync(Guid orderId);
    
    /// <summary>
    /// Valida os itens de um pedido
    /// </summary>
    /// <param name="items">Lista de itens do pedido</param>
    /// <returns>Resultado da validaÃ§Ã£o</returns>
    Task<ApiResponse<bool>> ValidateOrderItemsAsync(IEnumerable<CreateOrderItemRequestDto> items);
    
    /// <summary>
    /// Valida se uma transiÃ§Ã£o de status Ã© vÃ¡lida
    /// </summary>
    /// <param name="currentStatus">Status atual</param>
    /// <param name="newStatus">Novo status</param>
    /// <returns>True se a transiÃ§Ã£o Ã© vÃ¡lida</returns>
    bool IsValidStatusTransition(EOrderStatus currentStatus, EOrderStatus newStatus);
    
    /// <summary>
    /// Valida informaÃ§Ãµes do cliente
    /// </summary>
    /// <param name="customerName">Nome do cliente</param>
    /// <param name="customerPhone">Telefone do cliente</param>
    /// <returns>Resultado da validaÃ§Ã£o</returns>
    ApiResponse<bool> ValidateCustomerInfo(string customerName, string customerPhone, bool isRequired = true);
}
