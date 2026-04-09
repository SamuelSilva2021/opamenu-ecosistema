using CommunityToolkit.Mvvm.Input;
using OpaMenu.Desktop.ViewModels.Components;
using OpaMenu.Desktop.ViewModels.Screens;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace OpaMenu.Desktop.ViewModels.Dialogs;

public sealed partial class PdvCheckoutDialogViewModel : DialogViewModelBase
{
    private readonly PdvScreenViewModel _pdv;

    public PdvCheckoutDialogViewModel(PdvScreenViewModel pdv)
    {
        _pdv = pdv;
    }

    public override string Title => "Finalizar Venda";

    public decimal CartTotal => _pdv.CartTotal;

    public ObservableCollection<PaymentEntry> Payments => _pdv.Payments;

    public ObservableCollection<string> PaymentMethodOptions => new(_pdv.PaymentMethodOptions);

    public decimal TotalPaid => _pdv.TotalPaid;
    public decimal RemainingAmount => _pdv.RemainingAmount;
    public decimal ChangeAmount => _pdv.ChangeAmount;

    [RelayCommand]
    private void AddPayment()
    {
        _pdv.AddPaymentCommand.Execute(null);
    }

    [RelayCommand]
    private void RemovePayment(PaymentEntry entry)
    {
        _pdv.RemovePaymentCommand.Execute(entry);
    }

    [RelayCommand]
    private void Cancel()
    {
        _pdv.ResetCheckout();
        RequestClose();
    }

    [RelayCommand]
    private async Task ConfirmAsync()
    {
        await _pdv.ConfirmPaymentCommand.ExecuteAsync(null);

        if (_pdv.CartItems.Count == 0)
            RequestClose();
    }
}
