using Opamenu.Infrastructure.Shared.Enums.Opamenu;

namespace Opamenu.Domain.DTOs.Tab;

public class TabCheckoutRequestDto
{
    public EPaymentMethod PaymentMethod { get; set; }
}
