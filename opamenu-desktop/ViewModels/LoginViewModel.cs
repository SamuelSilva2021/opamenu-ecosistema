using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OpaMenu.Desktop.Services.Interfaces;
using System;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace OpaMenu.Desktop.ViewModels;

public partial class LoginViewModel : ObservableObject
{
    private readonly IAuthService _authService;

    [ObservableProperty]
    private string _email = string.Empty;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    [ObservableProperty]
    private bool _isBusy;

    public event Action? OnLoginSuccess;

    public LoginViewModel(IAuthService authService)
    {
        _authService = authService;
    }

    [RelayCommand]
    private async Task LoginAsync(object parameter)
    {
        ErrorMessage = string.Empty;
        IsBusy = true;

        try
        {
            var passwordBox = parameter as PasswordBox;
            var password = passwordBox?.Password ?? string.Empty;

            if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(password))
            {
                ErrorMessage = "Preencha o seu E-mail e Senha.";
                return;
            }

            // Realiza a chamada real para a opamenu-authentication
            bool success = await _authService.LoginAsync(Email, password);

            if (success)
            {
                OnLoginSuccess?.Invoke();
            }
            else
            {
                ErrorMessage = "E-mail ou senha inválidos. Tente novamente.";
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }
        finally
        {
            IsBusy = false;
        }
    }
}