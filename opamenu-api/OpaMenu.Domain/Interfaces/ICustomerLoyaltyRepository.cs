using System;
using System.Threading.Tasks;
using OpaMenu.Infrastructure.Shared.Entities;

namespace OpaMenu.Domain.Interfaces;

public interface ICustomerLoyaltyRepository : IRepository<CustomerLoyaltyBalanceEntity>
{
    Task<CustomerLoyaltyBalanceEntity?> GetByCustomerAndTenantAsync(Guid customerId, Guid tenantId);
    Task AddTransactionAsync(LoyaltyTransactionEntity transaction);
}
