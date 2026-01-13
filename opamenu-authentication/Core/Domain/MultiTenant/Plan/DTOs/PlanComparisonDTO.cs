namespace Authenticator.API.Core.Domain.MultiTenant.Plan.DTOs
{
    public class PlanComparisonDTO
    {
        public Guid PlanId { get; set; }
        public string PlanName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string BillingCycle { get; set; } = "monthly";
        public int MaxUsers { get; set; }
        public int MaxStorageGb { get; set; }
        public bool Popular { get; set; }
        public Dictionary<string, bool> FeatureMatrix { get; set; } = new();
        public decimal PricePerUser => MaxUsers > 0 ? Price / MaxUsers : Price;
        public string ValueTier { get; set; } = "standard"; // basic, standard, premium, enterpris

    }
}
