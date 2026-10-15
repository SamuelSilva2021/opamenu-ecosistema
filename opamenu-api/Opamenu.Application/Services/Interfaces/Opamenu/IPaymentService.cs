using Opamenu.Commons.Api.DTOs;
using Opamenu.Domain.DTOs.Payments;
using Opamenu.Infrastructure.Shared.Entities;
using Opamenu.Infrastructure.Shared.Enums.Opamenu;

namespace Opamenu.Application.Services.Interfaces.Opamenu;

/// <summary>
/// Interface para serviço de pagamento.
/// </summary>
public interface IPaymentService
{
    Task<IEnumerable<PaymentResponseDto>> GetPaymentsAsync();
    Task<PaymentStatusDto> GetPaymentStatusAsync(Guid paymentId);

    Task<ResponseDTO<PixResponseDto>> GeneratePixAsync(PixRequestDto request, Guid? tenantId = null, string? notificationUrl = null);
    Task<ResponseDTO<bool>> ProcessWebhookAsync(Guid tenantId, string provider, string payload, string signature);
}


