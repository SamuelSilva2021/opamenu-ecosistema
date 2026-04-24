namespace OpaMenu.Infrastructure.Services
{
    public class PixQRCodeResponse
    {
        public string QRCode { get; set; } = string.Empty;
        public string QRCodeImage { get; set; } = string.Empty;
        public string TransactionId { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string MerchantName { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
    }
}
