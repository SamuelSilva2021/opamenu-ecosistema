using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using OpaMenu.Application.Common.Models;
using OpaMenu.Application.Services.Interfaces.Opamenu;
using OpaMenu.Infrastructure.Shared.Enums.Opamenu;
using OpaMenu.Application.Services.Opamenu;
using OpaMenu.Domain.Interfaces;
using OpaMenu.Infrastructure.Shared.Entities.Opamenu;
using OpaMenu.Domain.DTOs;
using OpaMenu.Application.DTOs;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using System.Linq;

namespace OpamenuApp.Tests.Services    
{
    public class OrderValidationServiceTests
    {
        private readonly Mock<IOrderRepository> _mockOrderRepository;
        private readonly Mock<IProductRepository> _mockProductRepository;
        private readonly Mock<ILogger<OrderValidationService>> _mockLogger;
        private readonly Mock<ICurrentUserService> _mockCurrentUserService;
        private readonly OrderValidationService _validationService;
        private readonly Guid _tenantId = Guid.NewGuid();

        public OrderValidationServiceTests()
        {
            _mockOrderRepository = new Mock<IOrderRepository>();
            _mockProductRepository = new Mock<IProductRepository>();
            _mockLogger = new Mock<ILogger<OrderValidationService>>();
            _mockCurrentUserService = new Mock<ICurrentUserService>();
            
            _mockCurrentUserService.Setup(x => x.GetTenantGuid()).Returns(_tenantId);

            _validationService = new OrderValidationService(
                _mockOrderRepository.Object,
                _mockProductRepository.Object,
                _mockLogger.Object,
                _mockCurrentUserService.Object
            );
        }

        [Fact]
        public async Task ValidateCreateOrderAsync_WithValidRequest_ReturnsSuccess()
        {
            // Arrange
            var createRequest = new CreateOrderRequestDto
            {
                CustomerName = "João Silva",
                CustomerPhone = "11999999999",
                Items = new List<CreateOrderItemRequestDto>
                {
                    new CreateOrderItemRequestDto { ProductId = Guid.NewGuid(), Quantity = 2 }
                }
            };
            var productId = Guid.NewGuid();
            var product = new ProductEntity { Id = productId, Name = "Produto Teste", Price = 15.99m, IsActive = true, TenantId = _tenantId };

            _mockProductRepository.Setup(x => x.GetByIdAsync(productId, _tenantId)).ReturnsAsync(product);

            createRequest.Items[0].ProductId = productId;

            // Act
            var result = await _validationService.ValidateCreateOrderAsync(createRequest);

            // Assert
            Assert.True(result.Success);
        }

        [Fact]
        public async Task ValidateCreateOrderAsync_WithEmptyCustomerName_ReturnsBadRequest()
        {
            // Arrange
            var createRequest = new CreateOrderRequestDto
            {
                CustomerName = "",
                CustomerPhone = "11999999999",
                Items = new List<CreateOrderItemRequestDto>
                {
                    new CreateOrderItemRequestDto { ProductId = Guid.NewGuid(), Quantity = 2 }
                }
            };

            // Act
            var result = await _validationService.ValidateCreateOrderAsync(createRequest);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Nome do cliente é obrigatório.", result.Error);
        }

        [Fact]
        public async Task ValidateCreateOrderAsync_WithEmptyItems_ReturnsBadRequest()
        {
            // Arrange
            var createRequest = new CreateOrderRequestDto
            {
                CustomerName = "João Silva",
                CustomerPhone = "11999999999",
                Items = new List<CreateOrderItemRequestDto>()
            };

            // Act
            var result = await _validationService.ValidateCreateOrderAsync(createRequest);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("O pedido deve conter pelo menos um item.", result.Error);
        }

        [Fact]
        public async Task ValidateCreateOrderAsync_WithInvalidProduct_ReturnsBadRequest()
        {
            // Arrange
            var createRequest = new CreateOrderRequestDto
            {
                CustomerName = "João Silva",
                CustomerPhone = "11999999999",
                Items = new List<CreateOrderItemRequestDto>
                {
                    new CreateOrderItemRequestDto { ProductId = Guid.NewGuid(), Quantity = 2 }
                }
            };

            var productId = Guid.NewGuid();
            createRequest.Items[0].ProductId = productId;
            _mockProductRepository.Setup(x => x.GetByIdAsync(productId, _tenantId)).ReturnsAsync((ProductEntity?)null);

            // Act
            var result = await _validationService.ValidateCreateOrderAsync(createRequest);

            // Assert
            Assert.False(result.Success);
            Assert.Equal($"Produto com ID {productId} não encontrado.", result.Error);
        }

