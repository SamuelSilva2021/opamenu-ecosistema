namespace OpaMenu.Infrastructure.Services
{
    public class PagSeguroRefundResult
    {
        public bool Success { get; set; }
        public string RefundId { get; set; } = string.Empty;
        public string OriginalTransactionId { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime ProcessedAt { get; set; }
        public string? FailureReason { get; set; }
    }
}
