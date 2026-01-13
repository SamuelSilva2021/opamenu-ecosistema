using Microsoft.Extensions.Logging;
using OpaMenu.Application.Services.Interfaces;
using OpaMenu.Domain.DTOs;
using OpaMenu.Infrastructure.Shared.Entities;
using OpaMenu.Domain.Interfaces;
using System.Text.RegularExpressions;

namespace OpaMenu.Application.Services;

/// <summary>
/// ServiÃ§o para validaÃ§Ãµes de negÃ³cio relacionadas a pedidos
/// </summary>
public class OrderValidationService(
    IOrderRepository orderRepository,
    IProductRepository productRepository,
    ILogger<OrderValidationService> logger,
    ICurrentUserService currentUserService
    ) : IOrderValidationService
{
    private readonly IOrderRepository _orderRepository = orderRepository;
    private readonly IProductRepository _productRepository = productRepository;
    private readonly ILogger<OrderValidationService> _logger = logger;
    private readonly ICurrentUserService _currentUserService = currentUserService;

    /// <summary>
    /// Valida a criaÃ§Ã£o de um novo pedido
    /// </summary>
    public async Task<ApiResponse<bool>> ValidateCreateOrderAsync(CreateOrderRequestDto request)
    {
        try
        {
            // Validar informaÃ§Ãµes do cliente
            var customerValidation = ValidateCustomerInfo(request.CustomerName, request.CustomerPhone);
            if (!customerValidation.Success)
                return customerValidation;

            // Validar se hÃ¡ itens no pedido
            if (request.Items == null || !request.Items.Any())
            {
                return ApiResponse<bool>.ErrorResponse("O pedido deve conter pelo menos um item.");
            }

            // Validar itens do pedido
            var itemsValidation = await ValidateOrderItemsAsync(request.Items);
            if (!itemsValidation.Success)
                return itemsValidation;

            return ApiResponse<bool>.SuccessResponse(true, "Pedido vÃ¡lido para criaÃ§Ã£o.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao validar criaÃ§Ã£o de pedido");
            return ApiResponse<bool>.ErrorResponse("Erro interno ao validar pedido.");
        }
    }

    /// <summary>
    /// Valida a atualizaÃ§Ã£o de um pedido
    /// </summary>
    public async Task<ApiResponse<bool>> ValidateUpdateOrderAsync(int orderId, UpdateOrderRequestDto request)
    {
        try
        {
            var order = await _orderRepository.GetByIdAsync(orderId, _currentUserService.GetTenantGuid()!.Value);
            if (order == null)
            {
                return ApiResponse<bool>.ErrorResponse("Pedido nÃ£o encontrado.");
            }

            // Verificar se o pedido pode ser atualizado
            if (order.Status == OrderStatus.Delivered || order.Status == OrderStatus.Cancelled)
            {
                return ApiResponse<bool>.ErrorResponse("NÃ£o Ã© possÃ­vel atualizar pedidos finalizados ou cancelados.");
            }

            // Validar informaÃ§Ãµes do cliente se fornecidas
            if (!string.IsNullOrWhiteSpace(request.CustomerName) || !string.IsNullOrWhiteSpace(request.CustomerPhone))
            {
                var customerValidation = ValidateCustomerInfo(
                    request.CustomerName ?? order.CustomerName,
                    request.CustomerPhone ?? order.CustomerPhone);
                if (!customerValidation.Success)
                    return customerValidation;
            }

            return ApiResponse<bool>.SuccessResponse(true, "Pedido vÃ¡lido para atualizaÃ§Ã£o.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao validar atualizaÃ§Ã£o de pedido {OrderId}", orderId);
            return ApiResponse<bool>.ErrorResponse("Erro interno ao validar atualizaÃ§Ã£o.");
        }
    }

    /// <summary>
    /// Valida a mudanÃ§a de status de um pedido
    /// </summary>
    public async Task<ApiResponse<bool>> ValidateStatusChangeAsync(int orderId, OrderStatus newStatus)
    {
        try
        {
            var order = await _orderRepository.GetByIdAsync(orderId, _currentUserService.GetTenantGuid()!.Value);
            if (order == null)
            {
                return ApiResponse<bool>.ErrorResponse("Pedido nÃ£o encontrado.");
            }

            if (!IsValidStatusTransition(order.Status, newStatus))
            {
                return ApiResponse<bool>.BadRequest($"TransiÃ§Ã£o de status invÃ¡lida: de '{order.Status}' para '{newStatus}'.");
            }

            return ApiResponse<bool>.SuccessResponse(true, "TransiÃ§Ã£o de status vÃ¡lida.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao validar transiÃ§Ã£o de status do pedido {OrderId}", orderId);
            return ApiResponse<bool>.ErrorResponse("Erro interno ao validar transiÃ§Ã£o.");
        }
    }

    /// <summary>
    /// Valida se um pedido pode ser aceito
    /// </summary>
    public async Task<ApiResponse<bool>> ValidateAcceptOrderAsync(int orderId, AcceptOrderRequestDto request)
    {
        try
        {
            var order = await _orderRepository.GetByIdAsync(orderId, _currentUserService.GetTenantGuid()!.Value);
            if (order == null)
            {
                return ApiResponse<bool>.ErrorResponse("Pedido nÃ£o encontrado.");
            }

            if (order.Status != OrderStatus.Pending)
            {
                return ApiResponse<bool>.BadRequest("Apenas pedidos pendentes podem ser aceitos.");
            }

            return ApiResponse<bool>.SuccessResponse(true, "Pedido pode ser aceito.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao validar aceitaÃ§Ã£o de pedido {OrderId}", orderId);
            return ApiResponse<bool>.ErrorResponse("Erro interno ao validar aceitaÃ§Ã£o.");
        }
    }

    /// <summary>
    /// Valida se um pedido pode ser rejeitado
    /// </summary>
    public async Task<ApiResponse<bool>> ValidateRejectOrderAsync(int orderId, RejectOrderRequestDto request)
    {
        try
        {
            var order = await _orderRepository.GetByIdAsync(orderId, _currentUserService.GetTenantGuid()!.Value);
            if (order == null)
            {
                return ApiResponse<bool>.ErrorResponse("Pedido nÃ£o encontrado.");
            }

            if (string.IsNullOrWhiteSpace(request.Reason))
            {
                return ApiResponse<bool>.BadRequest("Motivo da rejeiÃ§Ã£o Ã© obrigatÃ³rio.");
            }

            if (order.Status != OrderStatus.Pending && order.Status != OrderStatus.Confirmed)
            {
                return ApiResponse<bool>.BadRequest("Apenas pedidos pendentes ou confirmados podem ser rejeitados.");
            }

            return ApiResponse<bool>.SuccessResponse(true, "Pedido pode ser rejeitado.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao validar rejeiÃ§Ã£o de pedido {OrderId}", orderId);
            return ApiResponse<bool>.ErrorResponse("Erro interno ao validar rejeiÃ§Ã£o.");
        }
    }

    /// <summary>
    /// Valida se um pedido pode ser cancelado
    /// </summary>
    public async Task<ApiResponse<bool>> ValidateCancelOrderAsync(int orderId)
    {
        try
        {
            var order = await _orderRepository.GetByIdAsync(orderId, _currentUserService.GetTenantGuid()!.Value);
            if (order == null)
            {
                return ApiResponse<bool>.ErrorResponse("Pedido nÃ£o encontrado.");
            }

            if (order.Status == OrderStatus.Delivered || order.Status == OrderStatus.Cancelled || order.Status == OrderStatus.Rejected)
            {
                return ApiResponse<bool>.BadRequest($"NÃ£o Ã© possÃ­vel cancelar um pedido com status '{order.Status}'.");
            }

            return ApiResponse<bool>.SuccessResponse(true, "Pedido pode ser cancelado.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao validar cancelamento de pedido {OrderId}", orderId);
            return ApiResponse<bool>.ErrorResponse("Erro interno ao validar cancelamento.");
        }
    }

    /// <summary>
    /// Valida se um pedido pode ser excluÃ­do
    /// </summary>
    public async Task<ApiResponse<bool>> ValidateDeleteOrderAsync(int orderId)
    {
        try
        {
            var order = await _orderRepository.GetByIdAsync(orderId, _currentUserService.GetTenantGuid()!.Value);
            if (order == null)
            {
                return ApiResponse<bool>.ErrorResponse("Pedido nÃ£o encontrado.");
            }

            // Apenas pedidos pendentes ou cancelados podem ser excluÃ­dos
            if (order.Status != OrderStatus.Pending && order.Status != OrderStatus.Cancelled)
            {
                return ApiResponse<bool>.ErrorResponse("Apenas pedidos pendentes ou cancelados podem ser excluÃ­dos.");
            }

            return ApiResponse<bool>.SuccessResponse(true, "Pedido vÃ¡lido para exclusÃ£o.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao validar exclusÃ£o do pedido {OrderId}", orderId);
            return ApiResponse<bool>.ErrorResponse("Erro interno ao validar exclusÃ£o.");
        }
    }

    /// <summary>
    /// Valida os itens de um pedido
    /// </summary>
    public async Task<ApiResponse<bool>> ValidateOrderItemsAsync(IEnumerable<CreateOrderItemRequestDto> items)
    {
        try
        {
            var itemList = items.ToList();
            if (!itemList.Any())
            {
                return ApiResponse<bool>.BadRequest("O pedido deve conter pelo menos um item.");
            }

            foreach (var item in itemList)
            {
                var itemValidation = await ValidateItemAsync(item);
                if (!itemValidation.Success)
                {
                    return itemValidation;
                }
            }

            return ApiResponse<bool>.SuccessResponse(true, "Todos os itens sÃ£o vÃ¡lidos.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao validar itens do pedido");
            return ApiResponse<bool>.ErrorResponse("Erro interno ao validar itens.");
        }
    }

    /// <summary>
    /// Valida um item do pedido
    /// </summary>
    public async Task<ApiResponse<bool>> ValidateItemAsync(CreateOrderItemRequestDto item)
    {
        try
        {
            if (item.Quantity <= 0)
            {
                return ApiResponse<bool>.BadRequest("Quantidade deve ser maior que zero.");
            }

            // O preÃ§o unitÃ¡rio serÃ¡ obtido do produto, nÃ£o do DTO

            var product = await _productRepository.GetByIdAsync(item.ProductId, _currentUserService.GetTenantGuid()!.Value);
            if (product == null)
            {
                return ApiResponse<bool>.BadRequest($"Produto com ID {item.ProductId} nÃ£o encontrado.");
            }

            if (!product.IsActive)
            {
                return ApiResponse<bool>.BadRequest($"Produto '{product.Name}' nÃ£o estÃ¡ disponÃ­vel.");
            }

            return ApiResponse<bool>.SuccessResponse(true, "Item vÃ¡lido.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao validar item do pedido {ProductId}", item.ProductId);
            return ApiResponse<bool>.ErrorResponse("Erro interno ao validar item.");
        }
    }

    /// <summary>
    /// Valida se uma transiÃ§Ã£o de status Ã© vÃ¡lida
    /// </summary>
    public bool IsValidStatusTransition(OrderStatus currentStatus, OrderStatus newStatus)
    {
        return currentStatus switch
        {
            OrderStatus.Pending => newStatus is OrderStatus.Confirmed or OrderStatus.Rejected or OrderStatus.Cancelled,
            OrderStatus.Confirmed => newStatus is OrderStatus.Preparing or OrderStatus.Rejected or OrderStatus.Cancelled,
            OrderStatus.Preparing => newStatus is OrderStatus.Ready or OrderStatus.Cancelled,
            OrderStatus.Ready => newStatus is OrderStatus.OutForDelivery or OrderStatus.Delivered or OrderStatus.Cancelled,
            OrderStatus.OutForDelivery => newStatus is OrderStatus.Delivered or OrderStatus.Cancelled,
            OrderStatus.Delivered => false, // Status final
            OrderStatus.Rejected => false, // Status final
            OrderStatus.Cancelled => false, // Status final
            _ => false
        };
    }

    /// <summary>
    /// Valida informaÃ§Ãµes do cliente
    /// </summary>
    public ApiResponse<bool> ValidateCustomerInfo(string? customerName, string? customerPhone, bool isRequired = true)
    {
        try
        {
            // Se nÃ£o Ã© obrigatÃ³rio e ambos estÃ£o vazios, Ã© vÃ¡lido (cliente anÃ´nimo)
            if (!isRequired && string.IsNullOrWhiteSpace(customerName) && string.IsNullOrWhiteSpace(customerPhone))
            {
                return ApiResponse<bool>.SuccessResponse(true, "Cliente anÃ´nimo vÃ¡lido.");
            }

            if (isRequired && string.IsNullOrWhiteSpace(customerName))
            {
                return ApiResponse<bool>.BadRequest("Nome do cliente Ã© obrigatÃ³rio.");
            }

            if (!string.IsNullOrWhiteSpace(customerName) && customerName.Length < 2)
            {
                return ApiResponse<bool>.BadRequest("Nome do cliente deve ter pelo menos 2 caracteres.");
            }

            if (!string.IsNullOrWhiteSpace(customerPhone))
            {
                // Validar formato do telefone (apenas nÃºmeros, com 10 ou 11 dÃ­gitos)
                var phoneRegex = new Regex(@"^\d{10,11}$");
                var cleanPhone = customerPhone.Replace("[", "").Replace("]", "").Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ", "");
                if (!phoneRegex.IsMatch(cleanPhone))
                {
                    return ApiResponse<bool>.BadRequest("Formato de telefone invÃ¡lido.");
                }
            }

            return ApiResponse<bool>.SuccessResponse(true, "InformaÃ§Ãµes do cliente vÃ¡lidas.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao validar informaÃ§Ãµes do cliente");
            return ApiResponse<bool>.ErrorResponse("Erro interno ao validar cliente.");
        }
    }
}
