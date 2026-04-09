using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;

namespace OpaMenu.Desktop.ViewModels.Screens;

public sealed class TablesScreenViewModel : ObservableObject
{
    private readonly MainViewModel _root;

    public TablesScreenViewModel(MainViewModel root)
    {
        _root = root;
        _root.PropertyChanged += RootOnPropertyChanged;
    }

    public ObservableCollection<MainViewModel.TableListItem> Tables => _root.Tables;

    public ICommand LoadTablesCommand => _root.LoadTablesCommand;
    public ICommand OpenTableDetailsCommand => _root.OpenTableDetailsCommand;

    private void RootOnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(e.PropertyName))
            return;

        OnPropertyChanged(e.PropertyName);
    }
}
