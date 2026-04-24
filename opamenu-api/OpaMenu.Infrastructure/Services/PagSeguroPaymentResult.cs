namespace OpaMenu.Infrastructure.Services
{
    public class PagSeguroPaymentResult
    {
        public bool Success { get; set; }
        public string TransactionId { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateTime ProcessedAt { get; set; }
        public string? AuthorizationCode { get; set; }
        public string? FailureReason { get; set; }
    }
}
