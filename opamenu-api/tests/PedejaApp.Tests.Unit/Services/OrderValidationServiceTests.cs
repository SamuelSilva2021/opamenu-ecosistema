using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using OpaMenu.Application.Services;
using OpaMenu.Infrastructure.Shared.Entities;
using OpaMenu.Domain.Interfaces;
using OpaMenu.Domain.DTOs;
using OpaMenu.Infrastructure.Shared.Enums;
using OpaMenu.Application.Common.Models;
using OpaMenu.Application.Services.Interfaces;

namespace OpaMenu.Tests.Unit.Services    
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
                CustomerName = "JoÃ£o Silva",
                CustomerPhone = "11999999999",
                Items = new List<CreateOrderItemRequestDto>
                {
                    new CreateOrderItemRequestDto { ProductId = 1, Quantity = 2 }
                }
            };
            var product = new ProductEntity { Id = 1, Name = "Produto Teste", Price = 15.99m, IsActive = true, TenantId = _tenantId };

            _mockProductRepository.Setup(x => x.GetByIdAsync(1, _tenantId)).ReturnsAsync(product);

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
                    new CreateOrderItemRequestDto { ProductId = 1, Quantity = 2 }
                }
            };

            // Act
            var result = await _validationService.ValidateCreateOrderAsync(createRequest);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Nome do cliente Ã© obrigatÃ³rio.", result.Error);
        }

        [Fact]
        public async Task ValidateCreateOrderAsync_WithEmptyItems_ReturnsBadRequest()
        {
            // Arrange
            var createRequest = new CreateOrderRequestDto
            {
                CustomerName = "JoÃ£o Silva",
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
                CustomerName = "JoÃ£o Silva",
                CustomerPhone = "11999999999",
                Items = new List<CreateOrderItemRequestDto>
                {
                    new CreateOrderItemRequestDto { ProductId = 999, Quantity = 2 }
                }
            };

            _mockProductRepository.Setup(x => x.GetByIdAsync(999, _tenantId)).ReturnsAsync((ProductEntity?)null);

            // Act
            var result = await _validationService.ValidateCreateOrderAsync(createRequest);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Produto com ID 999 nÃ£o encontrado.", result.Error);
        }

        [Fact]
        public async Task ValidateCreateOrderAsync_WithInactiveProduct_ReturnsBadRequest()
        {
            // Arrange
            var createRequest = new CreateOrderRequestDto
            {
                CustomerName = "JoÃ£o Silva",
                CustomerPhone = "11999999999",
                Items = new List<CreateOrderItemRequestDto>
                {
                    new CreateOrderItemRequestDto { ProductId = 1, Quantity = 2 }
                }
            };
            var product = new ProductEntity { Id = 1, Name = "Produto Teste", Price = 15.99m, IsActive = false, TenantId = _tenantId };

            _mockProductRepository.Setup(x => x.GetByIdAsync(1, _tenantId)).ReturnsAsync(product);

            // Act
            var result = await _validationService.ValidateCreateOrderAsync(createRequest);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Produto 'Produto Teste' nÃ£o estÃ¡ disponÃ­vel.", result.Error);
        }

        [Fact]
        public async Task ValidateUpdateOrderAsync_WithValidRequest_ReturnsSuccess()
        {
            // Arrange
            var updateRequest = new UpdateOrderRequestDto
            {
                CustomerName = "JoÃ£o Silva Atualizado",
                CustomerPhone = "11888888888"
            };
            var order = new OrderEntity { Id = 1, Status = OrderStatus.Pending, TenantId = _tenantId };

            _mockOrderRepository.Setup(x => x.GetByIdAsync(1, _tenantId)).ReturnsAsync(order);

            // Act
            var result = await _validationService.ValidateUpdateOrderAsync(1, updateRequest);

            // Assert
            Assert.True(result.Success);
        }

        [Fact]
        public async Task ValidateUpdateOrderAsync_WithNonEditableStatus_ReturnsBadRequest()
        {
            // Arrange
            var updateRequest = new UpdateOrderRequestDto
            {
                CustomerName = "JoÃ£o Silva Atualizado"
            };
            var order = new OrderEntity { Id = 1, Status = OrderStatus.Delivered, TenantId = _tenantId };

            _mockOrderRepository.Setup(x => x.GetByIdAsync(1, _tenantId)).ReturnsAsync(order);

            // Act
            var result = await _validationService.ValidateUpdateOrderAsync(1, updateRequest);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("NÃ£o Ã© possÃ­vel atualizar pedidos finalizados ou cancelados.", result.Error);
        }

        [Fact]
        public async Task ValidateAcceptOrderAsync_WithPendingOrder_ReturnsSuccess()
        {
            // Arrange
            var order = new OrderEntity { Id = 1, Status = OrderStatus.Pending, TenantId = _tenantId };
            var request = new AcceptOrderRequestDto();

            _mockOrderRepository.Setup(x => x.GetByIdAsync(1, _tenantId)).ReturnsAsync(order);

            // Act
            var result = await _validationService.ValidateAcceptOrderAsync(1, request);

            // Assert
            Assert.True(result.Success);
        }

        [Fact]
        public async Task ValidateAcceptOrderAsync_WithNonPendingOrder_ReturnsBadRequest()
        {
            // Arrange
            var order = new OrderEntity { Id = 1, Status = OrderStatus.Confirmed, TenantId = _tenantId };
            var request = new AcceptOrderRequestDto();

            _mockOrderRepository.Setup(x => x.GetByIdAsync(1, _tenantId)).ReturnsAsync(order);

            // Act
            var result = await _validationService.ValidateAcceptOrderAsync(1, request);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Apenas pedidos pendentes podem ser aceitos.", result.Error);
        }

        [Fact]
        public async Task ValidateStatusChangeAsync_WithValidTransition_ReturnsSuccess()
        {
            // Arrange
            var order = new OrderEntity { Id = 1, Status = OrderStatus.Confirmed, TenantId = _tenantId };

            _mockOrderRepository.Setup(x => x.GetByIdAsync(1, _tenantId)).ReturnsAsync(order);

            // Act
            var result = await _validationService.ValidateStatusChangeAsync(1, OrderStatus.Preparing);

            // Assert
            Assert.True(result.Success);
        }

        [Fact]
        public async Task ValidateStatusChangeAsync_WithInvalidTransition_ReturnsBadRequest()
        {
            // Arrange
            var order = new OrderEntity { Id = 1, Status = OrderStatus.Pending, TenantId = _tenantId };

            _mockOrderRepository.Setup(x => x.GetByIdAsync(1, _tenantId)).ReturnsAsync(order);

            // Act
            var result = await _validationService.ValidateStatusChangeAsync(1, OrderStatus.Delivered);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("TransiÃ§Ã£o de status invÃ¡lida: de 'Pending' para 'Delivered'.", result.Error);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public async Task ValidateItemAsync_WithInvalidQuantity_ReturnsBadRequest(int quantity)
        {
            // Arrange
            var item = new CreateOrderItemRequestDto
            {
                ProductId = 1,
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

