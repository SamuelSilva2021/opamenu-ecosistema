using OpaMenu.Infrastructure.Shared.Entities.MultiTenant.Tenant;

namespace Authenticator.API.Core.Domain.MultiTenant.Tenant.DTOs
{
    public class TenantSummaryDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string? Domain { get; set; }
        public ETenantStatus Status { get; set; } = ETenantStatus.Ativo;
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public Guid? ActiveSubscriptionId { get; set; }
    }
}
