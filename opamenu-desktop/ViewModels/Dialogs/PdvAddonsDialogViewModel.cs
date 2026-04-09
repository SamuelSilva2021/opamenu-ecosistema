using CommunityToolkit.Mvvm.Input;
using OpaMenu.Desktop.ViewModels.Screens;
using System.Threading.Tasks;

namespace OpaMenu.Desktop.ViewModels.Dialogs;

public sealed partial class PdvAddonsDialogViewModel : DialogViewModelBase
{
    private readonly PdvScreenViewModel _pdv;

    public PdvAddonsDialogViewModel(PdvScreenViewModel pdv)
    {
        _pdv = pdv;
    }

    public override string Title => _pdv.ProductToConfigure == null ? "Personalizar" : $"Personalizar: {_pdv.ProductToConfigure.Name}";

    public object? ProductToConfigure => _pdv.ProductToConfigure;
    public System.Collections.ObjectModel.ObservableCollection<OpaMenu.Desktop.ViewModels.Components.SelectableAditionalGroup> AddonGroups => _pdv.AddonGroups;

    public string ItemNotes
    {
        get => _pdv.ItemNotes;
        set => _pdv.ItemNotes = value;
    }

    [RelayCommand]
    private void Cancel()
    {
        _pdv.ClearAddonSelection();
        RequestClose();
    }

    [RelayCommand]
    private async Task AddAsync()
    {
        await _pdv.AddToCartFromModalCommand.ExecuteAsync(null);

        if (_pdv.ProductToConfigure == null)
        {
            RequestClose();
        }
    }
}
