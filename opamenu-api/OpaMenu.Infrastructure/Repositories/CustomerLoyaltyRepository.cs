using Microsoft.EntityFrameworkCore;
using OpaMenu.Domain.Interfaces;
using OpaMenu.Infrastructure.Shared.Data.Context.Opamenu;
using OpaMenu.Infrastructure.Shared.Entities.Opamenu;
using OpaMenu.Infrastructure.Shared.Enums.Opamenu;

namespace OpaMenu.Infrastructure.Repositories;

public class CustomerLoyaltyRepository(OpamenuDbContext context) : OpamenuRepository<CustomerLoyaltyBalanceEntity>(context), ICustomerLoyaltyRepository
{
    public async Task<CustomerLoyaltyBalanceEntity?> GetByCustomerAndTenantAsync(Guid customerId, Guid tenantId)
    {
        return await _dbSet
            .Include(b => b.Transactions.OrderByDescending(t => t.CreatedAt).Take(20))
            .FirstOrDefaultAsync(b => b.CustomerId == customerId && b.TenantId == tenantId);
    }

    public async Task AddTransactionAsync(LoyaltyTransactionEntity transaction)
    {
        await _context.Set<LoyaltyTransactionEntity>().AddAsync(transaction);
    }

    public async Task<bool> TransactionExistsAsync(Guid orderId, ELoyaltyTransactionType type)
    {
        return await _context.Set<LoyaltyTransactionEntity>()
            .AnyAsync(t => t.OrderId == orderId && t.Type == type);
    }
}
