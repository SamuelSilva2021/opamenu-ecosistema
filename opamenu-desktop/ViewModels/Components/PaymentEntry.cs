using CommunityToolkit.Mvvm.ComponentModel;

namespace OpaMenu.Desktop.ViewModels.Components;

public partial class PaymentEntry : ObservableObject
{
    [ObservableProperty]
    private string _method = "Pix";

    [ObservableProperty]
    private decimal _amount;
}
