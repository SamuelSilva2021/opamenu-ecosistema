using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpaMenu.Desktop.Models.Data;
using OpaMenu.Desktop.Models.DTOs.Requests;
using OpaMenu.Desktop.Models.Enums;
using OpaMenu.Desktop.Services.Interfaces;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OpaMenu.Desktop.Services.Implementation;

public class SyncBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<SyncBackgroundService> _logger;
    private static readonly JsonSerializerOptions SyncJsonOptions = new(JsonSerializerDefaults.Web)
    {
        Converters = { new JsonStringEnumConverter() }
    };

    public SyncBackgroundService(IServiceProvider serviceProvider, ILogger<SyncBackgroundService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("SyncBackgroundService iniciado.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await SyncPendingOrdersAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro no loop principal do SyncBackgroundService.");
            }

            // Tenta sincronizar a cada 30 segundos
            await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
        }
    }

    private async Task SyncPendingOrdersAsync()
    {
        // Precisamos criar um escopo porque BackgroundService é Singleton
        // e o AppDbContext geralmente é Scoped.
        using var scope = _serviceProvider.CreateScope();
        
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var authService = scope.ServiceProvider.GetRequiredService<IAuthService>();
        
        // Pega o token atual do usuário logado
        var token = authService.GetCurrentToken();
        if (string.IsNullOrEmpty(token))
        {
            // Se não tem token, não tem como enviar. O usuário precisa estar logado.
            return;
        }

        // Buscar todos os pedidos pendentes de sincronização
        var pendingOrders = dbContext.LocalOrders
            .Where(o => o.SyncStatus == ESyncStatus.PendingSync || o.SyncStatus == ESyncStatus.Error)
            .ToList();

        if (!pendingOrders.Any())
            return;

        _logger.LogInformation($"Encontrados {pendingOrders.Count} pedidos pendentes de sincronização.");

        var httpClientFactory = scope.ServiceProvider.GetRequiredService<IHttpClientFactory>();
        // Usamos o httpClient configurado para o CatalogService que já aponta para a API correta
        // Ou podemos criar um client nomeado "CoreApi" no App.xaml.cs
        var httpClient = httpClientFactory.CreateClient("CoreApi");
        httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        foreach (var localOrder in pendingOrders)
        {
            try
            {
                localOrder.LastSyncAttempt = DateTime.UtcNow;

                if (string.IsNullOrWhiteSpace(localOrder.PayloadJson))
                {
                    localOrder.SyncStatus = ESyncStatus.Error;
                    localOrder.SyncErrorMessage = "PayloadJson vazio.";
                    continue;
                }

                var payloadJson = NormalizePayloadJson(localOrder.PayloadJson);
                if (!string.Equals(payloadJson, localOrder.PayloadJson, StringComparison.Ordinal))
                {
                    localOrder.PayloadJson = payloadJson;
                }

                var content = new StringContent(payloadJson, Encoding.UTF8, "application/json");
                
                var response = await httpClient.PostAsync("/api/orders", content);

                if (response.IsSuccessStatusCode)
                {
                    // Pedido sincronizado com sucesso
                    localOrder.SyncStatus = ESyncStatus.Synced;
                    localOrder.SyncErrorMessage = null;
                    
                    // Opcional: Ler a resposta para pegar o CloudId gerado pelo servidor
                    // var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<OrderResponseDto>>();
                    // if (apiResponse?.Data != null) localOrder.CloudId = apiResponse.Data.Id;
                    
                    _logger.LogInformation($"Pedido LocalId: {localOrder.LocalId} sincronizado com sucesso.");
                }
                else
                {
                    // Erro ao enviar (ex: 400 Bad Request, 500 Internal Server Error)
                    var errorResponse = await response.Content.ReadAsStringAsync();
                    localOrder.SyncStatus = ESyncStatus.Error;
                    localOrder.SyncErrorMessage = $"HTTP {(int)response.StatusCode} - {errorResponse}";
                    _logger.LogWarning($"Falha ao sincronizar pedido LocalId: {localOrder.LocalId}. Erro: {localOrder.SyncErrorMessage}");
                }
            }
            catch (Exception ex)
            {
                // Erro de rede (ex: sem internet, API fora do ar)
                localOrder.SyncStatus = ESyncStatus.Error;
                localOrder.SyncErrorMessage = ex.Message;
                _logger.LogWarning(ex, $"Exceção ao sincronizar pedido LocalId: {localOrder.LocalId}");
            }
        }

        await dbContext.SaveChangesAsync();
    }

    private static string NormalizePayloadJson(string payloadJson)
    {
        try
        {
            var requestDto = JsonSerializer.Deserialize<CreateOrderRequestDto>(payloadJson, SyncJsonOptions);
            if (requestDto is null)
                return payloadJson;

            var changed = false;

            if (string.IsNullOrWhiteSpace(requestDto.CustomerName))
            {
                requestDto.CustomerName = "Cliente Balcão";
                changed = true;
            }

            if (string.IsNullOrWhiteSpace(requestDto.CustomerPhone))
            {
                requestDto.CustomerPhone = "11999999999";
                changed = true;
            }

            if (requestDto.Items is null)
            {
                requestDto.Items = new();
                changed = true;
            }

            return changed ? JsonSerializer.Serialize(requestDto, SyncJsonOptions) : payloadJson;
        }
        catch
        {
            return payloadJson;
        }
    }
}
