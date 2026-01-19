using System.ComponentModel.DataAnnotations;
using OpaMenu.Infrastructure.Shared.Entities.MultiTenant.Tenant;

namespace Authenticator.API.Core.Domain.MultiTenant.Tenant.DTOs
{
    public class UpdateTenantDTO
    {
        [MaxLength(255)]
        public string? Name { get; set; }

        [MaxLength(100)]
        public string? Slug { get; set; }

        [MaxLength(255)]
        public string? Domain { get; set; }

        public ETenantStatus Status { get; set; }

        [MaxLength(18)]
        public string? CnpjCpf { get; set; }

        [MaxLength(255)]
        public string? RazaoSocial { get; set; }

        [MaxLength(50)]
        public string? InscricaoEstadual { get; set; }

        [MaxLength(50)]
        public string? InscricaoMunicipal { get; set; }

        [MaxLength(20)]
        public string? Phone { get; set; }

        [MaxLength(255)]
        [EmailAddress]
        public string? Email { get; set; }

        [MaxLength(255)]
        [Url]
        public string? Website { get; set; }

        // Endereço principal
        [MaxLength(255)]
        public string? AddressStreet { get; set; }

        [MaxLength(20)]
        public string? AddressNumber { get; set; }

        [MaxLength(100)]
        public string? AddressComplement { get; set; }

        [MaxLength(100)]
        public string? AddressNeighborhood { get; set; }

        [MaxLength(100)]
        public string? AddressCity { get; set; }

        [MaxLength(2)]
        public string? AddressState { get; set; }

        [MaxLength(10)]
        public string? AddressZipcode { get; set; }

        [MaxLength(2)]
        public string? AddressCountry { get; set; }

        // Endereço de cobrança
        [MaxLength(255)]
        public string? BillingStreet { get; set; }

        [MaxLength(20)]
        public string? BillingNumber { get; set; }

        [MaxLength(100)]
        public string? BillingComplement { get; set; }

        [MaxLength(100)]
        public string? BillingNeighborhood { get; set; }

        [MaxLength(100)]
        public string? BillingCity { get; set; }

        [MaxLength(2)]
        public string? BillingState { get; set; }

        [MaxLength(10)]
        public string? BillingZipcode { get; set; }

        [MaxLength(2)]
        public string? BillingCountry { get; set; }

        // Responsável legal
        [MaxLength(255)]
        public string? LegalRepresentativeName { get; set; }

        [MaxLength(14)]
        public string? LegalRepresentativeCpf { get; set; }

        [MaxLength(255)]
        [EmailAddress]
        public string? LegalRepresentativeEmail { get; set; }

        [MaxLength(20)]
        public string? LegalRepresentativePhone { get; set; }

        // Configurações
        public Dictionary<string, object>? Settings { get; set; }
    }
}
