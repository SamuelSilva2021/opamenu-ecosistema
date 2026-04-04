﻿using System;
using System.Windows;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpaMenu.Desktop.Models;
using OpaMenu.Desktop.Services;
using OpaMenu.Desktop.ViewModels;
using OpaMenu.Desktop.Views;

namespace OpaMenu.Desktop;

public partial class App : Application
{
    private readonly IHost _host;

    public App()
    {
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
                services.AddTransient<LoginWindow>();
                services.AddSingleton<MainWindow>();

                // Register ViewModels
                services.AddTransient<LoginViewModel>();
                services.AddSingleton<MainViewModel>();

                // Store para manter o token em memória (Singleton)
                services.AddSingleton<TokenStore>();
                services.AddSingleton<UserStore>();

                // HTTP Clients e Serviços de API
                services.AddHttpClient<IAuthService, AuthService>();
                services.AddHttpClient<ICatalogService, CatalogService>();
                services.AddHttpClient<ICashRegisterService, CashRegisterService>();

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

        var loginWindow = _host.Services.GetRequiredService<LoginWindow>();
        loginWindow.Show();

        base.OnStartup(e);
    }

    protected override async void OnExit(ExitEventArgs e)
    {
        await _host.StopAsync();
        _host.Dispose();

        base.OnExit(e);
    }
}
