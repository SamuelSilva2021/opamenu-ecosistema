using OpaMenu.Desktop.Services.Interfaces;
using OpaMenu.Desktop.ViewModels.Dialogs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OpaMenu.Desktop.Services.Implementation;

public sealed class DialogService : IDialogService
{
    private readonly Stack<(DialogViewModelBase Dialog, TaskCompletionSource<bool> Tcs)> _stack = new();

    public DialogHostViewModel Host { get; } = new();

    public Task ShowAsync(DialogViewModelBase dialog)
    {
        var tcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        _stack.Push((dialog, tcs));

        dialog.AttachCloseHandler(() => CloseTop(dialog));

        Host.CurrentDialog = dialog;
        Host.IsOpen = true;

        return tcs.Task;
    }

    public Task ShowMessageAsync(string title, string message)
    {
        return ShowAsync(new MessageDialogViewModel(title, message));
    }

    private void CloseTop(DialogViewModelBase dialog)
    {
        if (_stack.Count == 0)
        {
            Host.IsOpen = false;
            Host.CurrentDialog = null;
            return;
        }

        var (topDialog, topTcs) = _stack.Peek();
        if (!ReferenceEquals(topDialog, dialog))
            return;

        _stack.Pop();
        topTcs.TrySetResult(true);

        if (_stack.Count == 0)
        {
            Host.IsOpen = false;
            Host.CurrentDialog = null;
            return;
        }

        Host.CurrentDialog = _stack.Peek().Dialog;
        Host.IsOpen = true;
    }
}
