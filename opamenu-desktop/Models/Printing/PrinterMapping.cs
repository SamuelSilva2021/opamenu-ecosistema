using OpaMenu.Desktop.Models.Enums;

namespace OpaMenu.Desktop.Models.Printing;

public sealed record PrinterMapping(
    EPrintDestination Destination,
    EPrinterConnectionType ConnectionType,
    EPrinterPaperSize PaperSize,
    EPrinterProfile Profile,
    string? WindowsPrinterName,
    string? IpAddress,
    int? Port
);

