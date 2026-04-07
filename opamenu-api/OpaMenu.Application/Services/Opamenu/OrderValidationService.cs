using Microsoft.Extensions.Logging;
using OpaMenu.Domain.DTOs;
using OpaMenu.Infrastructure.Shared.Entities;
using OpaMenu.Domain.Interfaces;
using System.Text.RegularExpressions;
using OpaMenu.Infrastructure.Shared.Enums.Opamenu;
using OpaMenu.Application.Services.Interfaces.Opamenu;

namespace OpaMenu.Application.Services.Opamenu;

/// <summary>
/// Serviço de validação de pedidos
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
    /// Valida a criação de um novo pedido
    /// </summary>
    public async Task<ApiResponse<bool>> ValidateCreateOrderAsync(CreateOrderRequestDto request)
    {
        try
        {
            // Validar informações do cliente
            var customerValidation = ValidateCustomerInfo(request.CustomerName, request.CustomerPhone);
            if (!customerValidation.Success)
                return customerValidation;

            // Validar se há itens no pedido
            if (request.Items == null || !request.Items.Any())
            {
                return ApiResponse<bool>.ErrorResponse("O pedido deve conter pelo menos um item.");
            }

            // Validar itens do pedido
            var itemsValidation = await ValidateOrderItemsAsync(request.Items);
            if (!itemsValidation.Success)
                return itemsValidation;

            return ApiResponse<bool>.SuccessResponse(true, "Pedido válido para criação.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao validar criação de pedido");
            return ApiResponse<bool>.ErrorResponse("Erro interno ao validar pedido.");
        }
    }

    /// <summary>
    /// Valida a atualização de um pedido
    /// </summary>
    public async Task<ApiResponse<bool>> ValidateUpdateOrderAsync(Guid orderId, UpdateOrderRequestDto request)
    {
        try
        {
            var order = await _orderRepository.GetByIdAsync(orderId, _currentUserService.GetTenantGuid()!.Value);
            if (order == null)
            {
                return ApiResponse<bool>.ErrorResponse("Pedido não encontrado.");
            }

            // Verificar se o pedido pode ser atualizado
            if (order.Status == EOrderStatus.Delivered || order.Status == EOrderStatus.Cancelled)
            {
                return ApiResponse<bool>.ErrorResponse("Não é possível atualizar pedidos finalizados ou cancelados.");
            }

            // Validar informações do cliente se fornecidas
            if (!string.IsNullOrWhiteSpace(request.CustomerName) || !string.IsNullOrWhiteSpace(request.CustomerPhone))
            {
                var customerValidation = ValidateCustomerInfo(
                    request.CustomerName ?? order.CustomerName,
                    request.CustomerPhone ?? order.CustomerPhone);
                if (!customerValidation.Success)
                    return customerValidation;
            }

            return ApiResponse<bool>.SuccessResponse(true, "Pedido válido para atualização.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao validar atualização de pedido {OrderId}", orderId);
            return ApiResponse<bool>.ErrorResponse("Erro interno ao validar atualização.");
        }
    }

    /// <summary>
    /// Valida a mudança de status de um pedido
    /// </summary>
    public async Task<ApiResponse<bool>> ValidateStatusChangeAsync(Guid orderId, EOrderStatus newStatus)
    {
        try
        {
            var order = await _orderRepository.GetByIdAsync(orderId, _currentUserService.GetTenantGuid()!.Value);
            if (order == null)
            {
                return ApiResponse<bool>.ErrorResponse("Pedido não encontrado.");
            }

            if (!IsValidStatusTransition(order.Status, newStatus))
            {
                return ApiResponse<bool>.BadRequest($"Transição de status inválida: de '{order.Status}' para '{newStatus}'.");
            }

            return ApiResponse<bool>.SuccessResponse(true, "Transição de status válida.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao validar transição de status do pedido {OrderId}", orderId);
            return ApiResponse<bool>.ErrorResponse("Erro interno ao validar transição.");
        }
    }

    /// <summary>
    /// Valida se um pedido pode ser aceito
    /// </summary>
    public async Task<ApiResponse<bool>> ValidateAcceptOrderAsync(Guid orderId, AcceptOrderRequestDto request)
    {
        try
        {
            var order = await _orderRepository.GetByIdAsync(orderId, _currentUserService.GetTenantGuid()!.Value);
            if (order == null)
            {
                return ApiResponse<bool>.ErrorResponse("Pedido não encontrado.");
            }

            if (order.Status != EOrderStatus.Pending)
            {
                return ApiResponse<bool>.BadRequest("Apenas pedidos pendentes podem ser aceitos.");
            }

            return ApiResponse<bool>.SuccessResponse(true, "Pedido pode ser aceito.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao validar aceitação de pedido {OrderId}", orderId);
            return ApiResponse<bool>.ErrorResponse("Erro interno ao validar aceitação.");
        }
    }

    /// <summary>
    /// Valida se um pedido pode ser rejeitado
    /// </summary>
    public async Task<ApiResponse<bool>> ValidateRejectOrderAsync(Guid orderId, RejectOrderRequestDto request)
    {
        try
        {
            var order = await _orderRepository.GetByIdAsync(orderId, _currentUserService.GetTenantGuid()!.Value);
            if (order == null)
            {
                return ApiResponse<bool>.ErrorResponse("Pedido não encontrado.");
            }

            if (string.IsNullOrWhiteSpace(request.Reason))
            {
                return ApiResponse<bool>.BadRequest("Motivo da rejeição é obrigatório.");
            }

            if (order.Status != EOrderStatus.Pending && order.Status != EOrderStatus.Preparing)
            {
                return ApiResponse<bool>.BadRequest("Apenas pedidos pendentes ou em preparo podem ser rejeitados.");
            }

            return ApiResponse<bool>.SuccessResponse(true, "Pedido pode ser rejeitado.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao validar rejeição de pedido {OrderId}", orderId);
            return ApiResponse<bool>.ErrorResponse("Erro interno ao validar rejeição.");
        }
    }

    /// <summary>
    /// Valida se um pedido pode ser cancelado
    /// </summary>
    public async Task<ApiResponse<bool>> ValidateCancelOrderAsync(Guid orderId)
    {
        try
        {
            var order = await _orderRepository.GetByIdAsync(orderId, _currentUserService.GetTenantGuid()!.Value);
            if (order == null)
            {
                return ApiResponse<bool>.ErrorResponse("Pedido não encontrado.");
            }

            if (order.Status == EOrderStatus.Delivered || order.Status == EOrderStatus.Cancelled || order.Status == EOrderStatus.Rejected)
            {
                return ApiResponse<bool>.BadRequest($"Não é possível cancelar um pedido com status '{order.Status}'.");
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
    /// Valida se um pedido pode ser excluído
    /// </summary>
    public async Task<ApiResponse<bool>> ValidateDeleteOrderAsync(Guid orderId)
    {
        try
        {
            var order = await _orderRepository.GetByIdAsync(orderId, _currentUserService.GetTenantGuid()!.Value);
            if (order == null)
            {
                return ApiResponse<bool>.ErrorResponse("Pedido não encontrado.");
            }

            // Apenas pedidos pendentes ou cancelados podem ser excluídos
            if (order.Status != EOrderStatus.Pending && order.Status != EOrderStatus.Cancelled)
            {
                return ApiResponse<bool>.ErrorResponse("Apenas pedidos pendentes ou cancelados podem ser excluídos.");
            }

            return ApiResponse<bool>.SuccessResponse(true, "Pedido válido para exclusão.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao validar exclusão do pedido {OrderId}", orderId);
            return ApiResponse<bool>.ErrorResponse("Erro interno ao validar exclusão.");
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

            return ApiResponse<bool>.SuccessResponse(true, "Todos os itens são válidos.");
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

            // O preço unitário será obtido do produto, não do DTO

            var product = await _productRepository.GetByIdAsync(item.ProductId, _currentUserService.GetTenantGuid()!.Value);
            if (product == null)
            {
                return ApiResponse<bool>.BadRequest($"Produto com ID {item.ProductId} não encontrado.");
            }

            if (!product.IsActive)
            {
                return ApiResponse<bool>.BadRequest($"Produto '{product.Name}' não está disponível.");
            }

            return ApiResponse<bool>.SuccessResponse(true, "Item válido.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao validar item do pedido {ProductId}", item.ProductId);
            return ApiResponse<bool>.ErrorResponse("Erro interno ao validar item.");
        }
    }

    /// <summary>
    /// Valida se uma transição de status é válida
    /// </summary>
    public bool IsValidStatusTransition(EOrderStatus currentStatus, EOrderStatus newStatus)
    {
        return currentStatus switch
        {
            EOrderStatus.Pending => newStatus is EOrderStatus.Preparing or EOrderStatus.Rejected or EOrderStatus.Cancelled,
            EOrderStatus.Preparing => newStatus is EOrderStatus.Ready or EOrderStatus.Rejected or EOrderStatus.Cancelled,
            EOrderStatus.Ready => newStatus is EOrderStatus.OutForDelivery or EOrderStatus.Delivered or EOrderStatus.Cancelled,
            EOrderStatus.OutForDelivery => newStatus is EOrderStatus.Delivered or EOrderStatus.Cancelled,
            EOrderStatus.Delivered => false, // Status final
            EOrderStatus.Rejected => false, // Status final
            EOrderStatus.Cancelled => false, // Status final
            _ => false
        };
    }

    /// <summary>
    /// Valida informações do cliente
    /// </summary>
    public ApiResponse<bool> ValidateCustomerInfo(string? customerName, string? customerPhone, bool isRequired = true)
    {
        try
        {
            // Se não é obrigatório e ambos estão vazios, é válido (cliente anônimo)
            if (!isRequired && string.IsNullOrWhiteSpace(customerName) && string.IsNullOrWhiteSpace(customerPhone))
            {
                return ApiResponse<bool>.SuccessResponse(true, "Cliente anônimo válido.");
            }

            if (isRequired && string.IsNullOrWhiteSpace(customerName))
            {
                return ApiResponse<bool>.BadRequest("Nome do cliente é obrigatório.");
            }

            if (!string.IsNullOrWhiteSpace(customerName) && customerName.Length < 2)
            {
                return ApiResponse<bool>.BadRequest("Nome do cliente deve ter pelo menos 2 caracteres.");
            }

            if (!string.IsNullOrWhiteSpace(customerPhone))
            {
                // Validar formato do telefone (apenas números, com 10 ou 11 dígitos)
                var phoneRegex = new Regex(@"^\d{10,11}$");
                var cleanPhone = customerPhone.Replace("[", "").Replace("]", "").Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ", "");
                if (!phoneRegex.IsMatch(cleanPhone))
                {
                    return ApiResponse<bool>.BadRequest("Formato de telefone inválido.");
                }
            }

            return ApiResponse<bool>.SuccessResponse(true, "Informações do cliente válidas.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao validar informações do cliente");
            return ApiResponse<bool>.ErrorResponse("Erro interno ao validar cliente.");
        }
    }
}
