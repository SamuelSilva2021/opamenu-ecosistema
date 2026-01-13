using OpaMenu.Infrastructure.Shared.Entities.MultiTenant.TenantProduct;
using OpaMenu.Infrastructure.Shared.Enums.MultiTenant;
namespace Authenticator.API.Core.Domain.MultiTenant.TenantProduct.DTOs
{
    public class TenantProductSummaryDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public ETenantProductCategory Category { get; set; } = ETenantProductCategory.WebApp;
        public string Version { get; set; } = "1.0.0";
        public EProductStatus Status { get; set; } = EProductStatus.Ativo;
        public ETenantProductPricingModel PricingModel { get; set; } = ETenantProductPricingModel.Assinatura;
        public decimal BasePrice { get; set; } = 0.00m;
        public DateTime CreatedAt { get; set; }
    }
}

