using CommunityToolkit.Mvvm.Input;
using OpaMenu.Desktop.ViewModels.Screens;
using System.Threading.Tasks;

namespace OpaMenu.Desktop.ViewModels.Dialogs;

public sealed partial class TabCheckoutDialogViewModel : DialogViewModelBase
{
    private readonly TableDetailsScreenViewModel _details;

    public TabCheckoutDialogViewModel(TableDetailsScreenViewModel details)
    {
        _details = details;
    }

    public override string Title => _details.SelectedTab == null ? "Fechar Comanda" : $"Fechar {_details.SelectedTab.Name}";

    public decimal SelectedTabTotal => _details.SelectedTabTotal;

    public System.Collections.Generic.IReadOnlyList<TableDetailsScreenViewModel.PaymentMethodOption> TabPaymentMethodOptions => _details.TabPaymentMethodOptions;

    public TableDetailsScreenViewModel.PaymentMethodOption? SelectedTabPaymentMethod
    {
        get => _details.SelectedTabPaymentMethod;
        set => _details.SelectedTabPaymentMethod = value;
    }

    [RelayCommand]
    private void Cancel()
    {
        RequestClose();
    }

    [RelayCommand]
    private async Task ConfirmAsync()
    {
        var ok = await _details.TryCheckoutSelectedTabAsync();
        if (ok)
            RequestClose();
    }
}
