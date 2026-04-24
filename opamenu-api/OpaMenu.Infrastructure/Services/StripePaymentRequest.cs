namespace OpaMenu.Infrastructure.Services
{
    public class StripePaymentRequest
    {
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "brl";
        public string CardToken { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string CustomerEmail { get; set; } = string.Empty;
        public Dictionary<string, string> Metadata { get; set; } = new();
    }
}
