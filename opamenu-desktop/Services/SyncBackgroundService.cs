using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpaMenu.Desktop.Models;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OpaMenu.Desktop.Services;

/// <summary>
/// Serviço que roda em segundo plano (BackgroundService).
/// Sua responsabilidade é verificar os pedidos não sincronizados no banco SQLite e 
/// enviá-los para a `opamenu-api` sempre que houver conexão.
/// </summary>
public class SyncBackgroundService : BackgroundService
{
    private readonly ILogger<SyncBackgroundService> _logger;

    public SyncBackgroundService(ILogger<SyncBackgroundService> logger)
    {
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Serviço de Sincronização em Background (SyncBackgroundService) iniciado.");

        // Executa enquanto o aplicativo não for fechado
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                // TODO: 1. Checar conexão com a internet
                // Se offline, pula o ciclo
                bool isOnline = true; // Simulação

                if (isOnline)
                {
                    await SyncPendingOrdersAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro no loop principal do SyncBackgroundService.");
            }

            // Aguarda 30 segundos antes da próxima checagem (mesma lógica do opamenu-gestor Flutter)
            await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
        }
    }

    private async Task SyncPendingOrdersAsync()
    {
        // Usa um novo contexto a cada execução para não travar a UI (Thread-Safety)
        using var dbContext = new AppDbContext();

        // Busca todos os pedidos que ainda não foram pra nuvem
        var pendingOrders = dbContext.LocalOrders
            .Where(o => o.SyncStatus == SyncStatus.PendingSync || o.SyncStatus == SyncStatus.Error)
            .ToList();

        if (!pendingOrders.Any())
            return;

        _logger.LogInformation($"Sincronizando {pendingOrders.Count} pedidos pendentes para a nuvem...");

        foreach (var order in pendingOrders)
        {
            try
            {
                order.LastSyncAttempt = DateTime.UtcNow;

                // TODO: Enviar o order.PayloadJson para a opamenu-api (ex: via HttpClient POST api/orders)
                // Se sucesso, atualizar o CloudId e o SyncStatus.
                
                // Simulação de sucesso
                order.SyncStatus = SyncStatus.Synced;
                order.CloudId = new Random().Next(1000, 9999);
                order.SyncErrorMessage = null;

                _logger.LogInformation($"Pedido Local {order.LocalId} sincronizado com sucesso. ID Nuvem: {order.CloudId}");
            }
            catch (Exception ex)
            {
                order.SyncStatus = SyncStatus.Error;
                order.SyncErrorMessage = ex.Message;
                _logger.LogError($"Erro ao sincronizar o pedido {order.LocalId}: {ex.Message}");
            }
        }

        // Salva as alterações do status no banco SQLite
        await dbContext.SaveChangesAsync();
    }
}