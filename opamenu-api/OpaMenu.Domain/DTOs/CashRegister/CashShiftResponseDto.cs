using OpaMenu.Infrastructure.Shared.Enums.Opamenu;

namespace OpaMenu.Domain.DTOs.CashRegister;

public class CashShiftResponseDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string? UserName { get; set; }
    public DateTime OpenedAt { get; set; }
    public DateTime? ClosedAt { get; set; }
    public decimal OpeningBalance { get; set; }
    public decimal? ClosingBalance { get; set; }
    public decimal ExpectedBalance { get; set; }
    public ECashShiftStatus Status { get; set; }
    public List<CashMovementResponseDto> Movements { get; set; } = new();
}
