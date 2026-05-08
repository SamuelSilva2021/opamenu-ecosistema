namespace OpaMenu.Infrastructure.Shared.Entities.MultiTenant.Plan
{
    /// <summary>
    /// Categoriza o plano de assinatura
    /// </summary>
    public enum EPlanCategory
    {
        /// <summary>
        /// Plano para o cliente final (restaurante, lanchonete, etc.)
        /// </summary>
        Customer = 0,

        /// <summary>
        /// Plano para revendedores (parceiros que gerenciam múltiplos sub-tenants)
        /// </summary>
        Reseller = 1
    }
}
