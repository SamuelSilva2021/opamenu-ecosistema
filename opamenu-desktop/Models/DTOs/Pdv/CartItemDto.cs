using CommunityToolkit.Mvvm.ComponentModel;
using System;

namespace OpaMenu.Desktop.Models.DTOs;

public partial class CartItemDto : ObservableObject
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    
    [ObservableProperty]
    private int _quantity;
    
    public decimal UnitPrice { get; set; }
    
    [ObservableProperty]
    private decimal _totalPrice;

    public List<AditionalSelectionDto> SelectedAditionals { get; set; } = new();
    
    public string? Notes { get; set; }
}

public class AditionalSelectionDto
{
    public Guid AditionalId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal Price { get; set; }
}
