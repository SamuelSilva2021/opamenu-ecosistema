using Microsoft.Extensions.Logging;
using OpaMenu.Application.Services.Interfaces.Opamenu;
using OpaMenu.Commons.Api.Commons;
using OpaMenu.Commons.Api.DTOs;
using OpaMenu.Domain.DTOs.Payments;
using OpaMenu.Domain.Interfaces;
using OpaMenu.Infrastructure.Shared.Entities.Opamenu;

namespace OpaMenu.Application.Services.Opamenu;

/// <summary>
/// Serviço para gerenciamento de pagamentos
/// </summary>
public class PaymentService(
    IRepository<PaymentEntity> paymentRepository,
    ILogger<PaymentService> logger, 
    ICurrentUserService currentUserService,
    IPixProviderResolver providerResolver,
    IRepository<OrderEntity> orderRepository
    ) : IPaymentService
{
    private readonly IRepository<PaymentEntity> _paymentRepository = paymentRepository;
    private readonly ILogger<PaymentService> _logger = logger;
    private readonly ICurrentUserService _currentUserService = currentUserService;
    private readonly IPixProviderResolver _providerResolver = providerResolver;
    private readonly IRepository<OrderEntity> _orderRepository = orderRepository;

    public async Task<IEnumerable<PaymentResponseDto>> GetPaymentsAsync()
    {
        var tenantId = _currentUserService.GetTenantGuid() ?? throw new Exception("Tenant não identificado");
        var payments = await _paymentRepository.GetAllAsync(tenantId);
        
        return payments.Select(p => new PaymentResponseDto
        {
            Id = p.Id,
            OrderId = p.OrderId,
            Amount = p.Amount,
            Status = p.Status,
            GatewayTransactionId = p.GatewayTransactionId,
            CreatedAt = p.CreatedAt,
            ProcessedAt = p.PaidAt,
            Notes = p.Notes
        });
    }

    public async Task<PaymentStatusDto> GetPaymentStatusAsync(Guid paymentId)
    {
        var tenantId = _currentUserService.GetTenantGuid() ?? throw new Exception("Tenant não identificado");
        var payment = await _paymentRepository.GetByIdAsync(paymentId, tenantId);
        
        if (payment == null) throw new ArgumentException("Pagamento não encontrado");
        
        return new PaymentStatusDto
        {
            PaymentId = payment.Id,
            Status = payment.Status.ToString(),
            Amount = payment.Amount,
            Method = payment.Method.ToString(),
            ProcessedAt = payment.PaidAt,
            GatewayTransactionId = payment.GatewayTransactionId
        };
    }

    public async Task<ResponseDTO<PixResponseDto>> GeneratePixAsync(PixRequestDto request, Guid? tenantId = null)
    {
        try
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            ///Buscar o tenantId pelo pedido
            tenantId ??= _currentUserService.GetTenantGuid() ?? throw new Exception("Tenant não identificado");
            
            ///Buscar o provedor do tenant
            var provider = await _providerResolver.ResolvePixProviderAsync(tenantId.Value);

            // Cria a entidade usando a Factory
            var payment = PaymentEntity.CreatePix(request.OrderId, request.Amount, tenantId.Value);

            // Carrega o pedido para obter dados do cliente para o provedor (Mercado Pago requer Email/Nome)
            var order = await _orderRepository.GetByIdAsync(request.OrderId, tenantId.Value);
            if (order != null)
            {
                payment.Order = order;
            }
            
            // TODO: Se tiver PaymentMethodId, setar aqui ou passar na factory. 
            // Por enquanto deixamos null pois o fluxo foca no Enum Method.

            payment.Provider = provider.ProviderType;

            await _paymentRepository.AddAsync(payment);

            // Chama o provedor
            var pixResult = await provider.CreatePixAsync(payment);

            // Anexa os dados do PIX na entidade
            payment.AttachPixData(
                pixResult.Provider, 
                pixResult.ProviderPaymentId, 
                pixResult.QrCode, 
                pixResult.QrCodeBase64, 
                pixResult.ExpiresAt
            );

            await _paymentRepository.UpdateAsync(payment);

            // Mapeia para o DTO de resposta
            var responseDto = new PixResponseDto
            {
                PixId = pixResult.ProviderPaymentId,
                QrCode = pixResult.QrCode,
                QrCodeBase64 = pixResult.QrCodeBase64 ?? string.Empty,
                Amount = pixResult.Amount,
                ExpiresAt = pixResult.ExpiresAt
            };

            return StaticResponseBuilder<PixResponseDto>.BuildOk(responseDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao gerar PIX");
            return StaticResponseBuilder<PixResponseDto>.BuildErrorResponse(ex);
        }
    }

    public async Task<ResponseDTO<bool>> ProcessWebhookAsync(Guid tenantId, string providerName, string payload, string signature)
    {
        try
        {
            _logger.LogInformation("Recebido Webhook. Tenant: {TenantId}, Provider: {Provider}", tenantId, providerName);

            var provider = await _providerResolver.ResolvePixProviderAsync(tenantId);
            var result = await provider.ProcessWebhookAsync(payload, signature);

            if (result == null)
            {
                return StaticResponseBuilder<bool>.BuildErrorResponse(new Exception("Falha ao processar webhook no provedor."));
            }

            _logger.LogInformation("Webhook processado pelo provedor. PaymentId: {PaymentId}, Status: {Status}", result.ProviderPaymentId, result.NewStatus);

            var payment = await _paymentRepository.FirstOrDefaultAsync(p => p.ProviderPaymentId == result.ProviderPaymentId && p.TenantId == tenantId);

            if (payment == null)
            {
                _logger.LogWarning("Pagamento não encontrado para ProviderPaymentId: {Id}", result.ProviderPaymentId);
                return StaticResponseBuilder<bool>.BuildErrorResponse(new Exception("Pagamento não encontrado."));
            }

            payment.ProcessWebhookResult(result.NewStatus, result.PaidAt, result.RawResponse);
            await _paymentRepository.UpdateAsync(payment);
            
            return StaticResponseBuilder<bool>.BuildOk(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao processar Webhook");
            return StaticResponseBuilder<bool>.BuildErrorResponse(ex);
        }
    }
}
