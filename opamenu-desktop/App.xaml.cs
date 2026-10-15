using System;
using System.Text;
using System.Windows;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Opamenu.Desktop.Models.Data;
using Opamenu.Desktop.Services;
using Opamenu.Desktop.Services.Implementation;
using Opamenu.Desktop.Services.Interfaces;
using Opamenu.Desktop.ViewModels;
using Opamenu.Desktop.Views;

namespace Opamenu.Desktop;

public partial class App : Application
{
    private readonly IHost _host;
    private IServiceScope? _uiScope;

    public App()
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

        _host = Host.CreateDefaultBuilder()
            .ConfigureAppConfiguration((context, config) =>
            {
                config.SetBasePath(AppContext.BaseDirectory);
                config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            })
            .ConfigureServices((context, services) =>
            {
                var configuration = context.Configuration;
                // Register Views
                services.AddScoped<LoginWindow>();
                services.AddScoped<MainWindow>();

                // Register ViewModels
                services.AddScoped<LoginViewModel>();
                services.AddScoped<MainViewModel>();

                // Store para manter o token em memória (Singleton)
                services.AddSingleton<TokenStore>();
                services.AddSingleton<UserStore>();

                services.AddSingleton<IDialogService, DialogService>();

                // HTTP Clients e Serviços de API
                services.AddHttpClient<IAuthService, AuthService>();
                services.AddHttpClient<ICatalogService, CatalogService>();
                services.AddHttpClient<ICashRegisterService, CashRegisterService>();
                services.AddHttpClient<ITablesService, TablesService>();

                // Client nomeado para o Background Service (apontando para a API principal)
                services.AddHttpClient("CoreApi", (sp, client) =>
                {
                    var config = sp.GetRequiredService<IConfiguration>();
                    var coreUrl = config.GetValue<string>("ApiSettings:CoreApiUrl") 
                        ?? throw new InvalidOperationException("ApiSettings:CoreApiUrl não configurado");
                    client.BaseAddress = new Uri(coreUrl);
                });

                // Registrar o serviço de background para sincronização (Offline-First)
                services.AddHostedService<SyncBackgroundService>();

                services.AddScoped<IPrinterConfigurationService, PrinterConfigurationService>();
                services.AddScoped<IPrintService, PrintService>();
                services.AddScoped<IPrintJobProcessor, PrintJobProcessor>();
                services.AddHostedService<PrintBackgroundService>();

                // Banco de Dados Local (SQLite + Entity Framework)
                services.AddDbContext<AppDbContext>();
            })
            .Build();
    }

    protected override async void OnStartup(StartupEventArgs e)
    {
        // Garante que o banco de dados local SQLite seja atualizado com a última Migration
        using (var scope = _host.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            
            // Aplica a migration e cria o banco, se necessário
            dbContext.Database.Migrate();
        }

        await _host.StartAsync();

        _uiScope = _host.Services.CreateScope();
        var loginWindow = _uiScope.ServiceProvider.GetRequiredService<LoginWindow>();
        loginWindow.Show();

        base.OnStartup(e);
    }

    protected override async void OnExit(ExitEventArgs e)
    {
        await _host.StopAsync();
        _uiScope?.Dispose();
        _host.Dispose();

        base.OnExit(e);
    }
}
