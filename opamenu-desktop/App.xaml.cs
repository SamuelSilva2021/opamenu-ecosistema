﻿﻿﻿using System;
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

                // HTTP Clients e Serviços de API
                services.AddHttpClient<IAuthService, AuthService>();
                services.AddHttpClient<ICatalogService, CatalogService>();

                // Banco de Dados Local (SQLite + Entity Framework)
                services.AddDbContext<AppDbContext>();

                // Serviço de Sincronização rodando em Background
                services.AddHostedService<SyncBackgroundService>();
            })
            .Build();
    }

    protected override async void OnStartup(StartupEventArgs e)
    {
        // Garante que o banco de dados local SQLite seja criado caso não exista
        using (var scope = _host.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            dbContext.Database.EnsureCreated();
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