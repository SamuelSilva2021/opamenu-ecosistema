using OpaMenu.Infrastructure.Shared.Enums.Opamenu;

namespace OpaMenu.Domain.DTOs.Tab;

public class TabCheckoutRequestDto
{
    public EPaymentMethod PaymentMethod { get; set; }
}
