using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using OpaMenu.Desktop.Models.Enums;
using OpaMenu.Desktop.Models.Printing;

namespace OpaMenu.Desktop.Services.Interfaces;

public interface IPrinterConfigurationService
{
    Task<IReadOnlyList<string>> GetInstalledWindowsPrintersAsync(CancellationToken cancellationToken = default);
    Task<PrinterMapping?> GetMappingAsync(EPrintDestination destination, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<PrinterMapping>> GetMappingsAsync(CancellationToken cancellationToken = default);
    Task UpsertMappingAsync(PrinterMapping mapping, CancellationToken cancellationToken = default);
}

