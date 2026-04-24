namespace OpaMenu.Infrastructure.Services
{
    public class StripeRefundResult
    {
        public bool Success { get; set; }
        public string RefundId { get; set; } = string.Empty;
        public string PaymentIntentId { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Status { get; set; } = string.Empty;
        public string Reason { get; set; } = string.Empty;
        public DateTime ProcessedAt { get; set; }
        public string? FailureMessage { get; set; }
    }
}
