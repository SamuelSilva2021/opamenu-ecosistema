using AutoMapper;
using Microsoft.Extensions.Logging;
using OpaMenu.Application.Services.Interfaces.Opamenu;
using OpaMenu.Commons.Api.Commons;
using OpaMenu.Commons.Api.DTOs;
using OpaMenu.Domain.DTOs;
using OpaMenu.Domain.DTOs.Tab;
using OpaMenu.Domain.Interfaces;
using OpaMenu.Infrastructure.Shared.Entities.Opamenu;
using OpaMenu.Infrastructure.Shared.Enums.Opamenu;

namespace OpaMenu.Application.Services.Opamenu;

public class TabService(
    ITabRepository tabRepository,
    ITableRepository tableRepository,
    IOrderRepository orderRepository,
    IProductRepository productRepository,
    IAditionalRepository aditionalRepository,
    ICurrentUserService currentUserService,
    ICustomerRepository customerRepository,
    ITenantCustomerRepository tenantCustomerRepository,
    IMapper mapper,
    ILogger<TabService> logger) : ITabService
{
    private readonly ITabRepository _tabRepository = tabRepository;
    private readonly ITableRepository _tableRepository = tableRepository;
    private readonly IOrderRepository _orderRepository = orderRepository;
    private readonly IProductRepository _productRepository = productRepository;
    private readonly IAditionalRepository _aditionalRepository = aditionalRepository;
    private readonly ICurrentUserService _currentUserService = currentUserService;
    private readonly ICustomerRepository _customerRepository = customerRepository;
    private readonly ITenantCustomerRepository _tenantCustomerRepository = tenantCustomerRepository;
    private readonly IMapper _mapper = mapper;
    private readonly ILogger<TabService> _logger = logger;

    public async Task<ResponseDTO<IEnumerable<TabResponseDto>>> GetByTableIdAsync(Guid tableId, ETabStatus? status = null)
    {
        try
        {
            var tenantId = _currentUserService.GetTenantGuid();
            if (tenantId == null || tenantId == Guid.Empty)
                return StaticResponseBuilder<IEnumerable<TabResponseDto>>.BuildError("Tenant não identificado");

            var table = await _tableRepository.GetByIdAsync(tableId, tenantId.Value);
            if (table == null)
                return StaticResponseBuilder<IEnumerable<TabResponseDto>>.BuildError("Mesa não encontrada");

            var tabs = await _tabRepository.GetByTableIdAsync(tenantId.Value, tableId, status);
            return StaticResponseBuilder<IEnumerable<TabResponseDto>>.BuildOk(_mapper.Map<IEnumerable<TabResponseDto>>(tabs));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao listar comandas da mesa {TableId}", tableId);
            return StaticResponseBuilder<IEnumerable<TabResponseDto>>.BuildError("Erro ao listar comandas");
        }
    }

    public async Task<ResponseDTO<TabResponseDto>> GetByIdAsync(Guid tableId, Guid tabId)
    {
        try
        {
            var tenantId = _currentUserService.GetTenantGuid();
            if (tenantId == null || tenantId == Guid.Empty)
                return StaticResponseBuilder<TabResponseDto>.BuildError("Tenant não identificado");

            var tab = await _tabRepository.GetFullTabByIdAndTableIdAsync(tenantId.Value, tableId, tabId);
            if (tab == null)
                return StaticResponseBuilder<TabResponseDto>.BuildError("Comanda não encontrada");

            return StaticResponseBuilder<TabResponseDto>.BuildOk(_mapper.Map<TabResponseDto>(tab));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter comanda {TabId} da mesa {TableId}", tabId, tableId);
            return StaticResponseBuilder<TabResponseDto>.BuildError("Erro ao obter comanda");
        }
    }

    public async Task<ResponseDTO<TabResponseDto>> OpenAsync(Guid tableId, CreateTabRequestDto dto)
    {
        try
        {
            var tenantId = _currentUserService.GetTenantGuid();
            if (tenantId == null || tenantId == Guid.Empty)
                return StaticResponseBuilder<TabResponseDto>.BuildError("Tenant não identificado");

            var table = await _tableRepository.GetByIdAsync(tableId, tenantId.Value);
            if (table == null)
                return StaticResponseBuilder<TabResponseDto>.BuildError("Mesa não encontrada");

            var name = string.IsNullOrWhiteSpace(dto.Name) ? null : dto.Name.Trim();
            if (name == null)
            {
                var count = await _tabRepository.CountByTableIdAsync(tenantId.Value, tableId);
                name = $"Comanda {count + 1}";
            }

            var entity = new TabEntity
            {
                TableId = tableId,
                Name = name,
                Status = ETabStatus.Open,
                OpenedAt = DateTime.UtcNow,
                TenantId = tenantId.Value
            };

            await _tabRepository.AddAsync(entity);
            return StaticResponseBuilder<TabResponseDto>.BuildOk(_mapper.Map<TabResponseDto>(entity));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao abrir comanda na mesa {TableId}", tableId);
            return StaticResponseBuilder<TabResponseDto>.BuildError("Erro ao abrir comanda");
        }
    }

    public async Task<ResponseDTO<TabResponseDto>> CloseAsync(Guid tableId, Guid tabId)
    {
        try
        {
            var tenantId = _currentUserService.GetTenantGuid();
            if (tenantId == null || tenantId == Guid.Empty)
                return StaticResponseBuilder<TabResponseDto>.BuildError("Tenant não identificado");

            var tab = await _tabRepository.GetByIdAsync(tabId, tenantId.Value);
            if (tab == null || tab.TableId != tableId)
                return StaticResponseBuilder<TabResponseDto>.BuildError("Comanda não encontrada");

            if (tab.Status == ETabStatus.Closed)
                return StaticResponseBuilder<TabResponseDto>.BuildError("Comanda já está fechada");

            var orders = await _orderRepository.GetByTabIdAsync(tenantId.Value, tabId);
            var unpaidOrders = orders.Where(o =>
            {
                if (o.Status == EOrderStatus.Cancelled || o.Status == EOrderStatus.Rejected) return false;
                var paid = o.Payments.Where(p => p.Status == EPaymentStatus.Paid).Sum(p => p.Amount);
                return paid < o.Total;
            }).ToList();

            if (unpaidOrders.Any())
                return StaticResponseBuilder<TabResponseDto>.BuildError("Comanda possui pedidos com pagamento pendente");

            tab.Status = ETabStatus.Closed;
            tab.ClosedAt = DateTime.UtcNow;

            await _tabRepository.UpdateAsync(tab);
            return StaticResponseBuilder<TabResponseDto>.BuildOk(_mapper.Map<TabResponseDto>(tab));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao fechar comanda {TabId} da mesa {TableId}", tabId, tableId);
            return StaticResponseBuilder<TabResponseDto>.BuildError("Erro ao fechar comanda");
        }
    }

    public async Task<ResponseDTO<TabResponseDto>> UpdateAsync(Guid tabId, UpdateTabRequestDto dto)
    {
        try
        {
            var tenantId = _currentUserService.GetTenantGuid();
            if (tenantId == null || tenantId == Guid.Empty)
                return StaticResponseBuilder<TabResponseDto>.BuildError("Tenant não identificado");

            var tab = await _tabRepository.GetByIdAsync(tabId, tenantId.Value);
            if (tab == null)
                return StaticResponseBuilder<TabResponseDto>.BuildError("Comanda não encontrada");

            if (dto.TableId.HasValue && dto.TableId.Value != tab.TableId)
            {
                var table = await _tableRepository.GetByIdAsync(dto.TableId.Value, tenantId.Value);
                if (table == null)
                    return StaticResponseBuilder<TabResponseDto>.BuildError("Mesa não encontrada");

                tab.TableId = dto.TableId.Value;
            }

            if (!string.IsNullOrWhiteSpace(dto.Name))
                tab.Name = dto.Name.Trim();

            await _tabRepository.UpdateAsync(tab);
            return StaticResponseBuilder<TabResponseDto>.BuildOk(_mapper.Map<TabResponseDto>(tab));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar comanda {TabId}", tabId);
            return StaticResponseBuilder<TabResponseDto>.BuildError("Erro ao atualizar comanda");
        }
    }

    public async Task<ResponseDTO<bool>> DeleteAsync(Guid tabId)
    {
        try
        {
            var tenantId = _currentUserService.GetTenantGuid();
            if (tenantId == null || tenantId == Guid.Empty)
                return StaticResponseBuilder<bool>.BuildError("Tenant não identificado");

            var tab = await _tabRepository.GetByIdAsync(tabId, tenantId.Value);
            if (tab == null)
                return StaticResponseBuilder<bool>.BuildError("Comanda não encontrada");

            var orders = await _orderRepository.GetByTabIdAsync(tenantId.Value, tabId);
            if (orders.Any(o => o.Status != EOrderStatus.Cancelled && o.Status != EOrderStatus.Rejected))
            {
                foreach (var order in orders)
                {
                    if (order.Status == EOrderStatus.Cancelled || order.Status == EOrderStatus.Rejected) continue;
                    var paid = order.Payments.Where(p => p.Status == EPaymentStatus.Paid).Sum(p => p.Amount);
                    if (paid < order.Total)
                        return StaticResponseBuilder<bool>.BuildError("Não é possível excluir comanda com pedidos pendentes de pagamento");
                }
            }

            await _tabRepository.DeleteAsync(tab);
            return StaticResponseBuilder<bool>.BuildOk(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao excluir comanda {TabId}", tabId);
            return StaticResponseBuilder<bool>.BuildError("Erro ao excluir comanda");
        }
    }

    public async Task<ResponseDTO<IEnumerable<OrderItemResponseDto>>> GetItemsAsync(Guid tabId)
    {
        try
        {
            var tenantId = _currentUserService.GetTenantGuid();
            if (tenantId == null || tenantId == Guid.Empty)
                return StaticResponseBuilder<IEnumerable<OrderItemResponseDto>>.BuildError("Tenant não identificado");

            var tab = await _tabRepository.GetFullTabByIdAndTableIdAsync(tenantId.Value, Guid.Empty, tabId); // Guid.Empty as tableId since we only want tab by Id
            if (tab == null)
            {
                // Fallback trial if tableId is strictly checked by repository
                var existingTab = await _tabRepository.GetByIdAsync(tabId, tenantId.Value);
                if (existingTab == null) return StaticResponseBuilder<IEnumerable<OrderItemResponseDto>>.BuildError("Comanda não encontrada");
                tab = await _tabRepository.GetFullTabByIdAndTableIdAsync(tenantId.Value, existingTab.TableId, tabId);
            }
            
            if (tab == null) return StaticResponseBuilder<IEnumerable<OrderItemResponseDto>>.BuildError("Comanda não encontrada");

            var items = tab.Orders?.SelectMany(o => o.Items) ?? Enumerable.Empty<OrderItemEntity>();
            return StaticResponseBuilder<IEnumerable<OrderItemResponseDto>>.BuildOk(_mapper.Map<IEnumerable<OrderItemResponseDto>>(items));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao listar itens da comanda {TabId}", tabId);
            return StaticResponseBuilder<IEnumerable<OrderItemResponseDto>>.BuildError("Erro ao listar itens");
        }
    }

    public async Task<ResponseDTO<OrderResponseDto>> AddItemsAsync(Guid tabId, List<CreateOrderItemRequestDto> items)
    {
        try
        {
            var tenantId = _currentUserService.GetTenantGuid();
            if (tenantId == null || tenantId == Guid.Empty)
                return StaticResponseBuilder<OrderResponseDto>.BuildError("Tenant não identificado");

            var tab = await _tabRepository.GetByIdAsync(tabId, tenantId.Value);
            if (tab == null)
                return StaticResponseBuilder<OrderResponseDto>.BuildError("Comanda não encontrada");

            if (tab.Status == ETabStatus.Closed)
                return StaticResponseBuilder<OrderResponseDto>.BuildError("Comanda está fechada");

            var table = await _tableRepository.GetByIdAsync(tab.TableId, tenantId.Value);
            if (table == null)
                return StaticResponseBuilder<OrderResponseDto>.BuildError("Comanda não está vinculada a uma mesa");

            var customer = await GetOrCreateCustomerForTableAsync(tenantId.Value, table);

            var existingOrders = await _orderRepository.GetByTabIdAsync(tenantId.Value, tabId);
            var order = existingOrders.FirstOrDefault(o =>
                o.Status != EOrderStatus.Cancelled &&
                o.Status != EOrderStatus.Delivered &&
                o.Status != EOrderStatus.Rejected);

            var createNewOrder = order == null;
            if (createNewOrder)
            {
                order = new OrderEntity
                {
                    TenantId = tenantId.Value,
                    TabId = tabId,
                    TableId = tab.TableId,
                    OrderType = EOrderType.Table,
                    Status = EOrderStatus.Pending,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    CustomerId = customer.Id,
                    CustomerName = customer.Name ?? string.Empty,
                    CustomerPhone = customer.Phone ?? string.Empty,
                    CustomerEmail = customer.Email,
                    IsDelivery = false
                };

                var lastOrderNumber = await _orderRepository.GetLastOrderNumberAsync(tenantId.Value, DateTime.UtcNow);
                order.OrderNumber = (lastOrderNumber ?? 0) + 1;
            }

            foreach (var itemDto in items)
            {
                var product = await _productRepository.GetByIdAsync(itemDto.ProductId, tenantId.Value);
                if (product == null) continue;

                var orderItem = new OrderItemEntity
                {
                    ProductId = itemDto.ProductId,
                    ProductName = product.Name,
                    UnitPrice = product.Price,
                    Quantity = itemDto.Quantity,
                    Notes = itemDto.Notes,
                    Subtotal = product.Price * itemDto.Quantity,
                    Product = product
                };

                foreach (var aditionalDto in itemDto.Aditionals)
                {
                    var aditional = await _aditionalRepository.GetByIdAsync(aditionalDto.AditionalId, tenantId.Value);
                    if (aditional == null) continue;

                    orderItem.Aditionals.Add(new OrderItemAditionalEntity
                    {
                        AditionalId = aditionalDto.AditionalId,
                        AditionalName = aditional.Name,
                        UnitPrice = aditional.Price,
                        Quantity = aditionalDto.Quantity,
                        Subtotal = aditional.Price * aditionalDto.Quantity
                    });
                    orderItem.Subtotal += aditional.Price * aditionalDto.Quantity;
                }

                order!.Items.Add(orderItem);
            }

            order!.Subtotal = order.Items.Sum(i => i.Subtotal);
            order.Total = order.Subtotal;
            order.UpdatedAt = DateTime.UtcNow;

            if(createNewOrder)
                await _orderRepository.AddAsync(order);
            else
                await _orderRepository.UpdateAsync(order);

            return StaticResponseBuilder<OrderResponseDto>.BuildOk(_mapper.Map<OrderResponseDto>(order));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao adicionar itens à comanda {TabId}", tabId);
            return StaticResponseBuilder<OrderResponseDto>.BuildError("Erro ao adicionar itens");
        }
    }
    private async Task<CustomerEntity> GetOrCreateCustomerForTableAsync(Guid tenantId, TableEntity table)
    {
        var email = $"{table.Name?.Replace(" ", "").ToLower()}@mesa.com";

        var existing = await _customerRepository.GetByEmailAsync(tenantId, email);
        if (existing != null) return existing;

        var now = DateTime.UtcNow;
        var customer = new CustomerEntity
        {
            Id = Guid.NewGuid(),
            Name = table.Name,
            Phone = "0000000000",
            Email = email,
            CreatedAt = now,
            UpdatedAt = now
        };

        var createdCustomer = await _customerRepository.CreateAsync(customer);

        await _tenantCustomerRepository.CreateAsync(new TenantCustomerEntity
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            CustomerId = createdCustomer.Id,
            DisplayName = createdCustomer.Name,
            TotalOrders = 0,
            CreatedAt = now,
            UpdatedAt = now
        });

        return createdCustomer;
    }
        
}
