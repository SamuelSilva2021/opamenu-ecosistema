using OpaMenu.Domain.DTOs.Tab;

namespace OpaMenu.Domain.DTOs.Table;

public class TableFullResponseDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Capacity { get; set; }
    public bool IsActive { get; set; }
    public string? QrCodeUrl { get; set; }
    public double LayoutX { get; set; }
    public double LayoutY { get; set; }
    public double LayoutWidth { get; set; }
    public double LayoutHeight { get; set; }
    public string? Floor { get; set; }

    public IEnumerable<TabResponseDto> Tabs { get; set; } = Array.Empty<TabResponseDto>();
}
