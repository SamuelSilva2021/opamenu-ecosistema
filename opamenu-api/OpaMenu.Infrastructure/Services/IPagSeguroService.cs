namespace OpaMenu.Infrastructure.Services
{
    public interface IPagSeguroService
    {
        Task<PagSeguroPaymentResult> ProcessPayment(PagSeguroPaymentRequest request);
        Task<PagSeguroRefundResult> ProcessRefund(string transactionId, decimal amount);
        Task<PagSeguroPaymentStatus> GetPaymentStatus(string transactionId);
        Task<PagSeguroPixResult> GeneratePixPayment(decimal amount, string description);
    }
}
