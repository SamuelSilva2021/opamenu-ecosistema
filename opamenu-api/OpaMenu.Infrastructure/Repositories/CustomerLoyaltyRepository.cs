using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OpaMenu.Domain.Interfaces;
using OpaMenu.Infrastructure.Shared.Data.Context;
using OpaMenu.Infrastructure.Shared.Entities;

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
        // O SaveChanges é chamado pelo UnitOfWork ou Service
    }
}
