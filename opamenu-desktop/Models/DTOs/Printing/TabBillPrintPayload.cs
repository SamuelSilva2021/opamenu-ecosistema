using System.Collections.Generic;
using OpaMenu.Desktop.Models.DTOs.Tables;

namespace OpaMenu.Desktop.Models.DTOs.Printing;

public sealed class TabBillPrintPayload
{
    public string TableName { get; set; } = string.Empty;
    public string TabName { get; set; } = string.Empty;
    public decimal Total { get; set; }
    public List<OrderDto> Orders { get; set; } = new();
}

