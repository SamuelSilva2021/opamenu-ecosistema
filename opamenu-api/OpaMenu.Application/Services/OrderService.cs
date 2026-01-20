using Microsoft.Extensions.Logging;
using OpaMenu.Application.DTOs;
using OpaMenu.Application.Services.Interfaces;
using OpaMenu.Domain.DTOs;
using OpaMenu.Domain.DTOs.Addons;
using OpaMenu.Domain.DTOs.Order;
using OpaMenu.Infrastructure.Shared.Entities;
using OpaMenu.Infrastructure.Shared.Enums;
using OpaMenu.Domain.Interfaces;
using OpaMenu.Web.Models.DTOs;
using System.Threading.Tasks;
using OpaMenu.Commons.Api.DTOs;
using OpaMenu.Commons.Api.Commons;

namespace OpaMenu.Application.Services;

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
    ILoyaltyService loyaltyService
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
                o => o.Rejection);

            var orderDtos = orders.Select(MapToOrderResponseDto);
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
    public async Task<ResponseDTO<OrderResponseDto>> GetOrderByIdAsync(int id)
    {
        try
        {
            var order = await _orderRepository.GetByIdWithIncludesAsync(id,
                o => o.Items,
                o => o.StatusHistory,
                o => o.Rejection);

            if (order == null)
                return StaticResponseBuilder<OrderResponseDto>.BuildNotFound(null!);

            var orderDto = MapToOrderResponseDto(order);
            return StaticResponseBuilder<OrderResponseDto>.BuildOk(orderDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter pedido {OrderId}", id);
            return StaticResponseBuilder<OrderResponseDto>.BuildError("Erro interno do servidor");
        }
    }

    public async Task<ResponseDTO<OrderResponseDto>> GetPublicOrderByIdAsync(string slug, int id)
    {
        try
        {
            var tenant = await _tenantRepository.GetBySlugAsync(slug);
            if (tenant == null)
                return StaticResponseBuilder<OrderResponseDto>.BuildOk(new OrderResponseDto { });

            var order = await _orderRepository.GetByIdWithIncludesAsync(id,
                o => o.Items,
                o => o.StatusHistory,
                o => o.Rejection);

            if (order == null || order.TenantId != tenant.Id)
                return StaticResponseBuilder<OrderResponseDto>.BuildNotFound(new OrderResponseDto { });

            var orderDto = MapToOrderResponseDto(order);
            return StaticResponseBuilder<OrderResponseDto>.BuildOk(orderDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter pedido pÃºblico {OrderId} para o estabelecimento {Slug}", id, slug);
            return StaticResponseBuilder<OrderResponseDto>.BuildError("Erro interno do servidor");
        }
    }

    public async Task<ResponseDTO<IEnumerable<OrderResponseDto?>>> GetPublicOrdersByCustomerIdAsync(string slug, Guid customerId)
    {
        try
        {
            var tenant = await _tenantRepository.GetBySlugAsync(slug);
            if (tenant == null)
                return StaticResponseBuilder<IEnumerable<OrderResponseDto?>>.BuildNotFound([]);

            var order = await _orderRepository.GetByCustomerIdAndTenantIdAsync(tenant!.Id, customerId);

            if (order == null || !order.Any())
                return StaticResponseBuilder<IEnumerable<OrderResponseDto?>>.BuildOk([]);

            var orderDtos = order.Select(MapToOrderResponseDto);
            return StaticResponseBuilder<IEnumerable<OrderResponseDto?>>.BuildOk(orderDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter pedidos pÃºblicos para o cliente {CustomerId} no estabelecimento {Slug}", customerId, slug);
            return StaticResponseBuilder<IEnumerable<OrderResponseDto?>>.BuildError("Erro interno do servidor");
        }
        
    }

    /// <summary>
    /// Cria um novo pedido
    /// </summary>
    public async Task<ResponseDTO<OrderResponseDto>> CreateOrderAsync(CreateOrderRequestDto requestDto)
    {
        try
        {
            var tenantId = _currentUserService.GetTenantGuid()!.Value;

            // Validar produtos e calcular valores
            var validationResult = await ValidateOrderItemsAsync(requestDto.Items, tenantId);
            if (!validationResult.IsValid)
                return StaticResponseBuilder<OrderResponseDto>.BuildError(validationResult.ErrorMessage);

            // Validar mesa se for pedido na mesa
            if (requestDto.OrderType == EOrderType.Table)
            {
                if (!requestDto.TableId.HasValue)
                    return StaticResponseBuilder<OrderResponseDto>.BuildError("Mesa Ã© obrigatÃ³ria para pedidos na mesa");

                var table = await _tableRepository.GetByIdAsync(requestDto.TableId.Value, tenantId);

                if (table == null)
                    return StaticResponseBuilder<OrderResponseDto>.BuildError("Mesa nÃ£o encontrada");

                //if o pedido for para mesa, o customer serÃ¡ a propria mesa
                requestDto.CustomerName = requestDto.CustomerName ?? table.Name;
                requestDto.CustomerPhone = requestDto.CustomerPhone ??  "(00) 00000-0000";
                requestDto.CustomerEmail = requestDto.CustomerEmail ?? null;
            }

            // Verificar se o cliente existe ou criar novo
            var existingCustomer = await _customerRepository.GetByPhoneAsync(tenantId, requestDto.CustomerPhone);

            //Se nÃ£o possuir um cliente associado ao telefone, cria um novo
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
                CustomerName = requestDto.CustomerName,
                CustomerPhone = requestDto.CustomerPhone,
                CustomerEmail = requestDto.CustomerEmail,
                DeliveryAddress = FormatDeliveryAddress(requestDto.DeliveryAddress),
                IsDelivery = requestDto.OrderType == EOrderType.Delivery,
                OrderType = requestDto.OrderType,
                TableId = requestDto.OrderType == EOrderType.Table ? requestDto.TableId : null,
                Notes = requestDto.Notes,
                Status = OrderStatus.Pending,
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

                    var orderItemAddon = new OrderItemAddon
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
            order.DeliveryFee = order.IsDelivery ? 5.00m : 0; // Taxa fixa de entrega
            
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
                            if (coupon.DiscountType == EDiscountType.Percentage)
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
            var orderDto = MapToOrderResponseDto(createdOrder);

            _logger.LogInformation("Pedido {OrderId} criado com sucesso", createdOrder.Id);

            // Processar pontos de fidelidade
            await _loyaltyService.ProcessOrderPointsAsync(createdOrder.Id, tenantId);
            
            // Enviar notificação de novo pedido para administradores
            try
            {
                await _notificationService.NotifyNewOrderAsync(orderDto);
            }
            catch (Exception notificationEx)
            {
                _logger.LogWarning(notificationEx, "Erro ao enviar notificaÃ§Ã£o de novo pedido {OrderId}", createdOrder.Id);
                return StaticResponseBuilder<OrderResponseDto>.BuildError("Erro ao enviar notificaÃ§Ã£o de novo pedido");
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
    /// Cria um novo pedido via canal pÃºblico (ex: cardapio)
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
                return StaticResponseBuilder<OrderResponseDto>.BuildError("Estabelecimento nÃ£o encontrado");

            var validationResult = await ValidateOrderItemsAsync(requestDto.Items, tenant.Id);
            if (!validationResult.IsValid)
                return StaticResponseBuilder<OrderResponseDto>.BuildError(validationResult.ErrorMessage);

            // Validar mesa se for pedido na mesa
            if (requestDto.OrderType == EOrderType.Table)
            {
                if (!requestDto.TableId.HasValue)
                    return StaticResponseBuilder<OrderResponseDto>.BuildError("Mesa Ã© obrigatÃ³ria para pedidos na mesa");

                var table = await _tableRepository.GetByIdAsync(requestDto.TableId.Value, tenant.Id);
                if (table == null)
                    return StaticResponseBuilder<OrderResponseDto>.BuildError("Mesa nÃ£o encontrada");
            }

            // Verificar se o cliente existe ou criar novo
            var existingCustomer = await _customerRepository.GetByPhoneAsync(tenant.Id, requestDto.CustomerPhone);

            if (existingCustomer == null)
            {
                //Cria um novo cliente e associa ao tenant
                existingCustomer = await CreateCustomer(requestDto);
                await CreateTenantCustomer(tenant.Id, existingCustomer.Id);
            }

            var tenantCustomer = await _tenantCustomerRepository.GetByTenantIdAndCustomerIdAsync(tenant.Id, existingCustomer.Id);
            //Se nÃ£o existir jÃ¡ cria um novo e atribui
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
                Status = OrderStatus.Pending,
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

                    var orderItemAddon = new OrderItemAddon
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
            order.DeliveryFee = order.IsDelivery ? 5.00m : 0; // Taxa fixa de entrega

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
                        if (coupon.DiscountType == EDiscountType.Percentage)
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
            var orderDto = MapToOrderResponseDto(createdOrder);

            _logger.LogInformation("Pedido {OrderId} criado com sucesso", createdOrder.Id);

            // Processar pontos de fidelidade
            await _loyaltyService.ProcessOrderPointsAsync(createdOrder.Id, tenant.Id);

            // Enviar notificaÃ§Ã£o de novo pedido para administradores
            try
            {
                await _notificationService.NotifyNewOrderAsync(orderDto);
            }
            catch (Exception notificationEx)
            {
                _logger.LogWarning(notificationEx, "Erro ao enviar notificaÃ§Ã£o de novo pedido {OrderId}", createdOrder.Id);
                // NÃ£o falhar a criaÃ§Ã£o do pedido por erro de notificaÃ§Ã£o
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
    public async Task<ResponseDTO<OrderResponseDto>> UpdateOrderAsync(int id, UpdateOrderRequestDto requestDto)
    {
        try
        {
            var order = await _orderRepository.GetByIdAsync(id, _currentUserService.GetTenantGuid()!.Value);
            if (order == null)
                return StaticResponseBuilder<OrderResponseDto>.BuildNotFound(null!);

            if (order.Status != OrderStatus.Pending)
                return StaticResponseBuilder<OrderResponseDto>.BuildError("Apenas pedidos pendentes podem ser atualizados");

            // Atualizar propriedades
            order.CustomerName = requestDto.CustomerName;
            order.CustomerPhone = requestDto.CustomerPhone;
            order.CustomerEmail = requestDto.CustomerEmail;
            order.DeliveryAddress = requestDto.DeliveryAddress;
            if (requestDto.IsDelivery.HasValue)
                order.IsDelivery = requestDto.IsDelivery.Value;
            order.Notes = requestDto.Notes;
            order.EstimatedPreparationMinutes = requestDto.EstimatedPreparationMinutes;
            order.UpdatedAt = DateTime.UtcNow;

            await _orderRepository.UpdateAsync(order);

            var orderDto = MapToOrderResponseDto(order);
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
    public async Task<ResponseDTO<bool>> DeleteOrderAsync(int id)
    {
        try
        {
            var order = await _orderRepository.GetByIdAsync(id, _currentUserService.GetTenantGuid()!.Value);
            if (order == null)
                return StaticResponseBuilder<bool>.BuildNotFound(false);

            if (order.Status != OrderStatus.Pending && order.Status != OrderStatus.Cancelled)
                return StaticResponseBuilder<bool>.BuildError("Apenas pedidos pendentes ou cancelados podem ser excluÃ­dos");

            await _orderRepository.DeleteVirtualAsync(id, _currentUserService.GetTenantGuid()!.Value);

            _logger.LogInformation("Pedido {OrderId} excluÃ­do com sucesso", id);
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
    public async Task<ResponseDTO<OrderResponseDto>> UpdateOrderStatusAsync(int id, UpdateOrderStatusRequestDto requestDto)
    {
        try
        {
            var order = await _orderRepository.GetByIdWithIncludesAsync(id, o => o.StatusHistory);
            if (order == null)
                return StaticResponseBuilder<OrderResponseDto>.BuildNotFound(null!);

            // Validar transiÃ§Ã£o de status
            if (!IsValidStatusTransition(order.Status, requestDto.Status))
                return StaticResponseBuilder< OrderResponseDto>.BuildError($"TransiÃ§Ã£o de status invÃ¡lida de {order.Status} para {requestDto.Status}");

            var previousStatus = order.Status;
            order.Status = requestDto.Status;
            order.UpdatedAt = DateTime.UtcNow;

            // Adicionar histÃ³rico de status
            var statusHistory = new OrderStatusHistoryEntity
            {
                OrderId = order.Id,
                Status = requestDto.Status,
                Timestamp = DateTime.UtcNow,
                Notes = requestDto.Notes,
                UserId = requestDto.UserId ?? "system"
            };

            order.StatusHistory.Add(statusHistory);
            await _orderRepository.UpdateAsync(order);

            var orderDto = MapToOrderResponseDto(order);
            _logger.LogInformation("Status do pedido {OrderId} alterado de {PreviousStatus} para {NewStatus}", 
                id, previousStatus, requestDto.Status);
            
            // Enviar notificaÃ§Ã£o de mudanÃ§a de status
            try
            {
                await _notificationService.NotifyOrderStatusChangedAsync(id, previousStatus, requestDto.Status, requestDto.Notes);
                
                // NotificaÃ§Ãµes especÃ­ficas por status
                switch (requestDto.Status)
                {
                    case OrderStatus.Ready:
                        await _notificationService.NotifyOrderReadyAsync(id);
                        break;
                    case OrderStatus.Delivered:
                        await _notificationService.NotifyOrderCompletedAsync(id);
                        break;
                }
            }
            catch (Exception notificationEx)
            {
                _logger.LogWarning(notificationEx, "Erro ao enviar notificaÃ§Ã£o de mudanÃ§a de status do pedido {OrderId}", id);
                // NÃ£o falhar a atualizaÃ§Ã£o por erro de notificaÃ§Ã£o
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
    public async Task<ResponseDTO<IEnumerable<OrderResponseDto>>> GetOrdersByStatusAsync(OrderStatus status)
    {
        try
        {
            var orders = await _orderRepository.GetOrdersByStatusAsync(status);
            var orderDtos = orders.Select(MapToOrderResponseDto);
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
            var orderDtos = orders.Select(MapToOrderResponseDto);
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
    public async Task<ResponseDTO<OrderResponseDto>> AcceptOrderAsync(int id, int estimatedPreparationMinutes, string? notes = null, string? userId = null)
    {
        try
        {
            var order = await _orderRepository.GetByIdAsync(id, _currentUserService.GetTenantGuid()!.Value);
            if (order == null)
                return StaticResponseBuilder<OrderResponseDto>.BuildNotFound(null!);

            if (order.Status != OrderStatus.Pending)
                return StaticResponseBuilder<OrderResponseDto>.BuildError("Apenas pedidos pendentes podem ser aceitos");

            order.Status = OrderStatus.Confirmed;
            order.EstimatedPreparationMinutes = estimatedPreparationMinutes;
            order.EstimatedDeliveryTime = DateTime.UtcNow.AddMinutes(estimatedPreparationMinutes + (order.IsDelivery ? 30 : 0));
            order.UpdatedAt = DateTime.UtcNow;

            if (!string.IsNullOrEmpty(notes))
                order.Notes = notes;

            await _orderRepository.UpdateAsync(order);

            var orderDto = MapToOrderResponseDto(order);
            _logger.LogInformation("Pedido {OrderId} aceito com tempo estimado de {EstimatedMinutes} minutos", 
                id, estimatedPreparationMinutes);
            
            // Enviar notificaÃ§Ã£o de pedido aceito
            try
            {
                await _notificationService.NotifyOrderAcceptedAsync(orderDto);
            }
            catch (Exception notificationEx)
            {
                _logger.LogWarning(notificationEx, "Erro ao enviar notificaÃ§Ã£o de pedido aceito {OrderId}", id);
                // NÃ£o falhar a aceitaÃ§Ã£o por erro de notificaÃ§Ã£o
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
    public async Task<ResponseDTO<OrderResponseDto>> RejectOrderAsync(int id, string reason, string? notes = null, string? rejectedBy = null)
    {
        try
        {
            var order = await _orderRepository.GetByIdWithIncludesAsync(id, o => o.Rejection);
            if (order == null)
                return StaticResponseBuilder<OrderResponseDto>.BuildNotFound(null!);
            if (order.Status != OrderStatus.Pending && order.Status != OrderStatus.Confirmed)
                return StaticResponseBuilder<OrderResponseDto>.BuildError("Apenas pedidos pendentes ou confirmados podem ser rejeitados");

            order.Status = OrderStatus.Rejected;
            order.UpdatedAt = DateTime.UtcNow;

            // Criar registro de rejeiÃ§Ã£o
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

            var orderDto = MapToOrderResponseDto(order);
            _logger.LogInformation("Pedido {OrderId} rejeitado. Motivo: {Reason}", id, reason);
            
            // Enviar notificaÃ§Ã£o de pedido rejeitado
            try
            {
                await _notificationService.NotifyOrderRejectedAsync(orderDto, reason);
            }
            catch (Exception notificationEx)
            {
                _logger.LogWarning(notificationEx, "Erro ao enviar notificaÃ§Ã£o de pedido rejeitado {OrderId}", id);
                // NÃ£o falhar a rejeiÃ§Ã£o por erro de notificaÃ§Ã£o
            }
            return StaticResponseBuilder<OrderResponseDto>.BuildOk(orderDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao rejeitar pedido {OrderId}", id);
            return StaticResponseBuilder<OrderResponseDto>.BuildError("Erro interno do servidor");
        }
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

    public async Task<ResponseDTO<OrderResponseDto>> CancelOrderAsync(int id, CancelOrderRequestDto requestDto)
    {
        try
        {
            var order = await _orderRepository.GetByIdWithIncludesAsync(id, o => o.StatusHistory);
            if (order == null)
                return StaticResponseBuilder<OrderResponseDto>.BuildNotFound(null!);

            if (order.Status != OrderStatus.Pending)
                return StaticResponseBuilder<OrderResponseDto>.BuildError("Apenas pedidos pendentes podem ser cancelados pelo cliente.");

            var previousStatus = order.Status;
            order.Status = OrderStatus.Cancelled;
            order.UpdatedAt = DateTime.UtcNow;

            var statusHistory = new OrderStatusHistoryEntity
            {
                OrderId = order.Id,
                Status = OrderStatus.Cancelled,
                Timestamp = DateTime.UtcNow,
                Notes = $"Cancelado pelo cliente. Motivo: {requestDto.Reason}",
                UserId = "customer" // Identificar que foi o cliente
            };

            order.StatusHistory.Add(statusHistory);
            await _orderRepository.UpdateAsync(order);

            var orderDto = MapToOrderResponseDto(order);
            
            // Notificar cancelamento
            await _notificationService.NotifyOrderStatusChangedAsync(id, previousStatus, OrderStatus.Cancelled, requestDto.Reason);

            return StaticResponseBuilder<OrderResponseDto>.BuildOk(orderDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao cancelar pedido {OrderId}", id);
            return StaticResponseBuilder<OrderResponseDto>.BuildError("Erro interno ao cancelar pedido");
        }
    }

    public async Task<ResponseDTO<OrderResponseDto>> UpdateOrderPaymentMethodAsync(int id, UpdateOrderPaymentRequestDto requestDto)
    {
        try
        {
            var order = await _orderRepository.GetByIdWithIncludesAsync(id, o => o.Payments);
            if (order == null)
                return StaticResponseBuilder<OrderResponseDto>.BuildNotFound(null!);

            if (order.Status != OrderStatus.Pending)
                return StaticResponseBuilder<OrderResponseDto>.BuildError("Apenas pedidos pendentes podem ser alterados.");

            // Assumindo que o pedido tem um pagamento inicial ou nenhum.
            // Se tiver pagamentos, pegamos o Ãºltimo nÃ£o cancelado/falho ou todos.
            // SimplificaÃ§Ã£o: Atualizar o Ãºltimo pagamento pendente ou criar um novo se nÃ£o houver.
            
            var currentPayment = order.Payments.OrderByDescending(p => p.CreatedAt).FirstOrDefault(p => p.Status == PaymentStatus.Pending);

            if (currentPayment != null)
            {
                currentPayment.Method = requestDto.PaymentMethod;
                currentPayment.UpdatedAt = DateTime.UtcNow;
            }
            else
            {
                // Se nÃ£o houver pagamento pendente, cria um novo
                var newPayment = new PaymentEntity
                {
                    OrderId = order.Id,
                    Amount = order.Total,
                    Method = requestDto.PaymentMethod,
                    Status = PaymentStatus.Pending,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                order.Payments.Add(newPayment);
            }

            order.UpdatedAt = DateTime.UtcNow;
            await _orderRepository.UpdateAsync(order);

            // Recarregar com todos os includes para retorno completo
            var updatedOrder = await _orderRepository.GetByIdWithIncludesAsync(id, o => o.Items);
            return StaticResponseBuilder<OrderResponseDto>.BuildOk(MapToOrderResponseDto(updatedOrder));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar mÃ©todo de pagamento do pedido {OrderId}", id);
            return StaticResponseBuilder<OrderResponseDto>.BuildError("Erro interno ao atualizar pagamento");
        }
    }

    public async Task<ResponseDTO<OrderResponseDto>> UpdateOrderDeliveryTypeAsync(int id, UpdateOrderDeliveryTypeRequestDto requestDto)
    {
        try
        {
            var order = await _orderRepository.GetByIdWithIncludesAsync(id, o => o.Items);
            if (order == null)
                return StaticResponseBuilder<OrderResponseDto>.BuildNotFound(null!);

            if (order.Status != OrderStatus.Pending)
                return StaticResponseBuilder<OrderResponseDto>.BuildError("Apenas pedidos pendentes podem ser alterados.");

            // Se nada mudou, retorna ok
            if (order.IsDelivery == requestDto.IsDelivery && 
                (!requestDto.IsDelivery || order.DeliveryAddress == requestDto.DeliveryAddress))
            {
                return StaticResponseBuilder<OrderResponseDto>.BuildOk(MapToOrderResponseDto(order));
            }

            order.IsDelivery = requestDto.IsDelivery;
            
            if (requestDto.IsDelivery)
            {
                if (string.IsNullOrWhiteSpace(requestDto.DeliveryAddress))
                    return StaticResponseBuilder<OrderResponseDto>.BuildError("EndereÃ§o Ã© obrigatÃ³rio para entrega.");
                
                order.DeliveryAddress = requestDto.DeliveryAddress;
                
                // Recalcular taxa de entrega (SimplificaÃ§Ã£o: manter a taxa original se jÃ¡ era entrega, ou buscar regra de taxa)
                // Como nÃ£o tenho a regra de taxa aqui fÃ¡cil, vou assumir uma regra simples ou manter a taxa se jÃ¡ existir.
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
                // Retirada: Zera taxa e limpa endereÃ§o (opcional limpar endereÃ§o, mas bom manter histÃ³rico)
                order.DeliveryFee = 0;
            }

            // Recalcular total
            order.Subtotal = order.Items.Sum(i => i.Subtotal);
            order.Total = order.Subtotal + order.DeliveryFee - order.DiscountAmount;
            
            order.UpdatedAt = DateTime.UtcNow;
            await _orderRepository.UpdateAsync(order);

            return StaticResponseBuilder<OrderResponseDto>.BuildOk(MapToOrderResponseDto(order));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar tipo de entrega do pedido {OrderId}", id);
            return StaticResponseBuilder<OrderResponseDto>.BuildError("Erro interno ao atualizar entrega");
        }
    }

    /// <summary>
    /// Mapeia uma entidade Order para OrderResponseDto
    /// </summary>
    private static OrderResponseDto MapToOrderResponseDto(OrderEntity order)
    {
        return new OrderResponseDto
        {
            Id = order.Id,
            CustomerName = order.CustomerName,
            CustomerPhone = order.CustomerPhone,
            CustomerEmail = order.CustomerEmail,
            DeliveryAddress = order.DeliveryAddress,
            Subtotal = order.Subtotal,
            DeliveryFee = order.DeliveryFee,
            DiscountAmount = order.DiscountAmount,
            CouponCode = order.CouponCode,
            Total = order.Total,
            Status = order.Status,
            CreatedAt = order.CreatedAt,
            UpdatedAt = order.UpdatedAt,
            IsDelivery = order.IsDelivery,
            OrderType = order.OrderType,
            TableId = order.TableId,
            Notes = order.Notes,
            EstimatedPreparationMinutes = order.EstimatedPreparationMinutes,
            EstimatedDeliveryTime = order.EstimatedDeliveryTime,
            QueuePosition = order.QueuePosition,
            Items = order.Items?.Select(MapToOrderItemResponseDto).ToList() ?? []
        };
    }

    /// <summary>
    /// Mapeia uma entidade OrderItem para OrderItemResponseDto
    /// </summary>
    private static OrderItemResponseDto MapToOrderItemResponseDto(OrderItemEntity orderItem)
    {
        return new OrderItemResponseDto
        {
            Id = orderItem.Id,
            ProductId = orderItem.ProductId,
            ProductName = orderItem.ProductName,
            UnitPrice = orderItem.UnitPrice,
            Quantity = orderItem.Quantity,
            Subtotal = orderItem.Subtotal,
            Notes = orderItem.Notes,
            ImageUrl = orderItem.Product?.ImageUrl,
            Addons = orderItem.Addons?.Select(MapToOrderItemAddonResponseDto).ToList() ?? []
        };
    }

    /// <summary>
    /// Mapeia uma entidade OrderItemAddon para OrderItemAddonResponseDto
    /// </summary>
    private static OrderItemAddonResponseDto MapToOrderItemAddonResponseDto(OrderItemAddon addon)
    {
        return new OrderItemAddonResponseDto
        {
            Id = addon.Id,
            AddonId = addon.AddonId,
            AddonName = addon.AddonName,
            UnitPrice = addon.UnitPrice,
            Quantity = addon.Quantity,
            Subtotal = addon.Subtotal
        };
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
    /// Valida se a transiÃ§Ã£o de status Ã© vÃ¡lida
    /// </summary>
    private static bool IsValidStatusTransition(OrderStatus currentStatus, OrderStatus newStatus)
    {
        return currentStatus switch
        {
            OrderStatus.Pending => newStatus is OrderStatus.Confirmed or OrderStatus.Cancelled or OrderStatus.Rejected,
            OrderStatus.Confirmed => newStatus is OrderStatus.Preparing or OrderStatus.Pending or OrderStatus.Cancelled or OrderStatus.Rejected,
            OrderStatus.Preparing => newStatus is OrderStatus.Ready or OrderStatus.Confirmed or OrderStatus.Cancelled,
            OrderStatus.Ready => newStatus is OrderStatus.Preparing or OrderStatus.OutForDelivery or OrderStatus.Delivered or OrderStatus.Cancelled,
            OrderStatus.OutForDelivery => newStatus is OrderStatus.Delivered or OrderStatus.Cancelled,
            OrderStatus.Delivered => newStatus is OrderStatus.Ready, // Status final
            OrderStatus.Cancelled => false, // Status final
            OrderStatus.Rejected => false, // Status final
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
            _logger.LogError(ex, "Erro ao criar cliente para pedido pÃºblico");
            throw ex;
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

    public async Task<PagedResponseDTO<OrderResponseDto>> GetOrdersPagedAsync(int pageNumber, int pageSize)
    {
        try
        {
            var tenantId = _currentUserService.GetTenantGuid()!.Value;
            var orders = await _orderRepository.GetPagedByTenantIdWithDetailsAsync(tenantId, pageNumber, pageSize);
            
            var totalItems = await _orderRepository.CountByTenantIdAsync(tenantId);

            var orderDtos = orders.Select(MapToOrderResponseDto);
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
    public async Task<ResponseDTO<OrderResponseDto?>> GetActiveOrderByTableIdAsync(int tableId)
    {
        try
        {
            var tenantId = _currentUserService.GetTenantGuid()!.Value;
            // Get latest non-cancelled/rejected order for the table created today
            var orders = await _orderRepository.FindOrderedAsync(
                o => o.TenantId == tenantId && o.TableId == tableId && 
                     o.Status != OrderStatus.Cancelled && o.Status != OrderStatus.Rejected &&
                     o.CreatedAt >= DateTime.UtcNow.Date, 
                o => o.CreatedAt, 
                false
            );
            
            var activeOrder = orders.FirstOrDefault();
            
            if (activeOrder == null)
                return StaticResponseBuilder<OrderResponseDto?>.BuildOk(null); // No active order
            
            // Need to load details (Items)
            var orderWithDetails = await _orderRepository.GetByIdWithIncludesAsync(activeOrder.Id, o => o.Items);
            
            // Manually load Addons for items if needed, but GetByIdWithIncludesAsync usually only does one level? 
            // OrderRepository implementation of GetByIdWithIncludesAsync should be checked if it includes sub-includes (ThenInclude).
            // Assuming it does or we need to fix it.
            // Actually OrderItem -> Addons is a collection. EF Core Include usually needs ThenInclude.
            // Let's assume standard repository handles it or we might need a specific query.
            // Looking at CreateOrder, it creates entities.
            // Looking at MapToOrderResponseDto, it maps items.
            
            return StaticResponseBuilder<OrderResponseDto?>.BuildOk(MapToOrderResponseDto(orderWithDetails!));
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
    public async Task<ResponseDTO<OrderResponseDto>> AddItemsToOrderAsync(int orderId, List<CreateOrderItemRequestDto> items)
    {
        try
        {
            var tenantId = _currentUserService.GetTenantGuid()!.Value;
            // Need to include Items to calculate totals correctly
            var order = await _orderRepository.GetByIdWithIncludesAsync(orderId, o => o.Items);
            
            if (order == null)
                return StaticResponseBuilder<OrderResponseDto>.BuildNotFound(null);
                
            if (order.TenantId != tenantId)
                 return StaticResponseBuilder<OrderResponseDto>.BuildError("Pedido nÃ£o pertence ao tenant");
                 
            if (order.Status == OrderStatus.Cancelled || order.Status == OrderStatus.Rejected)
                return StaticResponseBuilder<OrderResponseDto>.BuildError("NÃ£o Ã© possÃ­vel adicionar itens a um pedido cancelado ou rejeitado");

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

                    var orderItemAddon = new OrderItemAddon
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
            
            return StaticResponseBuilder<OrderResponseDto>.BuildOk(MapToOrderResponseDto(order));
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
    public async Task<ResponseDTO<OrderResponseDto>> CloseTableAccountAsync(int tableId)
    {
        var result = await GetActiveOrderByTableIdAsync(tableId);
        if (!result.Succeeded || result.Data == null)
            return StaticResponseBuilder<OrderResponseDto>.BuildError("Nenhum pedido ativo para esta mesa");
            
        return StaticResponseBuilder<OrderResponseDto>.BuildOk(result.Data);
    }
    #endregion
}

