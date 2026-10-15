using System.Windows;
using Opamenu.Desktop.ViewModels;

namespace Opamenu.Desktop.Views;

public partial class MainWindow : Window
{
    public MainWindow(MainViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}