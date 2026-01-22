using System.Diagnostics;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OpaMenu.Infrastructure.Shared.Data.Context.AccessControl;
using OpaMenu.Infrastructure.Shared.Data.Context.MultTenant;
using OpaMenu.Infrastructure.Shared.Data.Context.Opamenu;

var basePath = AppContext.BaseDirectory;
var configuration = new ConfigurationBuilder()
    .SetBasePath(basePath)
    .AddJsonFile("appsettings.json", optional: true)
    .AddEnvironmentVariables()
    .Build();

using var loggerFactory = LoggerFactory.Create(builder =>
{
    builder
        .AddConsole()
        .SetMinimumLevel(LogLevel.Information);
});

string? dbOption = string.Empty;

Console.WriteLine("Selecione uma opção:");
Console.WriteLine("1 - Adicionar nova migração");
Console.WriteLine("2 - Atualizar base de dados");
Console.WriteLine("3 - Dropar base de dados");
Console.Write("Opção: ");

var mainOption = Console.ReadLine();

switch (mainOption)
{
    case "1":
        await HandleAddMigrationAsync();
        break;
    case "2":
        await HandleUpdateDatabaseAsync();
        break;
    case "3":
        await HandleDropDatabaseAsync();
        break;
    default:
        Console.WriteLine("Opção inválida.");
        break;
}

Console.WriteLine("Processo finalizado.");

