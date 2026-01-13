namespace Authenticator.API.Core.Domain.MultiTenant.Plan.DTOs
{
    public class PlanPriceCalculationDTO
    {
        public Guid PlanId { get; set; }
        public int AdditionalUsers { get; set; }
        public int AdditionalStorageGb { get; set; }
        public string Currency { get; set; } = "BRL";
        public string BillingCycle { get; set; } = "monthly";
        public bool ProRated { get; set; } = false;
        public DateTime? StartDate { get; set; }

        // Resultados
        public decimal BasePrice { get; set; }
        public decimal AdditionalUsersPrice { get; set; }
        public decimal AdditionalStoragePrice { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal FinalPrice { get; set; }
    }
}
