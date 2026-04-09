using CommunityToolkit.Mvvm.Input;
using OpaMenu.Desktop.ViewModels.Screens;

namespace OpaMenu.Desktop.ViewModels.Dialogs;

public sealed partial class PdvCustomerDialogViewModel : DialogViewModelBase
{
    private readonly PdvScreenViewModel _pdv;
    private readonly string _initialTableNumber;
    private readonly string _initialCustomerName;
    private readonly string _initialCustomerDocument;
    private readonly string _initialOrderObservation;

    public PdvCustomerDialogViewModel(PdvScreenViewModel pdv)
    {
        _pdv = pdv;
        _initialTableNumber = pdv.TableNumber;
        _initialCustomerName = pdv.CustomerName;
        _initialCustomerDocument = pdv.CustomerDocument;
        _initialOrderObservation = pdv.OrderObservation;
    }

    public override string Title => "Identificação do Cliente";

    public string TableNumber
    {
        get => _pdv.TableNumber;
        set => _pdv.TableNumber = value;
    }

    public string CustomerName
    {
        get => _pdv.CustomerName;
        set => _pdv.CustomerName = value;
    }

    public string CustomerDocument
    {
        get => _pdv.CustomerDocument;
        set => _pdv.CustomerDocument = value;
    }

    public string OrderObservation
    {
        get => _pdv.OrderObservation;
        set => _pdv.OrderObservation = value;
    }

    [RelayCommand]
    private void Cancel()
    {
        _pdv.TableNumber = _initialTableNumber;
        _pdv.CustomerName = _initialCustomerName;
        _pdv.CustomerDocument = _initialCustomerDocument;
        _pdv.OrderObservation = _initialOrderObservation;
        RequestClose();
    }

    [RelayCommand]
    private void Save()
    {
        RequestClose();
    }
}
