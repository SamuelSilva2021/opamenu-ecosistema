namespace OpaMenu.Infrastructure.Services
{
    public class PagSeguroPaymentRequest
    {
        public decimal Amount { get; set; }
        public string CardToken { get; set; } = string.Empty;
        public string CardHolderName { get; set; } = string.Empty;
        public string CardHolderDocument { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int Installments { get; set; } = 1;
        public Dictionary<string, string> Customer { get; set; } = new();
    }
}
