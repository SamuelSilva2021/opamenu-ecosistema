using CommunityToolkit.Mvvm.ComponentModel;
using OpaMenu.Desktop.Models.DTOs.Tables;
using OpaMenu.Desktop.Models.Enums;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;

namespace OpaMenu.Desktop.ViewModels.Screens;

public sealed class TableDetailsScreenViewModel : ObservableObject
{
    private readonly MainViewModel _root;

    public TableDetailsScreenViewModel(MainViewModel root)
    {
        _root = root;
        _root.PropertyChanged += RootOnPropertyChanged;
    }

    public TableFullDto? SelectedTableDetails => _root.SelectedTableDetails;

    public ObservableCollection<MainViewModel.TabSummaryItem> SelectedTableTabs => _root.SelectedTableTabs;

    public MainViewModel.TabSummaryItem? SelectedTab
    {
        get => _root.SelectedTab;
        set => _root.SelectedTab = value;
    }

    public decimal SelectedTabTotal => _root.SelectedTabTotal;

    public bool CanCheckoutSelectedTab => _root.CanCheckoutSelectedTab;

    public IReadOnlyList<MainViewModel.PaymentMethodOption> TabPaymentMethodOptions => _root.TabPaymentMethodOptions;

    public MainViewModel.PaymentMethodOption? SelectedTabPaymentMethod
    {
        get => _root.SelectedTabPaymentMethod;
        set => _root.SelectedTabPaymentMethod = value;
    }

    public bool IsTabCheckoutModalOpen => _root.IsTabCheckoutModalOpen;

    public ICommand BackToTablesCommand => _root.BackToTablesCommand;
    public ICommand OpenTabCheckoutModalCommand => _root.OpenTabCheckoutModalCommand;
    public ICommand ConfirmTabCheckoutCommand => _root.ConfirmTabCheckoutCommand;
    public ICommand CloseModalsCommand => _root.CloseModalsCommand;

    private void RootOnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(e.PropertyName))
            return;

        if (e.PropertyName is nameof(MainViewModel.SelectedTab))
        {
            OnPropertyChanged(nameof(SelectedTabTotal));
            OnPropertyChanged(nameof(CanCheckoutSelectedTab));
        }

        OnPropertyChanged(e.PropertyName);
    }
}
