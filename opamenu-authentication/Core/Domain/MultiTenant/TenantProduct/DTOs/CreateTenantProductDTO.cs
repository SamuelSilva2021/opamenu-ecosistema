using OpaMenu.Infrastructure.Shared.Entities.MultiTenant.TenantProduct;
using OpaMenu.Infrastructure.Shared.Enums.MultiTenant;
using System.ComponentModel.DataAnnotations;

namespace Authenticator.API.Core.Domain.MultiTenant.TenantProduct.DTOs
{
    public class CreateTenantProductDTO
    {
        [Required]
        [MaxLength(255)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Slug { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Required]
        public ETenantProductCategory Category { get; set; } = ETenantProductCategory.WebApp;

        [MaxLength(20)]
        public string Version { get; set; } = "1.0.0";

        public EProductStatus Status { get; set; } = EProductStatus.Ativo;

        public string? ConfigurationSchema { get; set; }

        public ETenantProductPricingModel PricingModel { get; set; } = ETenantProductPricingModel.Assinatura;

        [Range(0, double.MaxValue)]
        public decimal BasePrice { get; set; } = 0.00m;

        [Range(0, double.MaxValue)]
        public decimal SetupFee { get; set; } = 0.00m;
    }
}

