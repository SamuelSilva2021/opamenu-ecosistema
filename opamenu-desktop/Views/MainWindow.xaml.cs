using System.Windows;
using OpaMenu.Desktop.ViewModels;

namespace OpaMenu.Desktop.Views;

public partial class MainWindow : Window
{
    public MainWindow(MainViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}