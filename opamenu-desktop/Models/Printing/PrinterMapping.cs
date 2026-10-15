using Opamenu.Desktop.Models.Enums;

namespace Opamenu.Desktop.Models.Printing;

public sealed record PrinterMapping(
    EPrintDestination Destination,
    EPrinterConnectionType ConnectionType,
    EPrinterPaperSize PaperSize,
    EPrinterProfile Profile,
    string? WindowsPrinterName,
    string? IpAddress,
    int? Port
);

