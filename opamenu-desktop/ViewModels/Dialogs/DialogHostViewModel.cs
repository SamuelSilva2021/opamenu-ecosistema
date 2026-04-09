using CommunityToolkit.Mvvm.ComponentModel;

namespace OpaMenu.Desktop.ViewModels.Dialogs;

public sealed partial class DialogHostViewModel : ObservableObject
{
    [ObservableProperty]
    private bool _isOpen;

    [ObservableProperty]
    private DialogViewModelBase? _currentDialog;
}
