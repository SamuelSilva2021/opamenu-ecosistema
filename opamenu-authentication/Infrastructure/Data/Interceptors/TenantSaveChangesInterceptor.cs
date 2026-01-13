using OpaMenu.Infrastructure.Shared.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Authenticator.API.Infrastructure.Data.Interceptors
{
    /// <summary>
    /// Interceptor para garantir que operaÃ§Ãµes de escrita respeitem o TenantId do contexto
    /// </summary>
    public class TenantSaveChangesInterceptor : SaveChangesInterceptor
    {
        private readonly ITenantContext _tenantContext;

        public TenantSaveChangesInterceptor(ITenantContext tenantContext)
        {
            _tenantContext = tenantContext;
        }

        public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
        {
            ApplyTenantRules(eventData.Context);
            return base.SavingChanges(eventData, result);
        }

        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
        {
            ApplyTenantRules(eventData.Context);
            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        private void ApplyTenantRules(DbContext? context)
        {
            if (context == null || !_tenantContext.HasTenant || !_tenantContext.TenantId.HasValue)
                return;

            var tenantId = _tenantContext.TenantId!.Value;

            foreach (var entry in context.ChangeTracker.Entries())
            {
                if (entry.State is not EntityState.Added and not EntityState.Modified)
                    continue;

                var tenantProp = entry.Properties.FirstOrDefault(p => p.Metadata.Name == "TenantId");
                if (tenantProp == null)
                    continue;

                // Em criaÃ§Ã£o, define o TenantId se nÃ£o informado
                if (entry.State == EntityState.Added)
                {
                    if (tenantProp.CurrentValue is null || (tenantProp.CurrentValue is Guid g && g == Guid.Empty))
                    {
                        tenantProp.CurrentValue = tenantId;
                    }
                    else if (tenantProp.CurrentValue is Guid existing && existing != tenantId)
                    {
                        throw new InvalidOperationException("Tentativa de criar entidade com TenantId diferente do contexto atual");
                    }
                }

                // Em atualizaÃ§Ã£o, impede alteraÃ§Ã£o cross-tenant
                if (entry.State == EntityState.Modified)
                {
                    if (tenantProp.CurrentValue is Guid current && current != tenantId)
                    {
                        throw new InvalidOperationException("OperaÃ§Ã£o de atualizaÃ§Ã£o cross-tenant nÃ£o permitida");
                    }
                }
            }
        }
    }
}
