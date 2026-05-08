namespace OpaMenu.Infrastructure.Shared.Entities.MultiTenant.Tenant
{
    /// <summary>
    /// Define o tipo do tenant no sistema
    /// </summary>
    public enum ETenantType
    {
        /// <summary>
        /// Cliente final (restaurante, lanchonete, etc.)
        /// </summary>
        Cliente = 0,

        /// <summary>
        /// Revendedor (gerencia múltiplos sub-tenants)
        /// </summary>
        Revendedor = 1
    }
}
