using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OpaMenu.Desktop.Models.Enums;
using OpaMenu.Desktop.Models.Printing;
using OpaMenu.Desktop.Services.Interfaces;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace OpaMenu.Desktop.ViewModels.Dialogs;

public sealed partial class PrinterSettingsDialogViewModel : DialogViewModelBase
{
    private readonly IPrinterConfigurationService _printerConfigurationService;
    private readonly IPrintService _printService;
    private readonly IDialogService _dialogService;

    public PrinterSettingsDialogViewModel(
        IPrinterConfigurationService printerConfigurationService,
        IPrintService printService,
        IDialogService dialogService)
    {
        _printerConfigurationService = printerConfigurationService;
        _printService = printService;
        _dialogService = dialogService;

        Destinations.Add(new DestinationOption(EPrintDestination.TableBill, "Conta (Mesa/Comanda)"));
        Destinations.Add(new DestinationOption(EPrintDestination.Kitchen, "Comanda (Cozinha)"));
        Destinations.Add(new DestinationOption(EPrintDestination.Bar, "Comanda (Bar)"));
        Destinations.Add(new DestinationOption(EPrintDestination.Cashier, "Recibo (Caixa)"));

        SelectedDestination = Destinations.FirstOrDefault();

        ConnectionTypeOptions = new[] { EPrinterConnectionType.Tcp9100, EPrinterConnectionType.WindowsSpoolerRaw };
        PaperSizeOptions = new[] { EPrinterPaperSize.Mm80, EPrinterPaperSize.Mm58 };

        ConnectionType = EPrinterConnectionType.Tcp9100;
        PaperSize = EPrinterPaperSize.Mm80;
        PortText = "9100";
    }

    public override string Title => "Impressoras";

    public ObservableCollection<string> InstalledPrinters { get; } = new();
    public ObservableCollection<DestinationOption> Destinations { get; } = new();

    public EPrinterConnectionType[] ConnectionTypeOptions { get; }
    public EPrinterPaperSize[] PaperSizeOptions { get; }

    [ObservableProperty]
    private DestinationOption? _selectedDestination;

    [ObservableProperty]
    private EPrinterConnectionType _connectionType;

    [ObservableProperty]
    private EPrinterPaperSize _paperSize;

    [ObservableProperty]
    private string? _windowsPrinterName;

    [ObservableProperty]
    private string? _ipAddress;

    [ObservableProperty]
    private string _portText = string.Empty;

    public bool IsWindowsSpooler => ConnectionType == EPrinterConnectionType.WindowsSpoolerRaw;
    public bool IsTcp => ConnectionType == EPrinterConnectionType.Tcp9100;

    public async Task InitializeAsync()
    {
        InstalledPrinters.Clear();
        foreach (var name in await _printerConfigurationService.GetInstalledWindowsPrintersAsync())
            InstalledPrinters.Add(name);

        await LoadMappingAsync();
    }

    partial void OnConnectionTypeChanged(EPrinterConnectionType value)
    {
        OnPropertyChanged(nameof(IsWindowsSpooler));
        OnPropertyChanged(nameof(IsTcp));
    }

    partial void OnSelectedDestinationChanged(DestinationOption? value)
    {
        _ = LoadMappingAsync();
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        try
        {
            var mapping = BuildMapping();
            await _printerConfigurationService.UpsertMappingAsync(mapping);
            RequestClose();
        }
        catch (Exception ex)
        {
            await _dialogService.ShowMessageAsync("Impressoras", ex.Message);
        }
    }

    [RelayCommand]
    private async Task TestAsync()
    {
        try
        {
            var mapping = BuildMapping();
            await _printService.PrintTestAsync(mapping);
            await _dialogService.ShowMessageAsync("Impressoras", "Teste enviado para a impressora.");
        }
        catch (Exception ex)
        {
            await _dialogService.ShowMessageAsync("Impressoras", ex.Message);
        }
    }

    [RelayCommand]
    private void Cancel()
    {
        RequestClose();
    }

    private async Task LoadMappingAsync()
    {
        try
        {
            if (SelectedDestination == null)
                return;

            var mapping = await _printerConfigurationService.GetMappingAsync(SelectedDestination.Value.Destination);
            if (mapping == null)
                return;

            ConnectionType = mapping.ConnectionType;
            PaperSize = mapping.PaperSize;
            WindowsPrinterName = mapping.WindowsPrinterName;
            IpAddress = mapping.IpAddress;
            PortText = (mapping.Port ?? 9100).ToString();
        }
        catch
        {
        }
    }

    private PrinterMapping BuildMapping()
    {
        if (SelectedDestination == null)
            throw new InvalidOperationException("Selecione um destino.");

        var port = 9100;
        if (IsTcp && !int.TryParse(PortText?.Trim(), out port))
            throw new InvalidOperationException("Porta inválida.");

        if (IsTcp && string.IsNullOrWhiteSpace(IpAddress))
            throw new InvalidOperationException("Informe o IP da impressora.");

        if (IsWindowsSpooler && string.IsNullOrWhiteSpace(WindowsPrinterName))
            throw new InvalidOperationException("Selecione uma impressora do Windows.");

        return new PrinterMapping(
            SelectedDestination.Value.Destination,
            ConnectionType,
            PaperSize,
            EPrinterProfile.GenericEscPos,
            WindowsPrinterName,
            IpAddress,
            IsTcp ? port : null
        );
    }

    public readonly record struct DestinationOption(EPrintDestination Destination, string Name);
}
