using OpaMenu.Infrastructure.Shared.Enums.Opamenu;

namespace OpaMenu.Domain.DTOs.CashRegister;

public class AddCashMovementRequestDto
{
    public ECashMovementType Type { get; set; }
    public decimal Amount { get; set; }
    public string Description { get; set; } = string.Empty;
    public EPaymentMethod? PaymentMethod { get; set; }
    public Guid? OrderId { get; set; }
}
