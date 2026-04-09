using OpaMenu.Desktop.ViewModels.Dialogs;
using System.Threading.Tasks;

namespace OpaMenu.Desktop.Services.Interfaces;

public interface IDialogService
{
    DialogHostViewModel Host { get; }

    Task ShowAsync(DialogViewModelBase dialog);
    Task ShowMessageAsync(string title, string message);
}
