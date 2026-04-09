using CommunityToolkit.Mvvm.Input;

namespace OpaMenu.Desktop.ViewModels.Dialogs;

public sealed partial class MessageDialogViewModel : DialogViewModelBase
{
    public MessageDialogViewModel(string title, string message)
    {
        Title = title;
        Message = message;
    }

    public override string Title { get; }
    public string Message { get; }

    [RelayCommand]
    private void Ok()
    {
        RequestClose();
    }
}
