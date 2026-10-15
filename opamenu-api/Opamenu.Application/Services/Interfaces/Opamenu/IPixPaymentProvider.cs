using Opamenu.Domain.DTOs.Opamenu.Providers;
using Opamenu.Infrastructure.Shared.Entities.Opamenu;
using Opamenu.Infrastructure.Shared.Enums.Opamenu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Opamenu.Application.Services.Interfaces.Opamenu
{
    public interface IPixPaymentProvider
    {
        EPaymentProvider ProviderType { get; }
        Task<PixProviderResultDto> CreatePixAsync(PaymentEntity payment, string? notificationUrl = null);
        Task<WebhookPaymentResultDto> ProcessWebhookAsync(string payload, string signature);
    }

}
