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
}
