namespace Authenticator.API.Core.Domain.MultiTenant.TenantProduct.DTOs
{
    public class TenantProductConfigurationDTO
    {
        public Guid ProductId { get; set; }
        public string ConfigurationSchema { get; set; } = string.Empty;
        public Dictionary<string, object> DefaultConfiguration { get; set; } = new();
        public bool RequiresConfiguration { get; set; }
    }
}
