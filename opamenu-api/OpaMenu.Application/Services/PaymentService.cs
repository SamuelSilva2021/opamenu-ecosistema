using Microsoft.Extensions.Logging;
using OpaMenu.Application.Services.Interfaces;
using OpaMenu.Infrastructure.Shared.Entities;
using OpaMenu.Domain.Interfaces;
using OpaMenu.Infrastructure.Shared.Enums.Opamenu;
using OpaMenu.Domain.DTOs.Payments;

namespace OpaMenu.Application.Services;

/// <summary>
/// ServiÃ§o para gerenciamento de pagamentos seguindo princÃ­pios SOLID e Clean Architecture
/// </summary>
public class PaymentService(
    IRepository<PaymentEntity> paymentRepository,
    IRepository<PaymentRefundEntity> refundRepository,
    ILogger<PaymentService> logger, ICurrentUserService currentUserService) : IPaymentService
{
    private readonly IRepository<PaymentEntity> _paymentRepository = paymentRepository;
    private readonly IRepository<PaymentRefundEntity> _refundRepository = refundRepository;
    private readonly ILogger<PaymentService> _logger = logger;
    private readonly ICurrentUserService _currentUserService = currentUserService;

    /// <summary>
    /// Obter todos os pagamentos
    /// </summary>
    public async Task<IEnumerable<PaymentResponseDto>> GetPaymentsAsync()
    {
        try
        {
            var payments = await _paymentRepository.GetAllAsync(_currentUserService.GetTenantGuid()!.Value);
            return payments.Select(p => new PaymentResponseDto
            {
                Id = p.Id,
                OrderId = p.OrderId,
                Amount = p.Amount,
                Method = p.Method,
                Status = p.Status,
                GatewayTransactionId = p.GatewayTransactionId,
                CreatedAt = p.CreatedAt,
                ProcessedAt = p.ProcessedAt,
                Notes = p.Notes
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar pagamentos");
            throw;
        }
    }

    public async Task<PaymentResponseDto> ProcessPaymentAsync(PaymentRequestDto request)
    {
        try
        {
            _logger.LogInformation("Processando pagamento para pedido {OrderId}", request.OrderId);
            
            if (request.Amount <= 0)
                throw new ArgumentException("Valor do pagamento deve ser maior que zero");
            
            var payment = new PaymentEntity
            {
                OrderId = request.OrderId,
                Amount = request.Amount,
                Method = request.Method,
                Status = EPaymentStatus.Pending,
                Notes = request.Notes
            };
            
            // Simular processamento baseado no mÃ©todo
            switch (request.Method)
            {
                case EPaymentMethod.Cash:
                    payment.Status = EPaymentStatus.Approved;
                    payment.ProcessedAt = DateTime.UtcNow;
                    break;
                case EPaymentMethod.Pix:
                    payment.Status = EPaymentStatus.Processing;
                    break;
                default:
                    payment.Status = EPaymentStatus.Processing;
                    break;
            }
            
            var savedPayment = await _paymentRepository.AddAsync(payment);
            
            return new PaymentResponseDto
            {
                Id = savedPayment.Id,
                OrderId = savedPayment.OrderId,
                Amount = savedPayment.Amount,
                Method = savedPayment.Method,
                Status = savedPayment.Status,
                GatewayTransactionId = savedPayment.GatewayTransactionId,
                CreatedAt = savedPayment.CreatedAt,
                ProcessedAt = savedPayment.ProcessedAt,
                Notes = savedPayment.Notes
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao processar pagamento");
            throw;
        }
    }

    public async Task<PixResponseDto> GeneratePixPaymentAsync(PixRequestDto request)
    {
        try
        {
            _logger.LogInformation("Gerando PIX para pedido {OrderId}", request.OrderId);
            
            if (request.Amount <= 0)
                throw new ArgumentException("Valor do PIX deve ser maior que zero");
            
            var pixId = $"pix_{Guid.NewGuid().ToString("N")[..20]}";
            
            var payment = new PaymentEntity
            {
                OrderId = request.OrderId,
                Amount = request.Amount,
                Method = EPaymentMethod.Pix,
                Status = EPaymentStatus.Pending,
                GatewayTransactionId = pixId,
                Notes = "Pagamento PIX gerado"
            };
            
            await _paymentRepository.AddAsync(payment);
            
            return new PixResponseDto
            {
                PixId = pixId,
                QrCode = $"00020126580014br.gov.bcb.pix0136{pixId}520400005303986540{request.Amount:F2}5802BR6009SAO PAULO62070503***6304",
                QrCodeBase64 = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes($"PIX-{pixId}")),
                ExpiresAt = DateTime.UtcNow.AddMinutes(30),
                Amount = request.Amount
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao gerar PIX");
            throw;
        }
    }

    public async Task<PaymentStatusDto> GetPaymentStatusAsync(Guid paymentId)
    {
        try
        {
            _logger.LogInformation("Consultando status do pagamento {PaymentId}", paymentId);
            
            var payment = await _paymentRepository.GetByIdAsync(paymentId, _currentUserService.GetTenantGuid()!.Value);
            
            if (payment == null)
                throw new ArgumentException("Pagamento nÃ£o encontrado");
            
            return new PaymentStatusDto
            {
                PaymentId = payment.Id,
                Status = payment.Status.ToString(),
                Amount = payment.Amount,
                Method = payment.Method.ToString(),
                ProcessedAt = payment.ProcessedAt,
                GatewayTransactionId = payment.GatewayTransactionId
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao consultar status do pagamento");
            throw;
        }
    }

    public async Task<RefundResponseDto> RefundPaymentAsync(RefundRequestDto request)
    {
        try
        {
            _logger.LogInformation("Processando estorno do pagamento {PaymentId}", request.PaymentId);
            
            var payment = await _paymentRepository.GetByIdAsync(request.PaymentId, _currentUserService.GetTenantGuid()!.Value);
            
            if (payment == null)
                throw new ArgumentException("Pagamento nÃ£o encontrado");
            
            if (payment.Status != EPaymentStatus.Approved)
                throw new ArgumentException("Apenas pagamentos aprovados podem ser estornados");
            
            if (request.Amount <= 0 || request.Amount > payment.Amount)
                throw new ArgumentException("Valor do estorno invÃ¡lido");
            
            var refundId = $"refund_{Guid.NewGuid().ToString("N")[..20]}";
            
            var refund = new PaymentRefundEntity
            {
                PaymentId = payment.Id,
                Amount = request.Amount,
                Reason = request.Reason ?? "Estorno solicitado",
                RefundedBy = request.RefundedBy ?? "Sistema",
                GatewayRefundId = refundId,
                RefundedAt = DateTime.UtcNow
            };
            
            await _refundRepository.AddAsync(refund);
            
            if (request.Amount >= payment.Amount)
            {
                payment.Status = EPaymentStatus.Refunded;
                await _paymentRepository.UpdateAsync(payment);
            }
            
            return new RefundResponseDto
            {
                RefundId = refund.Id,
                PaymentId = payment.Id,
                Amount = refund.Amount,
                Reason = refund.Reason,
                RefundedAt = refund.RefundedAt,
                RefundedBy = refund.RefundedBy,
                GatewayRefundId = refund.GatewayRefundId
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao processar estorno");
            throw;
        }
    }
}
