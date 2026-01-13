using Microsoft.EntityFrameworkCore;
using OpaMenu.Infrastructure.Shared.Entities;
using OpaMenu.Domain.Interfaces;
using OpaMenu.Infrastructure.Shared.Data.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpaMenu.Infrastructure.Repositories
{
    public class CustomerRepository(OpamenuDbContext context) : ICustomerRepository
    {
        private readonly OpamenuDbContext _context = context;

        public async Task<CustomerEntity?> CreateAsync(CustomerEntity entity)
        {
            var createdEntity = await _context.Customers.AddAsync(entity);
            await _context.SaveChangesAsync();
            return createdEntity.Entity;
        }

        public async Task<IEnumerable<CustomerEntity>> GetByTenantIdAsync(Guid tenantId) =>
            await _context.Customers.Where(c => c.TenantCustomers.Any(tc => tc.TenantId == tenantId)).ToListAsync();

        public async Task<CustomerEntity?> GetByPhoneAsync(Guid tenantId, string phone) =>
             await _context.Customers
                .Include(c => c.TenantCustomers)
                .FirstOrDefaultAsync(c =>
                    c.Phone == phone &&
                    c.TenantCustomers.Any(tc => tc.TenantId == tenantId));

        public async Task<CustomerEntity?> UpdateAsync(CustomerEntity entity)
        {
            var updatedEntity = _context.Customers.Update(entity);
            await _context.SaveChangesAsync();
            return updatedEntity.Entity;
        }

        public async Task<CustomerEntity?> GetByIdAsync(Guid id) => await _context.Customers.FindAsync(id);
    }
}

