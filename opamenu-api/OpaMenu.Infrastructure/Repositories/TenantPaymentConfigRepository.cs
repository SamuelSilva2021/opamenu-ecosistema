using Microsoft.EntityFrameworkCore;
using OpaMenu.Domain.Interfaces;
using OpaMenu.Infrastructure.Shared.Data.Context.Opamenu;
using OpaMenu.Infrastructure.Shared.Entities.Opamenu;
using OpaMenu.Infrastructure.Shared.Enums.Opamenu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpaMenu.Infrastructure.Repositories
{
    public class TenantPaymentConfigRepository(OpamenuDbContext context) : OpamenuRepository<TenantPaymentConfigEntity>(context),  ITenantPaymentConfigRepository
    {
        public async Task<IReadOnlyCollection<TenantPaymentConfigEntity>> GetActiveConfigsAsync(Guid tenantId)
        {
            return await _dbSet
             .Where(x => x.TenantId == tenantId && x.IsActive)
             .Select(x => new TenantPaymentConfigEntity
             {
                 TenantId = x.TenantId,
                 PaymentMethod = x.PaymentMethod,
                 Provider = x.Provider,
                 IsSandbox = x.IsSandbox
             })
             .ToListAsync();
        }

        public async Task<TenantPaymentConfigEntity?> GetActivePixConfigAsync(Guid tenantId)
        {
            return await _dbSet.Where(
               x => x.TenantId == tenantId &&
               x.PaymentMethod == EPaymentMethod.Pix &&
               x.IsActive
               ).Select(x => new TenantPaymentConfigEntity
               {
                   TenantId = tenantId,
                   PaymentMethod = EPaymentMethod.Pix,
                   Provider = x.Provider,
                   IsActive = x.IsActive,
                   ClientId = x.ClientId,
                   IsSandbox = x.IsSandbox,
                   ClientSecret = x.ClientSecret,
                   PublicKey = x.PublicKey,
                   AccessToken = x.AccessToken,
                   PixKey = x.PixKey,
               })
               .FirstOrDefaultAsync();
        }
    }
}
