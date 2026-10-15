namespace Opamenu.Domain.Interfaces;

public interface ITenantModuleGate
{
    bool Enforced { get; }

    Task<bool> HasModuleAsync(Guid tenantId, string moduleKey);
}
