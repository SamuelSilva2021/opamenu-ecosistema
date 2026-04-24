namespace OpaMenu.Infrastructure.Services
{
    public interface IStripeService
    {
        Task<StripePaymentResult> ProcessPayment(StripePaymentRequest request);
        Task<StripeRefundResult> ProcessRefund(string paymentIntentId, decimal amount, string reason);
        Task<StripePaymentStatus> GetPaymentStatus(string paymentIntentId);
        Task<string> CreatePaymentIntent(decimal amount, string currency = "brl");
    }
}
