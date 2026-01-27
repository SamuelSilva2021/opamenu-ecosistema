using OpaMenu.Commons.Api.DTOs;
using OpaMenu.Domain.DTOs.Payments;
using OpaMenu.Infrastructure.Shared.Entities;
using OpaMenu.Infrastructure.Shared.Enums.Opamenu;

namespace OpaMenu.Application.Services.Interfaces.Opamenu;

/// <summary>
/// Interface para serviço de pagamento.
/// </summary>
public interface IPaymentService
{
    Task<IEnumerable<PaymentResponseDto>> GetPaymentsAsync();
    Task<PaymentStatusDto> GetPaymentStatusAsync(Guid paymentId);

    Task<ResponseDTO<PixResponseDto>> GeneratePixAsync(PixRequestDto request, Guid? tenantId = null);
    Task<ResponseDTO<bool>> ProcessWebhookAsync(Guid tenantId, string provider, string payload, string signature);
}


