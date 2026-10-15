using System.Threading.Tasks;
using Opamenu.Desktop.Models.DTOs.Pdv;
using Opamenu.Desktop.Models.Enums;

namespace Opamenu.Desktop.Services.Interfaces;

public interface ICashRegisterService
{
    Task<CashShiftDto?> GetActiveShiftAsync();
    Task<CashShiftSummaryDto?> GetActiveShiftSummaryAsync();
    Task<CashShiftDto> OpenShiftAsync(decimal openingBalance);
    Task<CashShiftDto> CloseShiftAsync(decimal closingBalance);
    Task<CashShiftCloseSummaryDto> CloseShiftWithSummaryAsync(decimal closingBalance, string? justification);
    Task<CashMovementDto> AddMovementAsync(ECashMovementType type, decimal amount, string description);
}
