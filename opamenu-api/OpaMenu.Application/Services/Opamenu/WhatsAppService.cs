//using Microsoft.Extensions.Logging;
//using Microsoft.IdentityModel.Clients.ActiveDirectory;
//using OpaMenu.Application.Services.Interfaces.Opamenu;
//using OpaMenu.Domain.Interfaces;
//using OpaMenu.Infrastructure.Shared.Entities.MultiTenant.Tenant;
//using OpaMenu.Infrastructure.Shared.Entities.Opamenu;
//using OpaMenu.Infrastructure.Shared.Enums.Opamenu;
//using System.Net.Http.Json;
//using System.Text.Json;

//namespace OpaMenu.Application.Services.Opamenu;

//public class WhatsAppService(
//    IRepository<TenantWhatsAppConfigEntity> configRepository,
//    IRepository<TenantEntity> tenantRepository,
//    IHttpClientFactory httpClientFactory,
//    ILogger<WhatsAppService> logger
//) : IWhatsAppService
//{
//    private readonly IRepository<TenantWhatsAppConfigEntity> _configRepository = configRepository;
//    private readonly IRepository<TenantEntity> _tenantRepository = tenantRepository;
//    private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;
//    private readonly ILogger<WhatsAppService> _logger = logger;

//    public async Task<bool> IsInstanceConnectedAsync(Guid tenantId)
//    {
//        var config = await GetConfigAsync(tenantId);
//        if (config == null || config.Provider != EWhatsAppProvider.EvolutionApi) return false;

//        try
//        {
//            var client = _httpClientFactory.CreateClient();
//            client.BaseAddress = new Uri(config.BaseUrl ?? "https://api.evolution-api.com");
//            client.DefaultRequestHeaders.Add("apikey", config.ApiKey);

//            var response = await client.GetAsync($"/instance/connectionState/{config.InstanceId}");
//            if (!response.IsSuccessStatusCode) return false;

//            var content = await response.Content.ReadAsStringAsync();
//            var json = JsonDocument.Parse(content);
//            var state = json.RootElement.GetProperty("instance").GetProperty("state").GetString();

//            return state == "open";
//        }
//        catch (Exception ex)
//        {
//            _logger.LogError(ex, "Erro ao verificar conexão do WhatsApp para o Tenant {TenantId}", tenantId);
//            return false;
//        }
//    }

//    public async Task ProcessIncomingMessageAsync(Guid tenantId, string phoneNumber, string message)
//    {
//        var config = await GetConfigAsync(tenantId);
//        if (config == null || !config.IsActive) return;

//        _logger.LogInformation("Mensagem recebida do WhatsApp ({Phone}): {Message}", phoneNumber, message);

//        // Lógica de consulta de status automática
//        if (config.OrderStatusLookupEnabled && message.Contains("status", StringComparison.OrdinalIgnoreCase))
//        {
//            // TODO: Buscar último pedido do cliente por telefone e enviar status
//            await SendTextMessageAsync(tenantId, phoneNumber, "Em breve consultaremos seu pedido automaticamente...");
//        }
//        // Lógica de boas-vindas
//        else if (config.WelcomeBotEnabled)
//        {
//            await SendMenuLinkAsync(tenantId, phoneNumber);
//        }
//    }

//    public async Task<bool> SendMenuLinkAsync(Guid tenantId, string phoneNumber)
//    {
//        var tenant = await _tenantRepository.GetByIdAsync(tenantId);
//        if (tenant == null) return false;

//        var config = await GetConfigAsync(tenantId);
//        var welcomeMsg = config?.WelcomeMessage ?? $"Olá! Bem-vindo ao {tenant.Name}. Aqui está o nosso cardápio digital:";
//        var menuUrl = $"https://{tenant.Slug}.opamenu.com.br";

//        return await SendTextMessageAsync(tenantId, phoneNumber, $"{welcomeMsg}\n{menuUrl}");
//    }

//    public async Task<bool> SendOrderStatusUpdateAsync(Guid tenantId, string phoneNumber, string orderNumber, EOrderStatus newStatus)
//    {
//        var statusLabel = newStatus switch
//        {
//            EOrderStatus.Pendente => "Pendente",
//            EOrderStatus.Confirmado => "Confirmado",
//            EOrderStatus.EmPreparo => "em Preparo",
//            EOrderStatus.Pronto => "Pronto para entrega/retirada",
//            EOrderStatus.EmEntrega => "saiu para Entrega",
//            EOrderStatus.Concluido => "Concluído",
//            EOrderStatus.Cancelado => "Cancelado",
//            _ => newStatus.ToString()
//        };

//        var message = $"Seu pedido #{orderNumber} foi atualizado! Status atual: *{statusLabel}*.";
//        return await SendTextMessageAsync(tenantId, phoneNumber, message);
//    }

//    public async Task<bool> SendTextMessageAsync(Guid tenantId, string phoneNumber, string message)
//    {
//        var config = await GetConfigAsync(tenantId);
//        if (config == null || !config.IsActive || string.IsNullOrEmpty(config.InstanceId)) 
//        {
//            _logger.LogWarning("Configuração de WhatsApp não encontrada ou inativa para o Tenant {TenantId}", tenantId);
//            return false;
//        }

//        try
//        {
//            var client = _httpClientFactory.CreateClient();
//            var baseUrl = config.BaseUrl?.TrimEnd('/') ?? "https://api.evolution-api.com";
//            client.BaseAddress = new Uri(baseUrl);
//            client.DefaultRequestHeaders.Add("apikey", config.ApiKey);

//            var payload = new
//            {
//                number = phoneNumber.Replace("+", "").Replace("-", "").Replace(" ", ""),
//                options = new { delay = 1200, presence = "composing" },
//                textMessage = new { text = message }
//            };

//            var response = await client.PostAsJsonAsync($"/message/sendText/{config.InstanceId}", payload);
            
//            if (!response.IsSuccessStatusCode)
//            {
//                var error = await response.Content.ReadAsStringAsync();
//                _logger.LogError("Erro ao enviar mensagem WhatsApp: {Status} - {Error}", response.StatusCode, error);
//                return false;
//            }

//            return true;
//        }
//        catch (Exception ex)
//        {
//            _logger.LogError(ex, "Erro fatal ao enviar mensagem WhatsApp para {Phone}", phoneNumber);
//            return false;
//        }
//    }

//    private async Task<TenantWhatsAppConfigEntity?> GetConfigAsync(Guid tenantId)
//    {
//        return await _configRepository.FirstOrDefaultAsync(c => c.TenantId == tenantId);
//    }
//}
