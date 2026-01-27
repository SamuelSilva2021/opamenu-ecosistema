using OpaMenu.Infrastructure.Shared.Entities.Opamenu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpaMenu.Domain.Interfaces
{
    public interface ITenantPaymentConfigRepository : IRepository<TenantPaymentConfigEntity>
    {
        /// <summary>
        /// Retorna a configuração PIX ativa do tenant, se existir.
        /// </summary>
        Task<TenantPaymentConfigEntity?> GetActivePixConfigAsync(Guid tenantId);

        /// <summary>
        /// Retorna todas as configurações de pagamento ativas do tenant.
        /// (futuro: cartão, wallet, etc)
        /// </summary>
        Task<IReadOnlyCollection<TenantPaymentConfigEntity>> GetActiveConfigsAsync(Guid tenantId);
    }

}
