using OpaMenu.Infrastructure.Shared.Entities.MultiTenant.Tenant;

namespace Authenticator.API.Core.Domain.MultiTenant.Tenant.DTOs
{
    public class TenantFilterDTO
    {
        public string? Name { get; set; }
        public string? Slug { get; set; }
        public string? Domain { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public ETenantStatus? Status { get; set; }
    }
}
