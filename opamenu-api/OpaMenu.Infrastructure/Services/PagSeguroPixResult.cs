namespace OpaMenu.Infrastructure.Services
{
    public class PagSeguroPixResult
    {
        public bool Success { get; set; }
        public string TransactionId { get; set; } = string.Empty;
        public string QRCode { get; set; } = string.Empty;
        public string QRCodeImage { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateTime ExpiresAt { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? FailureReason { get; set; }
    }
}
