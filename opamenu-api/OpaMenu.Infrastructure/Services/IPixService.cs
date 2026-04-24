namespace OpaMenu.Infrastructure.Services
{
    public interface IPixService
    {
        Task<PixQRCodeResponse> GenerateQRCode(decimal amount, string description, string orderId);
        Task<PixValidationResult> ValidatePixPayment(string transactionId);
        string GeneratePixPayload(decimal amount, string description, string merchantKey, string? transactionId = null);
    }
}