async Task HandleAddMigrationAsync()
{
    try
    {
        dbOption = AskDatabaseOption();

        if (dbOption is null)
        {
            Console.WriteLine("Nenhuma opção selecionada.");
            return;
        }

        Console.Write("Digite o nome da nova migração: ");
        var migrationName = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(migrationName))
        {
            Console.WriteLine("Nome da migração inválido.");
            return;
        }

        var baseDirectoryInfo = new DirectoryInfo(basePath);
        var migratorProjectDir = baseDirectoryInfo.Parent?.Parent?.Parent;

        if (migratorProjectDir is null)
        {
            Console.WriteLine("Não foi possível resolver o diretório do projeto de migrador.");
            return;
        }

        var commonsDirectoryInfo = migratorProjectDir.Parent;

        if (commonsDirectoryInfo is null)
        {
            Console.WriteLine("Não foi possível resolver o diretório opamenu-commons.");
            return;
        }

        var infrastructureProject = Path.Combine(commonsDirectoryInfo.FullName, "OpaMenu.Infrastructure.Shared", "OpaMenu.Infrastructure.Shared.csproj");
        var migratorProject = Path.Combine(migratorProjectDir.FullName, "OpaMenu.DatabaseMigrator.csproj");

        string contextName;
        string migrationsFolder;

        switch (dbOption)
        {
            case "1":
                contextName = nameof(MultiTenantDbContext);
                migrationsFolder = "MultiTenant";
                break;
            case "2":
                contextName = nameof(AccessControlDbContext);
                migrationsFolder = "AccessControl";
                break;
            case "3":
                contextName = nameof(OpamenuDbContext);
                migrationsFolder = "Opamenu";
                break;
            default:
                Console.WriteLine("Opção de base inválida.");
                return;
        }

        var args = $"ef migrations add {migrationName} --context {contextName} --project \"{infrastructureProject}\" --startup-project \"{migratorProject}\" --output-dir \"Migrations/{migrationsFolder}\" --no-build";

        Console.WriteLine();
        Console.WriteLine($"Executando: dotnet {args}");
        Console.WriteLine();

        var workingDirectory = commonsDirectoryInfo.Parent?.FullName ?? migratorProjectDir.FullName;

        var startInfo = new ProcessStartInfo
        {
            FileName = "dotnet",
            Arguments = args,
            WorkingDirectory = workingDirectory,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using var process = Process.Start(startInfo);

        if (process is null)
        {
            Console.WriteLine("Não foi possível iniciar o processo dotnet ef.");
            return;
        }

        var output = await process.StandardOutput.ReadToEndAsync();
        var error = await process.StandardError.ReadToEndAsync();
        await process.WaitForExitAsync();

        if (!string.IsNullOrWhiteSpace(output))
        {
            Console.WriteLine(output);
        }

        if (!string.IsNullOrWhiteSpace(error))
        {
            Console.WriteLine(error);
        }

        if (process.ExitCode == 0)
        {
            Console.WriteLine("Migração criada com sucesso.");
            Console.WriteLine($"Verifique a pasta Migrations/{migrationsFolder} no projeto OpaMenu.Infrastructure.Shared.");
            Console.WriteLine("Deseja atualizar o banco de dados?");
            Console.WriteLine("1 - Sim");
            Console.WriteLine("2 - Não");
            Console.Write("Opção: ");
            var updateDatabaseOption = Console.ReadLine();
            if (updateDatabaseOption == "1")
                await HandleUpdateDatabaseAsync();
        }
        else
        {
            Console.WriteLine($"Falha ao criar migração. Código de saída: {process.ExitCode}");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Falha ao criar a migração! Motivo: {ex.Message}");
    }
    
}

async Task HandleUpdateDatabaseAsync()
{
    dbOption = AskDatabaseOption();

    if (dbOption is null)
    {
        Console.WriteLine("Nenhuma opção selecionada.");
        return;
    }

    switch (dbOption)
    {
        case "1":
            await MigrateContextAsync<MultiTenantDbContext>(
                "Multi Tenant Database",
                "MultiTenantDatabase",
                opts => new MultiTenantDbContext(opts));
            break;
        case "2":
            await MigrateContextAsync<AccessControlDbContext>(
                "Access Control Database",
                "AccessControlDatabase",
                opts => new AccessControlDbContext(opts));
            break;
        case "3":
            await MigrateContextAsync<OpamenuDbContext>(
                "OpaMenu Business Database",
                "OpamenuDatabase",
                opts => new OpamenuDbContext(opts));
            break;
        default:
            Console.WriteLine("Opção de base inválida.");
            break;
    }
}

async Task HandleDropDatabaseAsync()
{
    dbOption = AskDatabaseOption();

    if (dbOption is null)
    {
        Console.WriteLine("Nenhuma opção selecionada.");
        return;
    }

    Console.WriteLine("!!! ATENÇÃO !!!");
    Console.WriteLine("Você está prestes a EXCLUIR COMPLETAMENTE a base de dados selecionada.");
    Console.WriteLine("Todos os dados serão perdidos permanentemente.");
    Console.Write("Tem certeza que deseja continuar? (S/N): ");
    var confirm = Console.ReadLine();

    if (confirm?.Trim().ToUpper() != "S")
    {
        Console.WriteLine("Operação cancelada.");
        return;
    }

    switch (dbOption)
    {
        case "1":
            await DropContextAsync<MultiTenantDbContext>(
                "Multi Tenant Database",
                "MultiTenantDatabase",
                opts => new MultiTenantDbContext(opts));
            break;
        case "2":
            await DropContextAsync<AccessControlDbContext>(
                "Access Control Database",
                "AccessControlDatabase",
                opts => new AccessControlDbContext(opts));
            break;
        case "3":
            await DropContextAsync<OpamenuDbContext>(
                "OpaMenu Business Database",
                "OpamenuDatabase",
                opts => new OpamenuDbContext(opts));
            break;
        default:
            Console.WriteLine("Opção de base inválida.");
            break;
    }
}

string? AskDatabaseOption()
{
    Console.WriteLine();
    Console.WriteLine("Selecione a base de dados:");
    Console.WriteLine("1 - Multi Tenant Database");
    Console.WriteLine("2 - Access Control Database");
    Console.WriteLine("3 - OpaMenu Business Database");
    Console.Write("Opção: ");

    return Console.ReadLine();
}

async Task MigrateContextAsync<TContext>(
    string dbLabel,
    string connectionStringName,
    Func<DbContextOptions<TContext>, TContext> contextFactory) where TContext : DbContext
{
    Console.WriteLine("========================================");
    Console.WriteLine($"   PROCESSANDO: {dbLabel}");
    Console.WriteLine($"   Contexto: {typeof(TContext).Name}");
    Console.WriteLine("========================================");

    var connectionString = configuration.GetConnectionString(connectionStringName);

    if (string.IsNullOrWhiteSpace(connectionString))
    {
        Console.WriteLine($"[AVISO] Connection string '{connectionStringName}' não encontrada. Pulando...");
        Console.WriteLine();
        return;
    }

    try
    {
        var optionsBuilder = new DbContextOptionsBuilder<TContext>()
            .UseLoggerFactory(loggerFactory)
            .EnableSensitiveDataLogging()
            .UseNpgsql(connectionString);

        await using var context = contextFactory(optionsBuilder.Options);

        Console.WriteLine("Verificando migrações pendentes...");
        var pendingMigrations = await context.Database.GetPendingMigrationsAsync();
        var count = pendingMigrations.Count();

        if (count > 0)
        {
            Console.WriteLine($"Encontradas {count} migrações pendentes:");
            foreach (var migration in pendingMigrations)
            {
                Console.WriteLine($" - {migration}");
            }

            Console.WriteLine("Aplicando migrações...");
            await context.Database.MigrateAsync();
            Console.WriteLine("SUCESSO: Migrações aplicadas.");
        }
        else
        {
            Console.WriteLine("Tudo atualizado. Nenhuma migração pendente.");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"[ERRO] Falha ao migrar {dbLabel}:");
        Console.WriteLine(ex.Message);
        if (ex.InnerException != null)
        {
            Console.WriteLine($"Detalhe: {ex.InnerException.Message}");
        }
    }

    Console.WriteLine();
}

async Task DropContextAsync<TContext>(
    string dbLabel,
    string connectionStringName,
    Func<DbContextOptions<TContext>, TContext> contextFactory) where TContext : DbContext
{
    Console.WriteLine("========================================");
    Console.WriteLine($"   DROPPING: {dbLabel}");
    Console.WriteLine($"   Contexto: {typeof(TContext).Name}");
    Console.WriteLine("========================================");

    var connectionString = configuration.GetConnectionString(connectionStringName);

    if (string.IsNullOrWhiteSpace(connectionString))
    {
        Console.WriteLine($"[AVISO] Connection string '{connectionStringName}' não encontrada. Pulando...");
        Console.WriteLine();
        return;
    }

    try
    {
        var optionsBuilder = new DbContextOptionsBuilder<TContext>()
            .UseLoggerFactory(loggerFactory)
            .UseNpgsql(connectionString);

        await using var context = contextFactory(optionsBuilder.Options);

        Console.WriteLine("Excluindo banco de dados...");
        var deleted = await context.Database.EnsureDeletedAsync();
        
        if (deleted)
            Console.WriteLine("SUCESSO: Banco de dados excluído.");
        else
             Console.WriteLine("INFO: Banco de dados não existia.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"[ERRO] Falha ao excluir {dbLabel}:");
        Console.WriteLine(ex.Message);
        if (ex.InnerException != null)
        {
            Console.WriteLine($"Detalhe: {ex.InnerException.Message}");
        }
    }

    Console.WriteLine();
}
