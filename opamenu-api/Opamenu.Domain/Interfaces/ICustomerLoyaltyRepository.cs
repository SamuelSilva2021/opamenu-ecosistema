using System;
using System.Threading.Tasks;
using Opamenu.Infrastructure.Shared.Entities;
using Opamenu.Infrastructure.Shared.Entities.Opamenu;
using Opamenu.Infrastructure.Shared.Enums.Opamenu;

namespace Opamenu.Domain.Interfaces;

public interface ICustomerLoyaltyRepository : IRepository<CustomerLoyaltyBalanceEntity>
{
    Task<CustomerLoyaltyBalanceEntity?> GetByCustomerAndTenantAsync(Guid customerId, Guid tenantId);
    Task<CustomerLoyaltyBalanceEntity?> GetByCustomerAndProgramAsync(Guid customerId, Guid programId);
    Task<IEnumerable<CustomerLoyaltyBalanceEntity>> GetAllBalancesAsync(Guid customerId, Guid tenantId);
    Task AddTransactionAsync(LoyaltyTransactionEntity transaction);
    Task<bool> TransactionExistsAsync(Guid orderId, ELoyaltyTransactionType type, Guid? programId = null);
}
