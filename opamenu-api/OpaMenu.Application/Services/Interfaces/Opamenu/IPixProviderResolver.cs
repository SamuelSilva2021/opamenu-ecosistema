using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpaMenu.Application.Services.Interfaces.Opamenu
{
    public interface IPixProviderResolver
    {
        Task<IPixPaymentProvider> ResolvePixProviderAsync(Guid tenantId);
    }
}
