using CommunityToolkit.Mvvm.ComponentModel;
using System;

namespace OpaMenu.Desktop.ViewModels.Dialogs;

public abstract class DialogViewModelBase : ObservableObject
{
    private Action? _requestClose;

    public abstract string Title { get; }

    public void AttachCloseHandler(Action handler)
    {
        _requestClose = handler;
    }

    protected void RequestClose()
    {
        _requestClose?.Invoke();
    }
}
