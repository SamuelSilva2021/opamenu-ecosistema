using OpaMenu.Application.Services.Interfaces.Opamenu;
using OpaMenu.Domain.DTOs.Opamenu.Providers;
using OpaMenu.Infrastructure.Shared.Entities.Opamenu;
using OpaMenu.Infrastructure.Shared.Enums.Opamenu;
using System;
using System.Threading.Tasks;
using OpaMenu.Infrastructure.Services;

namespace OpaMenu.Infrastructure.Services.PaymentProviders;

public class GerencianetPixProvider(TenantPaymentConfigEntity config, IPixService pixService) : IPixPaymentProvider
{
    private readonly TenantPaymentConfigEntity _config = config;
    private readonly IPixService _pixService = pixService;

    public EPaymentProvider ProviderType => EPaymentProvider.Gerencianet;

    public async Task<PixProviderResultDto> CreatePixAsync(PaymentEntity payment)
    {
        // Geração de PIX Estático (Copia e Cola) usando a chave configurada
        // Em produção, integraria com SDK da Gerencianet para PIX Dinâmico
        
        var transactionId = Guid.NewGuid().ToString("N")[..25];
        var payload = _pixService.GeneratePixPayload(payment.Amount, $"Pedido {payment.OrderId}", _config.PixKey, transactionId);

        // Simulação
        await Task.Delay(100);

        return new PixProviderResultDto
        {
            Provider = EPaymentProvider.Gerencianet.ToString(),
            ProviderPaymentId = transactionId, 
            QrCode = payload,
            QrCodeBase64 = null,
            ExpiresAt = DateTime.UtcNow.AddMinutes(30),
            Amount = payment.Amount,
            Currency = "BRL"
        };
    }

    public Task<WebhookPaymentResultDto> ProcessWebhookAsync(string payload, string signature)
    {
        // TODO: Validar certificado/assinatura se necessário (Gerencianet usa mTLS geralmente, mas pode ter assinatura no body)
        
        // Simulação de parsing do payload da Gerencianet
        // { "pix": [ { "txid": "...", "valor": "..." } ] }

        return Task.FromResult(new WebhookPaymentResultDto
        {
            ProviderPaymentId = "simulated-gn-id", // Deveria extrair do payload
            NewStatus = EPaymentStatus.Paid,
            PaidAmount = 0,
            PaidAt = DateTime.UtcNow,
            RawResponse = payload
        });
    }
}
