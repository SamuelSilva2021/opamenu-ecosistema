using Microsoft.EntityFrameworkCore;
using OpaMenu.Infrastructure.Shared.Entities;
using OpaMenu.Domain.Interfaces;
using OpaMenu.Infrastructure.Shared.Data.Context;
using OpaMenu.Infrastructure.Shared.Data.Context.Opamenu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpaMenu.Infrastructure.Shared.Entities.Opamenu;

namespace OpaMenu.Infrastructure.Repositories
{
    public class TenantCustomerRepository(OpamenuDbContext context) : ITenantCustomerRepository
    {
        private readonly OpamenuDbContext _context = context;

        public async Task<TenantCustomerEntity?> CreateAsync(TenantCustomerEntity entity)
        {
            var createdEntity = await _context.TenantCustomers.AddAsync(entity);
            await _context.SaveChangesAsync();
            return createdEntity.Entity;
        }

        public async Task<IEnumerable<TenantCustomerEntity>> GetByTenantIdAsync(Guid tenantId) =>
            await _context.TenantCustomers
                .Where(tc => tc.TenantId == tenantId)
                .Include(tc => tc.Customer)
                .ToListAsync();

        public async Task<TenantCustomerEntity?> GetByTenantIdAndCustomerIdAsync(Guid tenantId, Guid customerId) =>
            await _context.TenantCustomers
                .Include(tc => tc.Customer)
                .FirstOrDefaultAsync(tc => tc.TenantId == tenantId && tc.CustomerId == customerId);

        public async Task<TenantCustomerEntity?> GetByTenantIdAndCustomerPhoneAsync(Guid tenantId, string phoneNumber) =>
            await _context.TenantCustomers
            .Where(tc => tc.TenantId == tenantId && tc.Customer.Phone == phoneNumber)
            .Include(tc => tc.Customer)
            .FirstOrDefaultAsync();
    }
}

