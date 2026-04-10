using System;
using System.ComponentModel.DataAnnotations;
using OpaMenu.Desktop.Models.Enums;

namespace OpaMenu.Desktop.Models.Entities;

public class PrinterMappingEntity
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    public EPrintDestination Destination { get; set; }
    public EPrinterConnectionType ConnectionType { get; set; }
    public EPrinterPaperSize PaperSize { get; set; }
    public EPrinterProfile Profile { get; set; }

    public string? WindowsPrinterName { get; set; }
    public string? IpAddress { get; set; }
    public int? Port { get; set; }

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

