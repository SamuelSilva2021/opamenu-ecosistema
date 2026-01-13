using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using OpaMenu.Application.Services;
using OpaMenu.Application.Services.Interfaces;
using OpaMenu.Infrastructure.Shared.Entities;
using OpaMenu.Domain.Interfaces;
using OpaMenu.Domain.DTOs;
using OpaMenu.Infrastructure.Shared.Enums;
using OpaMenu.Application.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace PedejaApp.Tests.Unit.Services    
{
    public class OrderServiceTests
    {
        private readonly Mock<IOrderRepository> _mockOrderRepository;
        private readonly Mock<IProductRepository> _mockProductRepository;
        private readonly Mock<IAddonRepository> _mockAddonRepository;
        private readonly Mock<ICouponRepository> _mockCouponRepository;
        private readonly Mock<INotificationService> _mockNotificationService;
        private readonly Mock<ICurrentUserService> _mockCurrentUserService;
        private readonly Mock<ITenantCustomerRepository> _mockTenantCustomerRepository;
        private readonly Mock<ICustomerRepository> _mockCustomerRepository;
        private readonly Mock<ITenantRepository> _mockTenantRepository;
        private readonly Mock<ITableRepository> _mockTableRepository;
        private readonly Mock<ILogger<OrderService>> _mockLogger;
        private readonly OrderService _orderService;

        private readonly Guid _tenantId = Guid.NewGuid();

        public OrderServiceTests()
        {
            _mockOrderRepository = new Mock<IOrderRepository>();
            _mockProductRepository = new Mock<IProductRepository>();
            _mockAddonRepository = new Mock<IAddonRepository>();
            _mockCouponRepository = new Mock<ICouponRepository>();
            _mockNotificationService = new Mock<INotificationService>();
            _mockCurrentUserService = new Mock<ICurrentUserService>();
            _mockTenantCustomerRepository = new Mock<ITenantCustomerRepository>();
            _mockCustomerRepository = new Mock<ICustomerRepository>();
            _mockTenantRepository = new Mock<ITenantRepository>();
            _mockTableRepository = new Mock<ITableRepository>();
            _mockLogger = new Mock<ILogger<OrderService>>();

            _mockCurrentUserService.Setup(x => x.GetTenantGuid()).Returns(_tenantId);

            _orderService = new OrderService(
                _mockOrderRepository.Object,
                _mockProductRepository.Object,
                _mockAddonRepository.Object,
                _mockCouponRepository.Object,
                _mockNotificationService.Object,
                _mockCurrentUserService.Object,
                _mockLogger.Object,
                _mockTenantCustomerRepository.Object,
                _mockCustomerRepository.Object,
                _mockTenantRepository.Object,
                _mockTableRepository.Object
            );
        }

        [Fact]
        public async Task CreateOrderAsync_WithTableOrder_ShouldValidateTableAndCreateOrder()
        {
            // Arrange
            var tableId = 1;
            var createRequest = new CreateOrderRequestDto
            {
                CustomerName = "Test Client",
                CustomerPhone = "11999999999",
                OrderType = OrderType.Table,
                TableId = tableId,
                Items = new List<CreateOrderItemRequestDto>
                {
                    new CreateOrderItemRequestDto { ProductId = 1, Quantity = 1 }
                }
            };

            var product = new ProductEntity { Id = 1, Name = "Burger", Price = 20.00m, IsActive = true };
            var table = new TableEntity { Id = tableId, Name = "Mesa 1", TenantId = _tenantId };
            var customer = new CustomerEntity { Id = Guid.NewGuid(), Name = "Test Client", Phone = "11999999999" };
            var tenantCustomer = new TenantCustomerEntity { Customer = customer, TenantId = _tenantId };

            _mockProductRepository.Setup(x => x.GetByIdAsync(1, _tenantId)).ReturnsAsync(product);
            _mockTableRepository.Setup(x => x.GetByIdAsync(tableId, _tenantId)).ReturnsAsync(table);
            _mockCustomerRepository.Setup(x => x.GetByPhoneAsync(_tenantId, "11999999999")).ReturnsAsync(customer);
            _mockTenantCustomerRepository.Setup(x => x.GetByTenantIdAndCustomerIdAsync(_tenantId, customer.Id)).ReturnsAsync(tenantCustomer);
            
            _mockOrderRepository.Setup(x => x.AddAsync(It.IsAny<OrderEntity>()))
                .ReturnsAsync((OrderEntity o) => {
                    o.Id = 123;
                    return o;
                });

            // Act
            var result = await _orderService.CreateOrderAsync(createRequest);

            // Assert
            Assert.True(result.Succeeded);
            Assert.Equal(OrderType.Table, result.Data!.OrderType);
            Assert.Equal(tableId, result.Data!.TableId);
            _mockTableRepository.Verify(x => x.GetByIdAsync(tableId, _tenantId), Times.Once);
        }

        [Fact]
        public async Task CreateOrderAsync_WithTableOrder_MissingTableId_ShouldReturnError()
        {
            // Arrange
            var createRequest = new CreateOrderRequestDto
            {
                CustomerName = "Test Client",
                CustomerPhone = "11999999999",
                OrderType = OrderType.Table,
                TableId = null, // Missing TableId
                Items = new List<CreateOrderItemRequestDto>
                {
                    new CreateOrderItemRequestDto { ProductId = 1, Quantity = 1 }
                }
            };

            var product = new ProductEntity { Id = 1, Name = "Burger", Price = 20.00m, IsActive = true };
            _mockProductRepository.Setup(x => x.GetByIdAsync(1, _tenantId)).ReturnsAsync(product);

            // Act
            var result = await _orderService.CreateOrderAsync(createRequest);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Equal("Mesa Ã© obrigatÃ³ria para pedidos na mesa", result.Errors.First().Message);
        }

        [Fact]
        public async Task CreateOrderAsync_WithTableOrder_InvalidTable_ShouldReturnError()
        {
            // Arrange
            var tableId = 99;
            var createRequest = new CreateOrderRequestDto
            {
                CustomerName = "Test Client",
                CustomerPhone = "11999999999",
                OrderType = OrderType.Table,
                TableId = tableId,
                Items = new List<CreateOrderItemRequestDto>
                {
                    new CreateOrderItemRequestDto { ProductId = 1, Quantity = 1 }
                }
            };

            var product = new ProductEntity { Id = 1, Name = "Burger", Price = 20.00m, IsActive = true };
            _mockProductRepository.Setup(x => x.GetByIdAsync(1, _tenantId)).ReturnsAsync(product);
            _mockTableRepository.Setup(x => x.GetByIdAsync(tableId, _tenantId)).ReturnsAsync((TableEntity?)null); // Table not found

            // Act
            var result = await _orderService.CreateOrderAsync(createRequest);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Equal("Mesa nÃ£o encontrada", result.Errors.First().Message);
        }
    }
}
