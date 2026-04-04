using System.Threading.Tasks;
using OpaMenu.Desktop.Models.DTOs;

namespace OpaMenu.Desktop.Services;

public interface ICashRegisterService
{
    Task<CashShiftDto?> GetActiveShiftAsync();
    Task<CashShiftDto> OpenShiftAsync(decimal openingBalance);
    Task<CashShiftDto> CloseShiftAsync(decimal closingBalance);
}
