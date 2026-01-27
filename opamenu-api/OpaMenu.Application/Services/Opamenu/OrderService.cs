using AutoMapper;
using Microsoft.Extensions.Logging;
using OpaMenu.Application.DTOs;
using OpaMenu.Application.Services.Interfaces.Opamenu;
using OpaMenu.Commons.Api.Commons;
using OpaMenu.Commons.Api.DTOs;
using OpaMenu.Domain.DTOs;
using OpaMenu.Domain.DTOs.Addons;
using OpaMenu.Domain.DTOs.Order;
using OpaMenu.Domain.Interfaces;
using OpaMenu.Infrastructure.Shared.Entities;
using OpaMenu.Infrastructure.Shared.Entities.Opamenu;
using OpaMenu.Infrastructure.Shared.Enums.Opamenu;
using OpaMenu.Web.Models.DTOs;
using System.Threading.Tasks;

namespace OpaMenu.Application.Services.Opamenu;

/// <summary>
/// ImplementaÃ§Ã£o do serviÃ§o de pedidos seguindo princÃ­pios SOLID e Clean Architecture
/// </summary>
public class OrderService(
    IOrderRepository orderRepository,
    IProductRepository productRepository,
    IAddonRepository addonRepository,
    ICouponRepository couponRepository,
    INotificationService notificationService,
    ICurrentUserService currentUserService,
    ILogger<OrderService> logger,
    ITenantCustomerRepository tenantCustomerRepository,
    ICustomerRepository customerRepository,
    ITenantRepository tenantRepository,
    ITableRepository tableRepository,
    ILoyaltyService loyaltyService,
    IMapper mapper
    ) : IOrderService
{
    private readonly IOrderRepository _orderRepository = orderRepository;
    private readonly IProductRepository _productRepository = productRepository;
    private readonly IAddonRepository _addonRepository = addonRepository;
    private readonly ICouponRepository _couponRepository = couponRepository;
    private readonly INotificationService _notificationService = notificationService;
    private readonly ICurrentUserService _currentUserService = currentUserService;
    private readonly ITenantCustomerRepository _tenantCustomerRepository = tenantCustomerRepository;
    private readonly ICustomerRepository _customerRepository = customerRepository;
    private readonly ITenantRepository _tenantRepository = tenantRepository;
    private readonly ITableRepository _tableRepository = tableRepository;
    private readonly ILoyaltyService _loyaltyService = loyaltyService;
    private readonly ILogger<OrderService> _logger = logger;
    private readonly IMapper _mapper = mapper;

    /// <summary>
    /// ObtÃ©m todos os pedidos
    /// </summary>
    public async Task<ResponseDTO<IEnumerable<OrderResponseDto>>> GetOrdersAsync()
    {
        try
        {
            var orders = await _orderRepository.GetAllByTenantIdWithIncludesAsync(_currentUserService.GetTenantGuid()!.Value,
                o => o.Items,
                o => o.StatusHistory,
                o => o.Rejection!);

            var orderDtos = _mapper.Map<IEnumerable<OrderResponseDto>>(orders);

            return StaticResponseBuilder<IEnumerable<OrderResponseDto>>.BuildOk(orderDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter pedidos");
            return StaticResponseBuilder<IEnumerable<OrderResponseDto>>.BuildError("Erro interno do servidor");
        }
    }
    /// <summary>
    /// ObtÃ©m um pedido por ID
    /// </summary>
    public async Task<ResponseDTO<OrderResponseDto>> GetOrderByIdAsync(Guid id)
    {
        try
        {
            var order = await _orderRepository.GetByIdWithIncludesAsync(id,
                o => o.Items,
                o => o.StatusHistory,
                o => o.Rejection!);

            var orderDto = _mapper.Map<OrderResponseDto>(order);
            return StaticResponseBuilder<OrderResponseDto>.BuildOk(orderDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter pedido {OrderId}", id);
            return StaticResponseBuilder<OrderResponseDto>.BuildError("Erro interno do servidor");
        }
    }
    public async Task<ResponseDTO<OrderResponseDto>> GetPublicOrderByIdAsync(string slug, Guid id)
    {
        try
        {
            var tenant = await _tenantRepository.GetBySlugAsync(slug);
            if (tenant == null)
                return StaticResponseBuilder<OrderResponseDto>.BuildOk(new OrderResponseDto { });

            var order = await _orderRepository.GetByIdWithIncludesAsync(id,
                o => o.Items,
                o => o.StatusHistory,
                o => o.Rejection!);

            if (order == null || order.TenantId != tenant.Id)
                return StaticResponseBuilder<OrderResponseDto>.BuildError("Pedido não encontrado");

            var orderDto = _mapper.Map<OrderResponseDto>(order);
            return StaticResponseBuilder<OrderResponseDto>.BuildOk(orderDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter pedido pÃºblico {OrderId} para o estabelecimento {Slug}", id, slug);
            return StaticResponseBuilder<OrderResponseDto>.BuildError("Erro interno do servidor");
        }
    }
    public async Task<ResponseDTO<IEnumerable<OrderResponseDto>>> GetPublicOrdersByCustomerIdAsync(string slug, Guid customerId)
    {
        try
        {
            var tenant = await _tenantRepository.GetBySlugAsync(slug);
            if (tenant == null)
                return StaticResponseBuilder<IEnumerable<OrderResponseDto>>.BuildOk([]);

            var order = await _orderRepository.GetByCustomerIdAndTenantIdAsync(tenant!.Id, customerId);

            var orderDtos = _mapper.Map<IEnumerable<OrderResponseDto>>(order);
            return StaticResponseBuilder<IEnumerable<OrderResponseDto>>.BuildOk(orderDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter pedidos pÃºblicos para o cliente {CustomerId} no estabelecimento {Slug}", customerId, slug);
            return StaticResponseBuilder<IEnumerable<OrderResponseDto>>.BuildError("Erro interno do servidor");
        }
        
    }
    /// <summary>
    /// Cria um novo pedido
    /// </summary>
    public async Task<ResponseDTO<OrderResponseDto>> CreateOrderDeliveryAsync(CreateOrderRequestDto requestDto)
    {
        try
        {
            var tenantId = _currentUserService.GetTenantGuid()!.Value;

            var (IsValid, ErrorMessage) = await ValidateOrderItemsAsync(requestDto.Items, tenantId);

            if (!IsValid)
                return StaticResponseBuilder<OrderResponseDto>.BuildError(ErrorMessage);

            var existingCustomer = await _customerRepository.GetByPhoneAsync(tenantId, requestDto.CustomerPhone!);

            //Se não possuir um cliente associado ao telefone, cria um novo
            if (existingCustomer == null)
            {
                //Cria um novo cliente e associa ao tenant
                existingCustomer = await CreateCustomer(requestDto);
                await CreateTenantCustomer(tenantId, existingCustomer.Id);
            }

            var tenantCustomer = await _tenantCustomerRepository.GetByTenantIdAndCustomerIdAsync(tenantId, existingCustomer.Id);
            //Se nÃ£o existir jÃ¡ cria um novo e atribui
            tenantCustomer ??= await CreateTenantCustomer(tenantId, existingCustomer.Id);


            var order = new OrderEntity
            {
                CustomerName = requestDto.CustomerName!,
                CustomerPhone = requestDto.CustomerPhone!,
                CustomerEmail = requestDto.CustomerEmail,
                DeliveryAddress = FormatDeliveryAddress(requestDto.DeliveryAddress),
                IsDelivery = requestDto.OrderType == EOrderType.Delivery,
                OrderType = requestDto.OrderType,
                TableId = requestDto.OrderType == EOrderType.Table ? requestDto.TableId : null,
                Notes = requestDto.Notes,
                Status = EOrderStatus.Pending,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                CustomerId = tenantCustomer.Customer.Id,
                TenantId = tenantId
            };

            // Criar itens do pedido
            foreach (var itemDto in requestDto.Items)
            {
                var product = await _productRepository.GetByIdAsync(itemDto.ProductId, tenantId);
                if (product == null) continue;

                var orderItem = new OrderItemEntity
                {
                    ProductId = itemDto.ProductId,
                    ProductName = product.Name,
                    UnitPrice = product.Price,
                    Quantity = itemDto.Quantity,
                    Notes = itemDto.Notes,
                    Subtotal = product.Price * itemDto.Quantity
                };

                // Adicionar addons
                foreach (var addonDto in itemDto.Addons)
                {
                    var addon = await _addonRepository.GetByIdAsync(addonDto.AddonId, tenantId);
                    if (addon == null) continue;

                    var orderItemAddon = new OrderItemAddonEntity
                    {
                        AddonId = addonDto.AddonId,
                        AddonName = addon.Name,
                        UnitPrice = addon.Price,
                        Quantity = addonDto.Quantity,
                        Subtotal = addon.Price * addonDto.Quantity
                    };

                    orderItem.Addons.Add(orderItemAddon);
                    orderItem.Subtotal += orderItemAddon.Subtotal;
                }

                order.Items.Add(orderItem);
            }

            // Calcular totais
            order.Subtotal = order.Items.Sum(i => i.Subtotal);
            order.DeliveryFee = requestDto.DeliveryFee ?? 0.0m;
            
            // Iniciar transação para garantir atomicidade entre atualização do cupom e criação do pedido
            using (var scope = new System.Transactions.TransactionScope(System.Transactions.TransactionScopeAsyncFlowOption.Enabled))
            {
                // Processar cupom
                if (!string.IsNullOrEmpty(requestDto.CouponCode))
                {
                    var coupon = await _couponRepository.GetByCodeAsync(requestDto.CouponCode, tenantId);
                    if (coupon != null && coupon.IsActive)
                    {
                        // Validar regras do cupom
                        var now = DateTime.UtcNow;
                        bool isValid = true;
                        
                        if (coupon.StartDate.HasValue && coupon.StartDate > now) isValid = false;
                        if (coupon.ExpirationDate.HasValue && coupon.ExpirationDate < now) isValid = false;
                        if (coupon.UsageLimit.HasValue && coupon.UsageCount >= coupon.UsageLimit) isValid = false;
                        if (coupon.MinOrderValue.HasValue && order.Subtotal < coupon.MinOrderValue) isValid = false;

                        if (isValid)
                        {
                            decimal discount = 0;
                            if (coupon.DiscountType == EDiscountType.Porcentagem)
                            {
                                discount = order.Subtotal * (coupon.DiscountValue / 100);
                                if (coupon.MaxDiscountValue.HasValue && discount > coupon.MaxDiscountValue.Value)
                                {
                                    discount = coupon.MaxDiscountValue.Value;
                                }
                            }
                            else
                            {
                                discount = coupon.DiscountValue;
                            }

                            // Garantir que o desconto não seja maior que o subtotal
                            if (discount > order.Subtotal) discount = order.Subtotal;

                            order.DiscountAmount = discount;
                            order.CouponCode = coupon.Code;
                            
                            // Atualizar uso do cupom
                            coupon.UsageCount++;
                            await _couponRepository.UpdateAsync(coupon);
                        }
                    }
                }

                order.Total = order.Subtotal + order.DeliveryFee - order.DiscountAmount;
                if (order.Total < 0) order.Total = 0;

                var createdOrder = await _orderRepository.AddAsync(order);
                var orderDto = _mapper.Map<OrderResponseDto>(createdOrder);

                _logger.LogInformation("Pedido {OrderId} criado com sucesso", createdOrder.Id);

                // Commit da transação
                scope.Complete();

                // Processar pontos de fidelidade (fora da transação principal para não bloquear, mas idealmente deveria ser resiliente)
                try
                {
                    await _loyaltyService.ProcessOrderPointsAsync(createdOrder.Id, tenantId);
                }
                catch (Exception loyaltyEx)
                {
                    _logger.LogWarning(loyaltyEx, "Erro ao processar pontos de fidelidade para o pedido {OrderId}", createdOrder.Id);
                    // Não falhar o pedido se o programa de fidelidade falhar
                }
                
                // Enviar notificação de novo pedido para administradores
                try
                {
                    await _notificationService.NotifyNewOrderAsync(orderDto);
                }
                catch (Exception notificationEx)
                {
                    _logger.LogWarning(notificationEx, "Erro ao enviar notificação de novo pedido {OrderId}", createdOrder.Id);
                    // Não retornar erro para o cliente se apenas a notificação falhar, pois o pedido foi criado
                }

                return StaticResponseBuilder<OrderResponseDto>.BuildOk(orderDto);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar pedido");
            return StaticResponseBuilder<OrderResponseDto>.BuildError("Erro interno do servidor");
        }
    }
    /// <summary>
    /// Cria um novo pedido via canal público (ex: cardapio)
    /// </summary>
    /// <param name="createOrderRequestDto"></param>
    /// <param name="slug"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task<ResponseDTO<OrderResponseDto>> CreatePublicOrderAsync(CreatePublicOrderRequestDto requestDto, string slug)
    {
        try
        {
            var tenant = await _tenantRepository.GetBySlugAsync(slug);
            if (tenant == null)
                return StaticResponseBuilder<OrderResponseDto>.BuildError("Estabelecimento não encontrado");

            var (IsValid, ErrorMessage) = await ValidateOrderItemsAsync(requestDto.Items, tenant.Id);
            if (!IsValid)
                return StaticResponseBuilder<OrderResponseDto>.BuildError(ErrorMessage);

            // Verificar se o cliente existe ou criar novo
            var existingCustomer = await _customerRepository.GetByPhoneAsync(tenant.Id, requestDto.CustomerPhone);

            if (existingCustomer == null)
            {
                existingCustomer = await CreateCustomer(requestDto);
                await CreateTenantCustomer(tenant.Id, existingCustomer.Id);
            }

            var tenantCustomer = await _tenantCustomerRepository.GetByTenantIdAndCustomerIdAsync(tenant.Id, existingCustomer.Id);

            tenantCustomer ??= await CreateTenantCustomer(tenant.Id, existingCustomer.Id);

            var order = new OrderEntity
            {
                CustomerName = requestDto.CustomerName,
                CustomerPhone = requestDto.CustomerPhone,
                CustomerEmail = requestDto.CustomerEmail,
                DeliveryAddress = FormatDeliveryAddress(requestDto.DeliveryAddress),
                IsDelivery = requestDto.OrderType == EOrderType.Delivery,
                OrderType = requestDto.OrderType,
                TableId = requestDto.OrderType == EOrderType.Table ? requestDto.TableId : null,
                Notes = requestDto.Notes,
                Status = EOrderStatus.Pending,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                CustomerId = tenantCustomer.Customer.Id,
                TenantId = tenant.Id    
            };

            // Criar itens do pedido
            foreach (var itemDto in requestDto.Items)
            {
                var product = await _productRepository.GetByIdAsync(itemDto.ProductId, tenant.Id);
                if (product == null) continue;

                var orderItem = new OrderItemEntity
                {
                    ProductId = itemDto.ProductId,
                    ProductName = product.Name,
                    UnitPrice = product.Price,
                    Quantity = itemDto.Quantity,
                    Notes = itemDto.Notes,
                    Subtotal = product.Price * itemDto.Quantity
                };

                // Adicionar addons
                foreach (var addonDto in itemDto.Addons)
                {
                    var addon = await _addonRepository.GetByIdAsync(addonDto.AddonId, tenant.Id);
                    if (addon == null) continue;

                    var orderItemAddon = new OrderItemAddonEntity
                    {
                        AddonId = addonDto.AddonId,
                        AddonName = addon.Name,
                        UnitPrice = addon.Price,
                        Quantity = addonDto.Quantity,
                        Subtotal = addon.Price * addonDto.Quantity
                    };

                    orderItem.Addons.Add(orderItemAddon);
                    orderItem.Subtotal += orderItemAddon.Subtotal;
                }

                order.Items.Add(orderItem);
            }

            // Calcular totais
            order.Subtotal = order.Items.Sum(i => i.Subtotal);
            order.DeliveryFee = requestDto.DeliveryFee ?? 0.0m; // Taxa fixa de entrega

            // Processar cupom
            if (!string.IsNullOrEmpty(requestDto.CouponCode))
            {
                var coupon = await _couponRepository.GetByCodeAsync(requestDto.CouponCode, tenant.Id);
                if (coupon != null && coupon.IsActive)
                {
                    var now = DateTime.UtcNow;
                    bool isValid = true;

                    if (coupon.StartDate.HasValue && coupon.StartDate > now) isValid = false;
                    if (coupon.ExpirationDate.HasValue && coupon.ExpirationDate < now) isValid = false;
                    if (coupon.UsageLimit.HasValue && coupon.UsageCount >= coupon.UsageLimit) isValid = false;
                    if (coupon.MinOrderValue.HasValue && order.Subtotal < coupon.MinOrderValue) isValid = false;

                    if (isValid)
                    {
                        decimal discount = 0;
                        if (coupon.DiscountType == EDiscountType.Porcentagem)
                        {
                            discount = order.Subtotal * (coupon.DiscountValue / 100);
                            if (coupon.MaxDiscountValue.HasValue && discount > coupon.MaxDiscountValue.Value)
                                discount = coupon.MaxDiscountValue.Value;
                        }
                        else
                        {
                            discount = coupon.DiscountValue;
                        }

                        // Garantir que o desconto nÃ£o seja maior que o subtotal
                        if (discount > order.Subtotal) discount = order.Subtotal;

                        order.DiscountAmount = discount;
                        order.CouponCode = coupon.Code;

                        // Atualizar uso do cupom
                        coupon.UsageCount++;
                        await _couponRepository.UpdateAsync(coupon);
                    }
                }
            }

            order.Total = order.Subtotal + order.DeliveryFee - order.DiscountAmount;
            if (order.Total < 0) order.Total = 0;

            var createdOrder = await _orderRepository.AddAsync(order);
            var orderDto = _mapper.Map<OrderResponseDto>(createdOrder);

            // Processar pontos de fidelidade
            await _loyaltyService.ProcessOrderPointsAsync(createdOrder.Id, tenant.Id);

            try
            {
                await _notificationService.NotifyNewOrderAsync(orderDto);
            }
            catch (Exception notificationEx)
            {
                _logger.LogWarning(notificationEx, "Erro ao enviar notificação de novo pedido {OrderId}", createdOrder.Id);
            }
            return StaticResponseBuilder<OrderResponseDto>.BuildOk(orderDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar pedido");
            return StaticResponseBuilder<OrderResponseDto>.BuildError("Erro interno do servidor");
        }
    }
    /// <summary>
    /// Atualiza um pedido existente
    /// </summary>
    public async Task<ResponseDTO<OrderResponseDto>> UpdateOrderAsync(Guid id, UpdateOrderRequestDto requestDto)
    {
        try
        {
            var order = await _orderRepository.GetByIdAsync(id, _currentUserService.GetTenantGuid()!.Value);
            if (order == null)
                return StaticResponseBuilder<OrderResponseDto>.BuildNotFound(null!);

            if (order.Status != EOrderStatus.Pending)
                return StaticResponseBuilder<OrderResponseDto>.BuildError("Apenas pedidos pendentes podem ser atualizados");

            order = _mapper.Map(requestDto, order);

            await _orderRepository.UpdateAsync(order);

            var orderDto = _mapper.Map<OrderResponseDto>(order);
            _logger.LogInformation("Pedido {OrderId} atualizado com sucesso", id);
            return StaticResponseBuilder<OrderResponseDto>.BuildOk(orderDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar pedido {OrderId}", id);
            return StaticResponseBuilder<OrderResponseDto>.BuildError("Erro interno do servidor");
        }
    }
    /// <summary>
    /// Exclui um pedido (soft delete)
    /// </summary>
    public async Task<ResponseDTO<bool>> DeleteOrderAsync(Guid id)
    {
        try
        {
            var order = await _orderRepository.GetByIdAsync(id, _currentUserService.GetTenantGuid()!.Value);
            if (order == null)
                return StaticResponseBuilder<bool>.BuildNotFound(false);

            if (order.Status != EOrderStatus.Pending && order.Status != EOrderStatus.Cancelled)
                return StaticResponseBuilder<bool>.BuildError("Apenas pedidos pendentes ou cancelados podem ser excluí­dos");

            await _orderRepository.DeleteVirtualAsync(id, _currentUserService.GetTenantGuid()!.Value);

            _logger.LogInformation("Pedido {OrderId} exclusão com sucesso", id);
            return StaticResponseBuilder<bool>.BuildOk(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao excluir pedido {OrderId}", id);
            return StaticResponseBuilder<bool>.BuildError("Erro interno do servidor");
        }
    }
    /// <summary>
    /// Atualiza o status de um pedido
    /// </summary>
    public async Task<ResponseDTO<OrderResponseDto>> UpdatEOrderStatusAsync(Guid id, UpdatEOrderStatusRequestDto requestDto)
    {
        try
        {
            var userId = _currentUserService!.UserId;

            var order = await _orderRepository.GetByIdWithIncludesAsync(id, o => o.StatusHistory);
            if (order == null)
                return StaticResponseBuilder<OrderResponseDto>.BuildNotFound(null!);

            // Validar transição de status
            if (!IsValidStatusTransition(order.Status, requestDto.Status))
                return StaticResponseBuilder< OrderResponseDto>.BuildError($"Transição de status inválida de {order.Status} para {requestDto.Status}");

            var previousStatus = order.Status;
            order.Status = requestDto.Status;
            order.UpdatedAt = DateTime.UtcNow;

            // Adicionar histórico de status
            var statusHistory = new OrderStatusHistoryEntity
            {
                OrderId = order.Id,
                Status = requestDto.Status,
                Timestamp = DateTime.UtcNow,
                Notes = requestDto.Notes,
                UserId = Guid.Parse(userId)
            };

            order.StatusHistory.Add(statusHistory);
            await _orderRepository.UpdateAsync(order);

            var orderDto = _mapper.Map<OrderResponseDto>(order);

            _logger.LogInformation("Status do pedido {OrderId} alterado de {PreviousStatus} para {NewStatus}", 
                id, previousStatus, requestDto.Status);
            
            try
            {
                await _notificationService.NotifyEOrderStatusChangedAsync(id, previousStatus, requestDto.Status, requestDto.Notes);
                
                switch (requestDto.Status)
                {
                    case EOrderStatus.Ready:
                        await _notificationService.NotifyOrderReadyAsync(id);
                        break;
                    case EOrderStatus.Delivered:
                        await _notificationService.NotifyOrderCompletedAsync(id);
                        break;
                }
            }
            catch (Exception notificationEx)
            {
                _logger.LogWarning(notificationEx, "Erro ao enviar notificação de mudança de status do pedido {OrderId}", id);
            }

            return StaticResponseBuilder<OrderResponseDto>.BuildOk(orderDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar status do pedido {OrderId}", id);
            return StaticResponseBuilder<OrderResponseDto>.BuildError("Erro interno do servidor");
        }
    }
    /// <summary>
    /// ObtÃ©m pedidos por status
    /// </summary>
    public async Task<ResponseDTO<IEnumerable<OrderResponseDto>>> GetOrdersByStatusAsync(EOrderStatus status)
    {
        try
        {
            var orders = await _orderRepository.GetOrdersByStatusAsync(status);
            var orderDtos = _mapper.Map<IEnumerable<OrderResponseDto>>(orders);
            return StaticResponseBuilder<IEnumerable<OrderResponseDto>>.BuildOk(orderDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter pedidos por status {Status}", status);
            return StaticResponseBuilder<IEnumerable<OrderResponseDto>>.BuildError("Erro interno do servidor");
        }
    }
    /// <summary>
    /// ObtÃ©m pedidos por telefone do cliente
    /// </summary>
    public async Task<ResponseDTO<IEnumerable<OrderResponseDto>>> GetOrdersByCustomerAsync(string customerPhone)
    {
        try
        {
            var orders = await _orderRepository.FindAsync(o => o.CustomerPhone == customerPhone);
            var orderDtos = _mapper.Map<IEnumerable<OrderResponseDto>>(orders);
            return StaticResponseBuilder<IEnumerable<OrderResponseDto>>.BuildOk(orderDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter pedidos do cliente {CustomerPhone}", customerPhone);
            return StaticResponseBuilder<IEnumerable<OrderResponseDto>>.BuildError("Erro interno do servidor");
        }
    }
    /// <summary>
    /// Aceita um pedido
    /// </summary>
    public async Task<ResponseDTO<OrderResponseDto>> AcceptOrderAsync(Guid id, int estimatedPreparationMinutes, string? notes = null)
    {
        try
        {
            var order = await _orderRepository.GetByIdAsync(id, _currentUserService.GetTenantGuid()!.Value);
            if (order == null)
                return StaticResponseBuilder<OrderResponseDto>.BuildNotFound(null!);

            if (order.Status != EOrderStatus.Pending)
                return StaticResponseBuilder<OrderResponseDto>.BuildError("Apenas pedidos pendentes podem ser aceitos");

            order.Status = EOrderStatus.Confirmed;
            order.EstimatedPreparationMinutes = estimatedPreparationMinutes;
            order.EstimatedDeliveryTime = DateTime.UtcNow.AddMinutes(estimatedPreparationMinutes + (order.IsDelivery ? 30 : 0));
            order.UpdatedAt = DateTime.UtcNow;

            if (!string.IsNullOrEmpty(notes))
                order.Notes = notes;

            await _orderRepository.UpdateAsync(order);

            var orderDto = _mapper.Map<OrderResponseDto>(order);
            _logger.LogInformation("Pedido {OrderId} aceito com tempo estimado de {EstimatedMinutes} minutos", 
                id, estimatedPreparationMinutes);
            
            // Enviar notificaÃ§Ã£o de pedido aceito
            try
            {
                await _notificationService.NotifyOrderAcceptedAsync(orderDto);
            }
            catch (Exception notificationEx)
            {
                _logger.LogWarning(notificationEx, "Erro ao enviar notificação de pedido aceito {OrderId}", id);
            }
            return StaticResponseBuilder<OrderResponseDto>.BuildOk(orderDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao aceitar pedido {OrderId}", id);
            return StaticResponseBuilder<OrderResponseDto>.BuildError("Erro interno do servidor");
        }
    }
    /// <summary>
    /// Rejeita um pedido
    /// </summary>
    public async Task<ResponseDTO<OrderResponseDto>> RejectOrderAsync(Guid id, string reason, string? notes = null, string? rejectedBy = null)
    {
        try
        {
            var order = await _orderRepository.GetByIdWithIncludesAsync(id, o => o.Rejection!);
            if (order == null)
                return StaticResponseBuilder<OrderResponseDto>.BuildNotFound(null!);
            if (order.Status != EOrderStatus.Pending && order.Status != EOrderStatus.Confirmed)
                return StaticResponseBuilder<OrderResponseDto>.BuildError("Apenas pedidos pendentes ou confirmados podem ser rejeitados");

            order.Status = EOrderStatus.Rejected;
            order.UpdatedAt = DateTime.UtcNow;

            var rejection = new OrderRejectionEntity
            {
                OrderId = order.Id,
                Reason = reason,
                Notes = notes,
                RejectedAt = DateTime.UtcNow,
                RejectedBy = rejectedBy ?? "system"
            };

            order.Rejection = rejection;
            await _orderRepository.UpdateAsync(order);

            var orderDto = _mapper.Map<OrderResponseDto>(order);
            _logger.LogInformation("Pedido {OrderId} rejeitado. Motivo: {Reason}", id, reason);
            
            try
            {
                await _notificationService.NotifyOrderRejectedAsync(orderDto, reason);
            }
            catch (Exception notificationEx)
            {
                _logger.LogWarning(notificationEx, "Erro ao enviar notificação de pedido rejeitado {OrderId}", id);
            }
            return StaticResponseBuilder<OrderResponseDto>.BuildOk(orderDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao rejeitar pedido {OrderId}", id);
            return StaticResponseBuilder<OrderResponseDto>.BuildError("Erro interno do servidor");
        }
    }
    public async Task<PagedResponseDTO<OrderResponseDto>> GetOrdersPagedAsync(int pageNumber, int pageSize)
    {
        try
        {
            var tenantId = _currentUserService.GetTenantGuid()!.Value;
            var orders = await _orderRepository.GetPagedByTenantIdWithDetailsAsync(tenantId, pageNumber, pageSize);

            var totalItems = await _orderRepository.CountByTenantIdAsync(tenantId);

            var orderDtos = _mapper.Map<List<OrderResponseDto>>(orders);
            return StaticResponseBuilder<OrderResponseDto>.BuildPagedOk(orderDtos, totalItems, pageNumber, pageSize);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter pedidos paginados");
            return new PagedResponseDTO<OrderResponseDto>
            {
                Succeeded = false,
                Code = 500,
                Errors = new List<ErrorDTO> { new ErrorDTO { Message = "Erro interno do servidor", Code = "ERROR" } }
            };
        }
    }
    /// <summary>
    /// ObtÃ©m o pedido ativo de uma mesa
    /// </summary>
    public async Task<ResponseDTO<OrderResponseDto?>> GetActiveOrderByTableIdAsync(Guid tableId)
    {
        try
        {
            var tenantId = _currentUserService.GetTenantGuid()!.Value;

            var orders = await _orderRepository.FindOrderedAsync(
                o => o.TenantId == tenantId && o.TableId == tableId &&
                     o.Status != EOrderStatus.Cancelled && o.Status != EOrderStatus.Rejected &&
                     o.CreatedAt >= DateTime.UtcNow.Date,
                o => o.CreatedAt,
                false
            );

            var activeOrder = orders.FirstOrDefault();

            if (activeOrder == null)
                return StaticResponseBuilder<OrderResponseDto?>.BuildOk(null); // No active order

            var orderWithDetails = await _orderRepository.GetByIdWithIncludesAsync(activeOrder.Id, o => o.Items);

            var dto = _mapper.Map<OrderResponseDto>(orderWithDetails);
            return StaticResponseBuilder<OrderResponseDto?>.BuildOk(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter pedido ativo da mesa {TableId}", tableId);
            return StaticResponseBuilder<OrderResponseDto?>.BuildError("Erro ao obter pedido da mesa");
        }
    }
    /// <summary>
    /// Adiciona itens a um pedido existente
    /// </summary>
    public async Task<ResponseDTO<OrderResponseDto>> AddItemsToOrderAsync(Guid orderId, List<CreateOrderItemRequestDto> items)
    {
        try
        {
            var tenantId = _currentUserService.GetTenantGuid()!.Value;
            // Need to include Items to calculate totals correctly
            var order = await _orderRepository.GetByIdWithIncludesAsync(orderId, o => o.Items);

            if (order == null)
                return StaticResponseBuilder<OrderResponseDto>.BuildNotFound(null);

            if (order.TenantId != tenantId)
                return StaticResponseBuilder<OrderResponseDto>.BuildError("Pedido não pertence ao tenant");

            if (order.Status == EOrderStatus.Cancelled || order.Status == EOrderStatus.Rejected)
                return StaticResponseBuilder<OrderResponseDto>.BuildError("Não é possível adicionar itens a um pedido cancelado ou rejeitado");

            // Add items logic
            foreach (var itemDto in items)
            {
                var product = await _productRepository.GetByIdAsync(itemDto.ProductId, tenantId);
                if (product == null) continue;

                var orderItem = new OrderItemEntity
                {
                    ProductId = itemDto.ProductId,
                    ProductName = product.Name,
                    UnitPrice = product.Price,
                    Quantity = itemDto.Quantity,
                    Notes = itemDto.Notes,
                    Subtotal = product.Price * itemDto.Quantity,
                    OrderId = order.Id // Ensure link
                };

                // Adicionar addons
                foreach (var addonDto in itemDto.Addons)
                {
                    var addon = await _addonRepository.GetByIdAsync(addonDto.AddonId, tenantId);
                    if (addon == null) continue;

                    var orderItemAddon = new OrderItemAddonEntity
                    {
                        AddonId = addonDto.AddonId,
                        AddonName = addon.Name,
                        UnitPrice = addon.Price,
                        Quantity = addonDto.Quantity,
                        Subtotal = addon.Price * addonDto.Quantity
                    };

                    orderItem.Addons.Add(orderItemAddon);
                    orderItem.Subtotal += orderItemAddon.Subtotal;
                }

                order.Items.Add(orderItem);
            }

            // Recalculate totals
            order.Subtotal = order.Items.Sum(i => i.Subtotal);
            order.Total = order.Subtotal + order.DeliveryFee - order.DiscountAmount;
            order.UpdatedAt = DateTime.UtcNow;

            await _orderRepository.UpdateAsync(order);

            var dto = _mapper.Map<OrderResponseDto>(order);
            return StaticResponseBuilder<OrderResponseDto>.BuildOk(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao adicionar itens ao pedido {OrderId}", orderId);
            return StaticResponseBuilder<OrderResponseDto>.BuildError("Erro ao adicionar itens ao pedido");
        }
    }
    /// <summary>
    /// Fecha a conta da mesa
    /// </summary>
    public async Task<ResponseDTO<OrderResponseDto>> CloseTableAccountAsync(Guid tableId)
    {
        var result = await GetActiveOrderByTableIdAsync(tableId);
        if (!result.Succeeded || result.Data == null)
            return StaticResponseBuilder<OrderResponseDto>.BuildError("Nenhum pedido ativo para esta mesa");

        return StaticResponseBuilder<OrderResponseDto>.BuildOk(result.Data);
    }
    public async Task<ResponseDTO<OrderResponseDto>> CancelOrderAsync(Guid id, CancelOrderRequestDto requestDto)
    {
        try
        {
            var userId = _currentUserService!.UserId;

            var order = await _orderRepository.GetByIdWithIncludesAsync(id, o => o.StatusHistory, o => o.Customer);
            if (order == null)
                return StaticResponseBuilder<OrderResponseDto>.BuildNotFound(null!);

            if (order.Status != EOrderStatus.Pending)
                return StaticResponseBuilder<OrderResponseDto>.BuildError("Apenas pedidos pendentes podem ser cancelados pelo cliente.");

            var previousStatus = order.Status;
            order.Status = EOrderStatus.Cancelled;
            order.UpdatedAt = DateTime.UtcNow;

            var statusHistory = new OrderStatusHistoryEntity
            {
                OrderId = order.Id,
                Status = EOrderStatus.Cancelled,
                Timestamp = DateTime.UtcNow,
                Notes = $"Cancelado pelo cliente. Motivo: {requestDto.Reason}",
                UserId = !string.IsNullOrEmpty(userId) ? Guid.Parse(userId) : order.Customer.Id,
            };

            order.StatusHistory.Add(statusHistory);
            await _orderRepository.UpdateAsync(order);

            var orderDto = _mapper.Map<OrderResponseDto>(order);

            // Notificar cancelamento
            await _notificationService.NotifyEOrderStatusChangedAsync(id, previousStatus, EOrderStatus.Cancelled, requestDto.Reason);

            return StaticResponseBuilder<OrderResponseDto>.BuildOk(orderDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao cancelar pedido {OrderId}", id);
            return StaticResponseBuilder<OrderResponseDto>.BuildError("Erro interno ao cancelar pedido");
        }
    }
    public async Task<ResponseDTO<OrderResponseDto>> UpdateOrderPaymentMethodAsync(Guid id, UpdateOrderPaymentRequestDto requestDto)
    {
        try
        {
            var order = await _orderRepository.GetByIdWithIncludesAsync(id, o => o.Payments);
            if (order == null)
                return StaticResponseBuilder<OrderResponseDto>.BuildNotFound(null!);

            if (order.Status != EOrderStatus.Pending)
                return StaticResponseBuilder<OrderResponseDto>.BuildError("Apenas pedidos pendentes podem ser alterados.");

            var currentPayment = order.Payments.OrderByDescending(p => p.CreatedAt).FirstOrDefault(p => p.Status == EPaymentStatus.Pending);

            if (currentPayment != null)
            {
                currentPayment.Method = requestDto.PaymentMethod;
                currentPayment.UpdatedAt = DateTime.UtcNow;
            }
            else
            {
                // Se não houver pagamento pendente, cria um novo
                var newPayment = new PaymentEntity
                {
                    OrderId = order.Id,
                    Amount = order.Total,
                    Method = requestDto.PaymentMethod,
                    Status = EPaymentStatus.Pending,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                order.Payments.Add(newPayment);
            }

            order.UpdatedAt = DateTime.UtcNow;
            await _orderRepository.UpdateAsync(order);

            // Recarregar com todos os includes para retorno completo
            var updatedOrder = await _orderRepository.GetByIdWithIncludesAsync(id, o => o.Items);
            var orderDto = _mapper.Map<OrderResponseDto>(updatedOrder);
            return StaticResponseBuilder<OrderResponseDto>.BuildOk(orderDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar mÃ©todo de pagamento do pedido {OrderId}", id);
            return StaticResponseBuilder<OrderResponseDto>.BuildError("Erro interno ao atualizar pagamento");
        }
    }
    public async Task<ResponseDTO<OrderResponseDto>> UpdateOrderDeliveryTypeAsync(Guid id, UpdateOrderDeliveryTypeRequestDto requestDto)
    {
        try
        {
            var order = await _orderRepository.GetByIdWithIncludesAsync(id, o => o.Items);
            if (order == null)
                return StaticResponseBuilder<OrderResponseDto>.BuildNotFound(null!);

            if (order.Status != EOrderStatus.Pending)
                return StaticResponseBuilder<OrderResponseDto>.BuildError("Apenas pedidos pendentes podem ser alterados.");

            // Se nada mudou, retorna ok
            if (order.IsDelivery == requestDto.IsDelivery && (!requestDto.IsDelivery || order.DeliveryAddress == requestDto.DeliveryAddress))
                return StaticResponseBuilder<OrderResponseDto>.BuildOk(_mapper.Map<OrderResponseDto>(order));

            order.IsDelivery = requestDto.IsDelivery;

            if (requestDto.IsDelivery)
            {
                if (string.IsNullOrWhiteSpace(requestDto.DeliveryAddress))
                    return StaticResponseBuilder<OrderResponseDto>.BuildError("EndereÃ§o Ã© obrigatÃ³rio para entrega.");

                order.DeliveryAddress = requestDto.DeliveryAddress;

                // Recalcular taxa de entrega (Simplificação: manter a taxa original se já¡ era entrega, ou buscar regra de taxa)

                // Como não tenho a regra de taxa aqui fácil, vou assumir uma regra simples ou manter a taxa se já¡ existir.
                // Se estava Retirada (Fee 0) e virou Entrega, precisa de taxa.
                // Vou assumir taxa fixa do tenant ou zero por enquanto se nÃ£o tiver lÃ³gica complexa, 
                // MAS o ideal Ã© recalcular.
                // TODO: Melhorar cÃ¡lculo de taxa. Por hora, se virou entrega e taxa era 0, mantÃ©m 0 ou usa um padrÃ£o?
                // Vou manter a taxa anterior se for > 0, senÃ£o 0 (risco de prejuÃ­zo, mas seguro pra MVP).
                // CORREÃ‡ÃƒO: Se o usuÃ¡rio mudar pra entrega, ele espera pagar a taxa.
                // Vou verificar se consigo pegar a taxa do tenant.

                // Se nÃ£o tem taxa definida, define uma padrÃ£o ou busca do tenant settings (nÃ£o tenho acesso fÃ¡cil aqui sem injetar mais coisas).
                // Vou deixar a taxa como estÃ¡ se for > 0. Se for 0 e virar entrega, continua 0 (promoÃ§Ã£o? erro?).
                // Melhor: Se virar retirada, zera a taxa.
            }
            else
            {
                order.DeliveryFee = 0.0m;
            }

            // Recalcular total
            order.Subtotal = order.Items.Sum(i => i.Subtotal);
            order.Total = order.Subtotal + order.DeliveryFee - order.DiscountAmount;

            order.UpdatedAt = DateTime.UtcNow;
            await _orderRepository.UpdateAsync(order);

            return StaticResponseBuilder<OrderResponseDto>.BuildOk(_mapper.Map<OrderResponseDto>(order));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar tipo de entrega do pedido {OrderId}", id);
            return StaticResponseBuilder<OrderResponseDto>.BuildError("Erro interno ao atualizar entrega");
        }
    }
    public async Task<ResponseDTO<OrderResponseDto>> CreateOrderPickupAsync(CreateOrderRequestDto requestDto)
    {
        throw new NotImplementedException();
    }
    public async Task<ResponseDTO<OrderResponseDto>> CreateOrderDineInAsync(CreateOrderRequestDto requestDto)
    {
        throw new NotImplementedException();
    }
    #region Private Methods
    private static string FormatDeliveryAddress(AddressDto? address)
    {
        if (address == null) return string.Empty;
        
        var formatted = $"{address.Street}, {address.Number}";
        
        if (!string.IsNullOrWhiteSpace(address.Complement))
        {
            formatted += $" - {address.Complement}";
        }
        
        formatted += $" - {address.Neighborhood}";
        formatted += $" - {address.City}/{address.State}";
        formatted += $" - CEP: {address.ZipCode}";
        
        return formatted;
    }
    /// <summary>
    /// Valida os itens do pedido
    /// </summary>
    private async Task<(bool IsValid, string ErrorMessage)> ValidateOrderItemsAsync(List<CreateOrderItemRequestDto> items, Guid tenantId)
    {
        if (items.Count == 0)
            return (false, "O pedido deve conter pelo menos um item");

        foreach (var item in items)
        {
            var product = await _productRepository.GetByIdAsync(item.ProductId, tenantId);
            if (product == null)
                return (false, $"Produto com ID {item.ProductId} nÃ£o encontrado");

            if (!product.IsActive)
                return (false, $"Produto '{product.Name}' nÃ£o estÃ¡ ativo");

            if (item.Quantity <= 0)
                return (false, "Quantidade deve ser maior que zero");

            foreach (var addon in item.Addons)
            {
                var addonEntity = await _addonRepository.GetByIdAsync(addon.AddonId, tenantId);
                if (addonEntity == null)
                    return (false, $"Adicional com ID {addon.AddonId} nÃ£o encontrado");

                if (!addonEntity.IsActive)
                    return (false, $"Adicional '{addonEntity.Name}' nÃ£o estÃ¡ ativo");

                if (addon.Quantity <= 0)
                    return (false, "Quantidade do adicional deve ser maior que zero");
            }
        }

        return (true, string.Empty);
    }
    /// <summary>
    /// Valida se a transição de status é válida
    /// </summary>
    private static bool IsValidStatusTransition(EOrderStatus currentStatus, EOrderStatus newStatus)
    {
        return currentStatus switch
        {
            EOrderStatus.Pending => newStatus is EOrderStatus.Confirmed or EOrderStatus.Cancelled or EOrderStatus.Rejected,
            EOrderStatus.Confirmed => newStatus is EOrderStatus.Preparing or EOrderStatus.Pending or EOrderStatus.Cancelled or EOrderStatus.Rejected,
            EOrderStatus.Preparing => newStatus is EOrderStatus.Ready or EOrderStatus.Confirmed or EOrderStatus.Cancelled,
            EOrderStatus.Ready => newStatus is EOrderStatus.Preparing or EOrderStatus.OutForDelivery or EOrderStatus.Delivered or EOrderStatus.Cancelled,
            EOrderStatus.OutForDelivery => newStatus is EOrderStatus.Delivered or EOrderStatus.Cancelled,
            EOrderStatus.Delivered => newStatus is EOrderStatus.Ready, // Status final
            EOrderStatus.Cancelled => false, // Status final
            EOrderStatus.Rejected => false, // Status final
            _ => false
        };
    }
    private async Task<CustomerEntity> CreateCustomer(CreatePublicOrderRequestDto requestDto)
    {
        try
        {
            var customerEntity = new CustomerEntity
            {
                Id = Guid.NewGuid(),
                Name = requestDto.CustomerName,
                Phone = requestDto.CustomerPhone,
                Email = requestDto.CustomerEmail,
                PostalCode = requestDto.DeliveryAddress?.ZipCode,
                Street = requestDto.DeliveryAddress?.Street,
                StreetNumber = requestDto.DeliveryAddress?.Number,
                Neighborhood = requestDto.DeliveryAddress?.Neighborhood,
                City = requestDto.DeliveryAddress?.City,
                State = requestDto.DeliveryAddress?.State,
                Complement = requestDto.DeliveryAddress?.Complement,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            return await _customerRepository.CreateAsync(customerEntity);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar cliente");
            throw;
        }
        
    }
    private async Task<CustomerEntity> CreateCustomer(CreateOrderRequestDto requestDto)
    {
        try
        {
            var customerEntity = new CustomerEntity
            {
                Id = Guid.NewGuid(),
                Name = requestDto.CustomerName,
                Phone = requestDto.CustomerPhone,
                Email = requestDto.CustomerEmail,
                PostalCode = requestDto.DeliveryAddress?.ZipCode,
                Street = requestDto.DeliveryAddress?.Street,
                StreetNumber = requestDto.DeliveryAddress?.Number,
                Neighborhood = requestDto.DeliveryAddress?.Neighborhood,
                City = requestDto.DeliveryAddress?.City,
                State = requestDto.DeliveryAddress?.State,
                Complement = requestDto.DeliveryAddress?.Complement,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            return await _customerRepository.CreateAsync(customerEntity);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar cliente para pedido pÃºblico");
            throw ex;
        }

    }
    private async Task<TenantCustomerEntity> CreateTenantCustomer(Guid tenantId, Guid customerId)
    {
        try
        {
            var tenantEntity = new TenantCustomerEntity
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                CustomerId = customerId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            return await _tenantCustomerRepository.CreateAsync(tenantEntity);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar TenantCustomer para TenantId {TenantId} e CustomerId {CustomerId}", tenantId, customerId);
            throw ex;
        } 
        
    }
    #endregion
}

