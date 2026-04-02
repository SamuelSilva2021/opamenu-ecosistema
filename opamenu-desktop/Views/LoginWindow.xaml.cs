using System;
using System.Windows;
using System.Windows.Input;
using Microsoft.Extensions.DependencyInjection;
using OpaMenu.Desktop.ViewModels;

namespace OpaMenu.Desktop.Views;

public partial class LoginWindow : Window
{
    private readonly IServiceProvider _serviceProvider;

    public LoginWindow(LoginViewModel viewModel, IServiceProvider serviceProvider)
    {
        InitializeComponent();
        DataContext = viewModel;
        _serviceProvider = serviceProvider;

        // Assinatura do evento de sucesso do ViewModel
        viewModel.OnLoginSuccess += ViewModel_OnLoginSuccess;
    }

    private void ViewModel_OnLoginSuccess()
    {
        // Se o login der certo, pedimos ao DI (Dependency Injection) para instanciar a MainWindow
        var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
        mainWindow.Show();

        // Fechamos a tela de Login
        this.Close();
    }

    // Permite que o usuário clique e arraste a janela mesmo ela não tendo bordas
    private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ButtonState == MouseButtonState.Pressed)
        {
            this.DragMove();
        }
    }

    // Botão de fechar (X)
    private void BtnClose_Click(object sender, RoutedEventArgs e)
    {
        Application.Current.Shutdown();
    }
}