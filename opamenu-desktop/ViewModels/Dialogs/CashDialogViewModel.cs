using CommunityToolkit.Mvvm.Input;
using OpaMenu.Desktop.Models.DTOs.Pdv;
using OpaMenu.Desktop.ViewModels;
using OpaMenu.Desktop.Services.Interfaces;
using System.ComponentModel;
using System.Threading.Tasks;

namespace OpaMenu.Desktop.ViewModels.Dialogs;

public sealed partial class CashDialogViewModel : DialogViewModelBase
{
    private readonly MainViewModel _root;
    private readonly IDialogService _dialogService;

    public CashDialogViewModel(MainViewModel root, IDialogService dialogService)
    {
        _root = root;
        _dialogService = dialogService;
        _root.PropertyChanged += RootOnPropertyChanged;
    }

    public override string Title => _root.CashModalTitle;

    public bool IsOpeningCashShift => _root.IsOpeningCashShift;
    public bool IsClosingCashShift => _root.IsClosingCashShift;
    public bool IsCashMovementModal => _root.IsCashMovementModal;
    public bool HasClosingDiscrepancy => _root.HasClosingDiscrepancy;

    public decimal OpeningCashBalance
    {
        get => _root.OpeningCashBalance;
        set => _root.OpeningCashBalance = value;
    }

    public decimal ClosingCashBalance
    {
        get => _root.ClosingCashBalance;
        set => _root.ClosingCashBalance = value;
    }

    public decimal ClosingDiscrepancy => _root.ClosingDiscrepancy;

    public string? ClosingDiscrepancyJustification
    {
        get => _root.ClosingDiscrepancyJustification;
        set => _root.ClosingDiscrepancyJustification = value ?? string.Empty;
    }

    public decimal ExpectedCashBalance => _root.ExpectedCashBalance;

    public decimal ClosingTotalSales => _root.ClosingTotalSales;
    public decimal ClosingTotalInflows => _root.ClosingTotalInflows;
    public decimal ClosingTotalOutflows => _root.ClosingTotalOutflows;
    public System.Collections.ObjectModel.ObservableCollection<PaymentMethodSummaryDto> ClosingSalesByPaymentMethod => _root.ClosingSalesByPaymentMethod;

    public decimal CashMovementAmount
    {
        get => _root.CashMovementAmount;
        set => _root.CashMovementAmount = value;
    }

    public string CashMovementDescription
    {
        get => _root.CashMovementDescription;
        set => _root.CashMovementDescription = value;
    }

    [RelayCommand]
    private void Cancel()
    {
        _root.CloseModalsCommand.Execute(null);
        RequestClose();
    }

    [RelayCommand]
    private async Task ConfirmAsync()
    {
        try
        {
            await _root.ConfirmCashShiftCommand.ExecuteAsync(null);
            RequestClose();
        }
        catch (System.Exception ex)
        {
            await _dialogService.ShowMessageAsync("Caixa", ex.Message);
        }
    }

    private void RootOnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(e.PropertyName))
            return;

        if (e.PropertyName is nameof(MainViewModel.CashModalTitle))
            OnPropertyChanged(nameof(Title));

        OnPropertyChanged(e.PropertyName);
    }
}
