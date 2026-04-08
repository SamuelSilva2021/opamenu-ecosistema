using OpaMenu.Desktop.Models.DTOs.Tables;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OpaMenu.Desktop.Services.Interfaces;

public interface ITablesService
{
    Task<IReadOnlyList<TableFullDto>> GetTablesFullAsync(int pageNumber = 1, int pageSize = 200);
    Task<TableFullDto?> GetTableFullByIdAsync(System.Guid tableId);
    Task CheckoutTabAsync(System.Guid tableId, System.Guid tabId, OpaMenu.Desktop.Models.Enums.EPaymentMethod paymentMethod);
}
