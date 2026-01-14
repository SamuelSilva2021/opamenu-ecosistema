using Authenticator.API.Core.Domain.MultiTenant.Subscriptions.DTOs;
using OpaMenu.Infrastructure.Shared.Entities.MultiTenant.Tenant;

namespace Authenticator.API.Core.Domain.MultiTenant.Tenant.DTOs
{
    public class TenantDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string? Domain { get; set; }
        public ETenantStatus Status { get; set; } = ETenantStatus.Ativo;
        public string? CnpjCpf { get; set; }
        public string? RazaoSocial { get; set; }
        public string? InscricaoEstadual { get; set; }
        public string? InscricaoMunicipal { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? Website { get; set; }

        // Endereço
        public string? AddressStreet { get; set; }
        public string? AddressNumber { get; set; }
        public string? AddressComplement { get; set; }
        public string? AddressNeighborhood { get; set; }
        public string? AddressCity { get; set; }
        public string? AddressState { get; set; }
        public string? AddressZipcode { get; set; }
        public string AddressCountry { get; set; } = "BR";

        // Endereço de cobrança
        public string? BillingStreet { get; set; }
        public string? BillingNumber { get; set; }
        public string? BillingComplement { get; set; }
        public string? BillingNeighborhood { get; set; }
        public string? BillingCity { get; set; }
        public string? BillingState { get; set; }
        public string? BillingZipcode { get; set; }
        public string BillingCountry { get; set; } = "BR";

        // Responsável legal
        public string? LegalRepresentativeName { get; set; }
        public string? LegalRepresentativeCpf { get; set; }
        public string? LegalRepresentativeEmail { get; set; }
        public string? LegalRepresentativePhone { get; set; }

        // Assinatura
        public Guid? ActiveSubscriptionId { get; set; }
        public SubscriptionDTO? ActiveSubscription { get; set; }

        // Datas
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // Configurações
        public Dictionary<string, object> Settings { get; set; } = new();
    }
}
