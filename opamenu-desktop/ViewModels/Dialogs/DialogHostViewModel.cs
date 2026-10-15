using CommunityToolkit.Mvvm.ComponentModel;

namespace Opamenu.Desktop.ViewModels.Dialogs;

public sealed partial class DialogHostViewModel : ObservableObject
{
    [ObservableProperty]
    private bool _isOpen;

    [ObservableProperty]
    private DialogViewModelBase? _currentDialog;
}
