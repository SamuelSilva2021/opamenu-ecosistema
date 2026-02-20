using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using OpaMenu.Application.Services.Opamenu;
using OpaMenu.Domain.DTOs.Loyalty;
using OpaMenu.Domain.Interfaces;
using OpaMenu.Infrastructure.Shared.Entities.Opamenu;
using OpaMenu.Infrastructure.Shared.Enums.Opamenu;
using Xunit;

namespace OpamenuApp.Tests.Services
{
    public class LoyaltyServiceRedemptionTests
    {
        private readonly Mock<ILoyaltyProgramRepository> _mockLoyaltyProgramRepository;
        private readonly Mock<ICustomerLoyaltyRepository> _mockCustomerLoyaltyRepository;
        private readonly Mock<IOrderRepository> _mockOrderRepository;
        private readonly Mock<ICustomerRepository> _mockCustomerRepository;
        private readonly Mock<ILogger<LoyaltyService>> _mockLogger;
        private readonly Mock<ICurrentUserService> _mockCurrentUserService;
        private readonly LoyaltyService _loyaltyService;

        private readonly Guid _tenantId = Guid.NewGuid();
        private readonly Guid _customerId = Guid.NewGuid();
        private readonly string _customerPhone = "11999999999";

        public LoyaltyServiceRedemptionTests()
        {
            _mockLoyaltyProgramRepository = new Mock<ILoyaltyProgramRepository>();
            _mockCustomerLoyaltyRepository = new Mock<ICustomerLoyaltyRepository>();
            _mockOrderRepository = new Mock<IOrderRepository>();
            _mockCustomerRepository = new Mock<ICustomerRepository>();
            _mockLogger = new Mock<ILogger<LoyaltyService>>();
            _mockCurrentUserService = new Mock<ICurrentUserService>();
            
            _mockCurrentUserService.Setup(x => x.GetTenantGuid()).Returns(_tenantId);

            _loyaltyService = new LoyaltyService(
                _mockLoyaltyProgramRepository.Object,
                _mockCustomerLoyaltyRepository.Object,
                _mockOrderRepository.Object,
                _mockCustomerRepository.Object,
                _mockLogger.Object,
                _mockCurrentUserService.Object
            );
        }

        [Fact]
        public async Task RedeemPointsAsync_ShouldDeductFromCorrectProgram()
        {
            // Arrange
            var programAId = Guid.NewGuid();
            var programBId = Guid.NewGuid();
            var pointsToRedeem = 50;

            var customer = new CustomerEntity { Id = _customerId, Phone = _customerPhone };
            var programA = new LoyaltyProgramEntity { Id = programAId, Name = "Program A", TenantId = _tenantId, IsActive = true };
            var balanceA = new CustomerLoyaltyBalanceEntity { Id = Guid.NewGuid(), CustomerId = _customerId, LoyaltyProgramId = programAId, Balance = 100 };
            var balanceB = new CustomerLoyaltyBalanceEntity { Id = Guid.NewGuid(), CustomerId = _customerId, LoyaltyProgramId = programBId, Balance = 200 };

            _mockCustomerRepository.Setup(x => x.GetByPhoneAsync(_tenantId, _customerPhone)).ReturnsAsync(customer);
            _mockLoyaltyProgramRepository.Setup(x => x.GetByIdAsync(programAId, _tenantId)).ReturnsAsync(programA);
            _mockCustomerLoyaltyRepository.Setup(x => x.GetByCustomerAndProgramAsync(_customerId, programAId)).ReturnsAsync(balanceA);

            var dto = new RedeemLoyaltyPointsDto
            {
                CustomerPhone = _customerPhone,
                ProgramId = programAId,
                Points = pointsToRedeem
            };

            // Act
            var result = await _loyaltyService.RedeemPointsAsync(_tenantId, dto);

            // Assert
            Assert.True(result.Succeeded);
            Assert.Equal(50, balanceA.Balance);
            Assert.Equal(200, balanceB.Balance); // Ensure Program B remains untouched
            _mockCustomerLoyaltyRepository.Verify(x => x.AddTransactionAsync(It.Is<LoyaltyTransactionEntity>(t => t.Points == -pointsToRedeem && t.Type == ELoyaltyTransactionType.Redeem)), Times.Once);
            _mockCustomerLoyaltyRepository.Verify(x => x.UpdateAsync(balanceA), Times.Once);
        }

        [Fact]
        public async Task RedeemPointsAsync_WithInsufficientBalance_ShouldReturnError()
        {
            // Arrange
            var programId = Guid.NewGuid();
            var pointsToRedeem = 150;

            var customer = new CustomerEntity { Id = _customerId, Phone = _customerPhone };
            var program = new LoyaltyProgramEntity { Id = programId, Name = "Program A", TenantId = _tenantId, IsActive = true };
            var balance = new CustomerLoyaltyBalanceEntity { Id = Guid.NewGuid(), CustomerId = _customerId, LoyaltyProgramId = programId, Balance = 100 };

            _mockCustomerRepository.Setup(x => x.GetByPhoneAsync(_tenantId, _customerPhone)).ReturnsAsync(customer);
            _mockLoyaltyProgramRepository.Setup(x => x.GetByIdAsync(programId, _tenantId)).ReturnsAsync(program);
            _mockCustomerLoyaltyRepository.Setup(x => x.GetByCustomerAndProgramAsync(_customerId, programId)).ReturnsAsync(balance);

            var dto = new RedeemLoyaltyPointsDto
            {
                CustomerPhone = _customerPhone,
                ProgramId = programId,
                Points = pointsToRedeem
            };

            // Act
            var result = await _loyaltyService.RedeemPointsAsync(_tenantId, dto);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Equal("Saldo insuficiente neste programa", result.Errors.First().Message);
            Assert.Equal(100, balance.Balance); // Balance should not change
            _mockCustomerLoyaltyRepository.Verify(x => x.AddTransactionAsync(It.IsAny<LoyaltyTransactionEntity>()), Times.Never);
        }
    }
}
