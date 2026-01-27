using OpaMenu.Infrastructure.Shared.Enums.Opamenu;
using System;

namespace OpaMenu.Domain.DTOs.Opamenu.Providers;

public class WebhookPaymentResultDto
{
    public string ProviderPaymentId { get; set; } = string.Empty;
    public EPaymentStatus NewStatus { get; set; }
    public decimal PaidAmount { get; set; }
    public DateTime? PaidAt { get; set; }
    public string RawResponse { get; set; } = string.Empty;
}
