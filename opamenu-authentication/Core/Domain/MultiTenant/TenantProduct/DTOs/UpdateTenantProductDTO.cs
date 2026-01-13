using OpaMenu.Infrastructure.Shared.Entities.MultiTenant.TenantProduct;
using OpaMenu.Infrastructure.Shared.Enums.MultiTenant;
using System.ComponentModel.DataAnnotations;

namespace Authenticator.API.Core.Domain.MultiTenant.TenantProduct.DTOs
{
    public class UpdateTenantProductDTO
    {
        [MaxLength(255)]
        public string? Name { get; set; }

        [MaxLength(100)]
        public string? Slug { get; set; }

        public string? Description { get; set; }

        public ETenantProductCategory Category { get; set; }

        [MaxLength(20)]
        public string? Version { get; set; }

        public EProductStatus? Status { get; set; }

        public string? ConfigurationSchema { get; set; }

        public ETenantProductPricingModel PricingModel { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? BasePrice { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? SetupFee { get; set; }
    }
}

