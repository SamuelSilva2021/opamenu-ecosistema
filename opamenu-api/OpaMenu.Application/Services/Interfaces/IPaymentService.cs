using OpaMenu.Domain.DTOs.Payments;
using OpaMenu.Infrastructure.Shared.Entities;
using OpaMenu.Infrastructure.Shared.Enums.Opamenu;

namespace OpaMenu.Application.Services.Interfaces;

/// <summary>
/// Interface para serviço de pagamento.
/// </summary>
public interface IPaymentService
{
    /// <summary>
    /// Obter todos os pagamentos
    /// </summary>
    /// <returns>Lista de pagamentos</returns>
    Task<IEnumerable<PaymentResponseDto>> GetPaymentsAsync();
    
    /// <summary>
    /// Processar um pagamento
    /// </summary>
    /// <param name="request">Dados do pagamento</param>
    /// <returns>Resultado do processamento</returns>
    Task<PaymentResponseDto> ProcessPaymentAsync(PaymentRequestDto request);
    
    /// <summary>
    /// Gerar pagamento PIX
    /// </summary>
    /// <param name="request">Dados do PIX</param>
    /// <returns>Dados do PIX gerado</returns>
    Task<PixResponseDto> GeneratePixPaymentAsync(PixRequestDto request);
    
    /// <summary>
    /// Obter status de um pagamento
    /// </summary>
    /// <param name="paymentId">ID do pagamento</param>
    /// <returns>Status do pagamento</returns>
    Task<PaymentStatusDto> GetPaymentStatusAsync(Guid paymentId);
    
    /// <summary>
    /// Processar estorno de pagamento
    /// </summary>
    /// <param name="request">Dados do estorno</param>
    /// <returns>Resultado do estorno</returns>
    Task<RefundResponseDto> RefundPaymentAsync(RefundRequestDto request);
}


