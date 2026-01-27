using OpaMenu.Domain.DTOs.Opamenu.Providers;
using OpaMenu.Infrastructure.Shared.Entities.Opamenu;
using OpaMenu.Infrastructure.Shared.Enums.Opamenu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpaMenu.Application.Services.Interfaces.Opamenu
{
    public interface IPixPaymentProvider
    {
        EPaymentProvider ProviderType { get; }
        Task<PixProviderResultDto> CreatePixAsync(PaymentEntity payment);
        Task<WebhookPaymentResultDto> ProcessWebhookAsync(string payload, string signature);
    }

}