        [Fact]
        public async Task ValidateCreateOrderAsync_WithInactiveProduct_ReturnsBadRequest()
        {
            // Arrange
            var createRequest = new CreateOrderRequestDto
            {
                CustomerName = "João Silva",
                CustomerPhone = "11999999999",
                Items = new List<CreateOrderItemRequestDto>
                {
                    new CreateOrderItemRequestDto { ProductId = Guid.NewGuid(), Quantity = 2 }
                }
            };
            var productId = Guid.NewGuid();
            var product = new ProductEntity { Id = productId, Name = "Produto Teste", Price = 15.99m, IsActive = false, TenantId = _tenantId };

            _mockProductRepository.Setup(x => x.GetByIdAsync(productId, _tenantId)).ReturnsAsync(product);
            createRequest.Items[0].ProductId = productId;

            // Act  
            var result = await _validationService.ValidateCreateOrderAsync(createRequest);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Produto 'Produto Teste' não está disponível.", result.Error);
        }

        [Fact]
        public async Task ValidateUpdateOrderAsync_WithValidRequest_ReturnsSuccess()
        {
            // Arrange
            var updateRequest = new UpdateOrderRequestDto
            {
                CustomerName = "João Silva Atualizado",
                CustomerPhone = "11888888888"
            };
            var orderId = Guid.NewGuid();
            var order = new OrderEntity { Id = orderId, Status = EOrderStatus.Pending, TenantId = _tenantId };

            _mockOrderRepository.Setup(x => x.GetByIdAsync(orderId, _tenantId)).ReturnsAsync(order);

            // Act
            var result = await _validationService.ValidateUpdateOrderAsync(orderId, updateRequest);

            // Assert
            Assert.True(result.Success);
        }

        [Fact]
        public async Task ValidateUpdateOrderAsync_WithNonEditableStatus_ReturnsBadRequest()
        {
            // Arrange
            var updateRequest = new UpdateOrderRequestDto
            {
                CustomerName = "João Silva Atualizado"
            };
            var orderId = Guid.NewGuid();
            var order = new OrderEntity { Id = orderId, Status = EOrderStatus.Delivered, TenantId = _tenantId };

            _mockOrderRepository.Setup(x => x.GetByIdAsync(orderId, _tenantId)).ReturnsAsync(order);

            // Act
            var result = await _validationService.ValidateUpdateOrderAsync(orderId, updateRequest);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Não é possível atualizar pedidos finalizados ou cancelados.", result.Error);
        }

        [Fact]
        public async Task ValidateAcceptOrderAsync_WithPendingOrder_ReturnsSuccess()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var order = new OrderEntity { Id = orderId, Status = EOrderStatus.Pending, TenantId = _tenantId };
            var request = new AcceptOrderRequestDto();

            _mockOrderRepository.Setup(x => x.GetByIdAsync(orderId, _tenantId)).ReturnsAsync(order);

            // Act
            var result = await _validationService.ValidateAcceptOrderAsync(orderId, request);

            // Assert
            Assert.True(result.Success);
        }

        [Fact]
        public async Task ValidateAcceptOrderAsync_WithNonPendingOrder_ReturnsBadRequest()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var order = new OrderEntity { Id = orderId, Status = EOrderStatus.Preparing, TenantId = _tenantId };
            var request = new AcceptOrderRequestDto();

            _mockOrderRepository.Setup(x => x.GetByIdAsync(orderId, _tenantId)).ReturnsAsync(order);

            // Act
            var result = await _validationService.ValidateAcceptOrderAsync(orderId, request);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Apenas pedidos pendentes podem ser aceitos.", result.Error);
        }

        [Fact]
        public async Task ValidateStatusChangeAsync_WithValidTransition_ReturnsSuccess()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var order = new OrderEntity { Id = orderId, Status = EOrderStatus.Preparing, TenantId = _tenantId };

            _mockOrderRepository.Setup(x => x.GetByIdAsync(orderId, _tenantId)).ReturnsAsync(order);

            // Act
            var result = await _validationService.ValidateStatusChangeAsync(orderId, EOrderStatus.Ready);

            // Assert
            Assert.True(result.Success);
        }

        [Fact]
        public async Task ValidateStatusChangeAsync_WithInvalidTransition_ReturnsBadRequest()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var order = new OrderEntity { Id = orderId, Status = EOrderStatus.Pending, TenantId = _tenantId };

            _mockOrderRepository.Setup(x => x.GetByIdAsync(orderId, _tenantId)).ReturnsAsync(order);

            // Act
            var result = await _validationService.ValidateStatusChangeAsync(orderId, EOrderStatus.Delivered);

            // Assert
            Assert.False(result.Success);
            Assert.Equal($"Transição de status inválida: de '{EOrderStatus.Pending}' para '{EOrderStatus.Delivered}'.", result.Error);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public async Task ValidateItemAsync_WithInvalidQuantity_ReturnsBadRequest(int quantity)
        {
            // Arrange
            var item = new CreateOrderItemRequestDto
            {
                ProductId = Guid.NewGuid(),
                Quantity = quantity
            };

            // Act
            var result = await _validationService.ValidateItemAsync(item);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Quantidade deve ser maior que zero.", result.Error);
        }
    }
}
