using OpaMenu.Desktop.Models.DTOs.Tables;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OpaMenu.Desktop.Services.Interfaces;

public interface ITablesService
{
    Task<IReadOnlyList<TableFullDto>> GetTablesFullAsync(int pageNumber = 1, int pageSize = 200);
}
