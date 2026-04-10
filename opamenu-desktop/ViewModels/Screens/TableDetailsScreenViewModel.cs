using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using OpaMenu.Desktop.Models.DTOs.Printing;
using OpaMenu.Desktop.Models.DTOs.Tables;
using OpaMenu.Desktop.Models.Enums;
using OpaMenu.Desktop.Models.Printing;
using OpaMenu.Desktop.Services.Interfaces;
using OpaMenu.Desktop.ViewModels.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace OpaMenu.Desktop.ViewModels.Screens;

public sealed partial class TableDetailsScreenViewModel : ObservableObject
{
    private readonly ITablesService _tablesService;
    private readonly Func<bool> _isCashShiftOpen;
    private readonly Action _navigateBackToTables;
    private readonly IDialogService _dialogService;
    private readonly IServiceProvider _serviceProvider;

    public TableDetailsScreenViewModel(
        ITablesService tablesService,
        Func<bool> isCashShiftOpen,
        Action navigateBackToTables,
        IDialogService dialogService,
        IServiceProvider serviceProvider)
    {
        _tablesService = tablesService;
        _isCashShiftOpen = isCashShiftOpen;
        _navigateBackToTables = navigateBackToTables;
        _dialogService = dialogService;
        _serviceProvider = serviceProvider;

        SelectedTabPaymentMethod = TabPaymentMethodOptions.FirstOrDefault();
    }

    [ObservableProperty]
    private TableFullDto? _selectedTableDetails;

    public ObservableCollection<TabSummaryItem> SelectedTableTabs { get; } = new();

    [ObservableProperty]
    private TabSummaryItem? _selectedTab;

    public decimal SelectedTabTotal => SelectedTab?.Total ?? 0m;
    public bool CanCheckoutSelectedTab => SelectedTab?.IsOpen == true;

    public IReadOnlyList<PaymentMethodOption> TabPaymentMethodOptions { get; } = new[]
    {
        new PaymentMethodOption(EPaymentMethod.Pix, "Pix"),
        new PaymentMethodOption(EPaymentMethod.CreditCard, "Cartão de Crédito"),
        new PaymentMethodOption(EPaymentMethod.DebitCard, "Cartão de Débito"),
        new PaymentMethodOption(EPaymentMethod.Cash, "Dinheiro")
    };

    [ObservableProperty]
    private PaymentMethodOption? _selectedTabPaymentMethod;

    public async Task LoadTableAsync(Guid tableId)
    {
        var details = await _tablesService.GetTableFullByIdAsync(tableId);
        if (details == null)
            throw new InvalidOperationException("Não foi possível carregar os detalhes da mesa.");

        SelectedTableDetails = details;
        BuildSelectedTableTabs(details);
    }

    [RelayCommand]
    private void BackToTables()
    {
        SelectedTab = null;
        SelectedTableTabs.Clear();
        SelectedTableDetails = null;
        _navigateBackToTables();
    }

    [RelayCommand]
    private async Task OpenTabCheckoutModalAsync()
    {
        if (SelectedTableDetails == null || SelectedTab == null)
            return;

        if (!_isCashShiftOpen())
        {
            await _dialogService.ShowMessageAsync("Mesas", "Não é possível fechar comanda com o caixa fechado.");
            return;
        }

        if (!CanCheckoutSelectedTab)
        {
            await _dialogService.ShowMessageAsync("Mesas", "Selecione uma comanda aberta para fechar.");
            return;
        }

        await _dialogService.ShowAsync(new TabCheckoutDialogViewModel(this));
    }

    [RelayCommand]
    private async Task PrintSelectedTabAsync()
    {
        try
        {
            if (SelectedTableDetails == null || SelectedTab == null)
                return;

            using var scope = _serviceProvider.CreateScope();
            var printerConfigurationService = scope.ServiceProvider.GetRequiredService<IPrinterConfigurationService>();
            var printService = scope.ServiceProvider.GetRequiredService<IPrintService>();

            var mapping = await printerConfigurationService.GetMappingAsync(EPrintDestination.TableBill);
            if (mapping == null)
            {
                await _dialogService.ShowMessageAsync("Impressão", "Configure a impressora de Conta (Mesa/Comanda) em Impressoras.");
                return;
            }

            var payload = new TabBillPrintPayload
            {
                TableName = SelectedTableDetails.Name,
                TabName = SelectedTab.Name,
                Total = SelectedTabTotal,
                Orders = SelectedTab.Orders.ToList()
            };

            var payloadJson = JsonSerializer.Serialize(payload, new JsonSerializerOptions(JsonSerializerDefaults.Web));
            await printService.EnqueueAsync(new PrintJobCreateRequest(EPrintDestination.TableBill, "TabBill", payloadJson));

            await _dialogService.ShowMessageAsync("Impressão", "Conta enviada para a fila de impressão.");
        }
        catch (Exception ex)
        {
            await _dialogService.ShowMessageAsync("Impressão", ex.Message);
        }
    }

    internal async Task<bool> TryCheckoutSelectedTabAsync()
    {
        try
        {
            if (SelectedTableDetails == null || SelectedTab == null || SelectedTabPaymentMethod == null)
                return false;

            await _tablesService.CheckoutTabAsync(SelectedTableDetails.Id, SelectedTab.Id, SelectedTabPaymentMethod.Method);
            await LoadTableAsync(SelectedTableDetails.Id);
            return true;
        }
        catch (Exception ex)
        {
            await _dialogService.ShowMessageAsync("Fechamento de Comanda", ex.Message);
            return false;
        }
    }

    partial void OnSelectedTabChanged(TabSummaryItem? value)
    {
        OnPropertyChanged(nameof(SelectedTabTotal));
        OnPropertyChanged(nameof(CanCheckoutSelectedTab));
    }

    private void BuildSelectedTableTabs(TableFullDto details)
    {
        SelectedTableTabs.Clear();

        foreach (var tab in details.Tabs.OrderByDescending(t => t.OpenedAt))
        {
            var orders = (tab.Orders ?? new List<OrderDto>())
                .Where(o => o.Status is not 5 and not 6)
                .ToList();

            var total = orders.Sum(o => o.Total);
            SelectedTableTabs.Add(new TabSummaryItem(tab, total, orders.Count));
        }

        SelectedTab = SelectedTableTabs.FirstOrDefault(t => t.IsOpen) ?? SelectedTableTabs.FirstOrDefault();
    }

    public sealed class TabSummaryItem
    {
        public TabSummaryItem(TabDto tab, decimal total, int ordersCount)
        {
            Tab = tab;
            Total = total;
            OrdersCount = ordersCount;
        }

        public TabDto Tab { get; }
        public Guid Id => Tab.Id;
        public string Name => string.IsNullOrWhiteSpace(Tab.Name) ? "Comanda" : Tab.Name!;
        public int Status => Tab.Status;
        public bool IsOpen => Status == 1;
        public string StatusText => IsOpen ? "Aberta" : "Fechada";
        public decimal Total { get; }
        public int OrdersCount { get; }
        public IReadOnlyList<OrderDto> Orders => (Tab.Orders ?? new List<OrderDto>()).AsReadOnly();
    }

    public sealed class PaymentMethodOption
    {
        public PaymentMethodOption(EPaymentMethod method, string name)
        {
            Method = method;
            Name = name;
        }

        public EPaymentMethod Method { get; }
        public string Name { get; }
    }
}
