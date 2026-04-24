namespace OpaMenu.Infrastructure.Services
{
    public class PixValidationResult
    {
        public bool IsValid { get; set; }
        public string Status { get; set; } = string.Empty;
        public string TransactionId { get; set; } = string.Empty;
        public DateTime ValidatedAt { get; set; }
        public decimal Amount { get; set; }
        public string PayerDocument { get; set; } = string.Empty;
    }
}
