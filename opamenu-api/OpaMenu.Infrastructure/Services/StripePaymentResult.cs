namespace OpaMenu.Infrastructure.Services
{
    public class StripePaymentResult
    {
        public bool Success { get; set; }
        public string PaymentIntentId { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Currency { get; set; } = string.Empty;
        public DateTime ProcessedAt { get; set; }
        public string? FailureCode { get; set; }
        public string? FailureMessage { get; set; }
    }
}
