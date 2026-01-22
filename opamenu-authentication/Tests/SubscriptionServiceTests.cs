using Authenticator.API.Core.Application.Implementation.MultiTenant;
using Authenticator.API.Core.Application.Interfaces.Auth;
using Authenticator.API.Core.Application.Interfaces.MultiTenant;
using Authenticator.API.Core.Application.Interfaces.Payment;
using Authenticator.API.Core.Domain.AccessControl.UserAccounts.DTOs;
using Authenticator.API.Core.Domain.Api;
using Authenticator.API.Core.Domain.Authentication;
using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using OpaMenu.Infrastructure.Shared.Entities.MultiTenant.Plan;
using OpaMenu.Infrastructure.Shared.Entities.MultiTenant.PlanModule;
using OpaMenu.Infrastructure.Shared.Entities.MultiTenant.Subscription;
using OpaMenu.Infrastructure.Shared.Entities.MultiTenant.Tenant;
using OpaMenu.Infrastructure.Shared.Entities.MultiTenant.TenantModule;
using OpaMenu.Infrastructure.Shared.Entities.MultiTenant.TenantProduct;
using Xunit;

namespace Authenticator.API.Tests
{
    public class SubscriptionServiceTests
    {
        private readonly Mock<ISubscriptionRepository> _subscriptionRepositoryMock;
        private readonly Mock<ITenantRepository> _tenantRepositoryMock;
        private readonly Mock<IPlanRepository> _planRepositoryMock;
        private readonly Mock<IPaymentGatewayService> _paymentGatewayServiceMock;
        private readonly Mock<IUserContext> _userContextMock;
        private readonly Mock<ITenantProductRepository> _tenantProductRepositoryMock;
        private readonly Mock<IPlanModuleRepository> _planModuleRepositoryMock;
        private readonly Mock<ITenantModuleRepository> _tenantModuleRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILogger<SubscriptionService>> _loggerMock;
        private readonly SubscriptionService _service;

        public SubscriptionServiceTests()
        {
            _subscriptionRepositoryMock = new Mock<ISubscriptionRepository>();
            _tenantRepositoryMock = new Mock<ITenantRepository>();
            _planRepositoryMock = new Mock<IPlanRepository>();
            _paymentGatewayServiceMock = new Mock<IPaymentGatewayService>();
            _userContextMock = new Mock<IUserContext>();
            _tenantProductRepositoryMock = new Mock<ITenantProductRepository>();
            _planModuleRepositoryMock = new Mock<IPlanModuleRepository>();
            _tenantModuleRepositoryMock = new Mock<ITenantModuleRepository>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILogger<SubscriptionService>>();

            _service = new SubscriptionService(
                _subscriptionRepositoryMock.Object,
                _tenantRepositoryMock.Object,
                _planRepositoryMock.Object,
                _paymentGatewayServiceMock.Object,
                _userContextMock.Object,
                _tenantProductRepositoryMock.Object,
                _planModuleRepositoryMock.Object,
                _tenantModuleRepositoryMock.Object,
                _mapperMock.Object,
                _loggerMock.Object
            );
        }

        [Fact]
        public async Task ActivatePlanAsync_ShouldSyncModules_WhenPlanActivated()
        {
            // Arrange
            var tenantId = Guid.NewGuid();
            var planId = Guid.NewGuid();
            var moduleId1 = Guid.NewGuid();
            var moduleId2 = Guid.NewGuid();

            var userContext = new AuthenticatedUser { TenantId = tenantId };
            _userContextMock.Setup(x => x.CurrentUser).Returns(userContext);

            var plan = new PlanEntity { Id = planId, IsTrial = true, TrialPeriodDays = 14 };
            _planRepositoryMock.Setup(x => x.GetByIdAsync(planId)).ReturnsAsync(plan);

            var subscription = new SubscriptionEntity { Id = Guid.NewGuid(), TenantId = tenantId, PlanId = planId };
            _subscriptionRepositoryMock.Setup(x => x.GetByTenantIdAsync(tenantId)).ReturnsAsync(subscription);

            var tenant = new TenantEntity { Id = tenantId };
            _tenantRepositoryMock.Setup(x => x.GetByIdAsync(tenantId)).ReturnsAsync(tenant);

            var planModules = new List<PlanModuleEntity>
            {
                new PlanModuleEntity { PlanId = planId, ModuleId = moduleId1 },
                new PlanModuleEntity { PlanId = planId, ModuleId = moduleId2 }
            };
            _planModuleRepositoryMock.Setup(x => x.GetByPlanIdAsync(planId)).ReturnsAsync(planModules);

            // Act
            var result = await _service.ActivatePlanAsync(planId);

            // Assert
            result.Succeeded.Should().BeTrue();
            _tenantModuleRepositoryMock.Verify(x => x.RemoveByTenantIdAsync(tenantId), Times.Once);
            _tenantModuleRepositoryMock.Verify(x => x.AddRangeAsync(It.Is<IEnumerable<TenantModuleEntity>>(
                m => m.Count() == 2 && 
                     m.Any(tm => tm.ModuleId == moduleId1) && 
                     m.Any(tm => tm.ModuleId == moduleId2) &&
                     m.All(tm => tm.TenantId == tenantId)
            )), Times.Once);
        }
    }
}
