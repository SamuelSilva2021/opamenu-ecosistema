using Opamenu.Application.Services.Interfaces.Opamenu;
using Opamenu.Domain.Interfaces;
using Opamenu.Infrastructure.Services.PaymentProviders;
using Opamenu.Infrastructure.Shared.Enums.Opamenu;
using System;
using System.Threading.Tasks;
using Opamenu.Infrastructure.Services;

namespace Opamenu.Infrastructure.Services.Opamenu;

public sealed class PixProviderResolver(ITenantPaymentConfigRepository configRepository, IPixService pixService) : IPixProviderResolver
{
    private readonly ITenantPaymentConfigRepository _configRepository = configRepository;
    private readonly IPixService _pixService = pixService;

    public async Task<IPixPaymentProvider> ResolvePixProviderAsync(Guid tenantId)
    {
        var config = await _configRepository.GetActivePixConfigAsync(tenantId);

        if (config == null)
            throw new Exception("Nenhuma configuração de PIX ativa encontrada para este tenant.");

        return config.Provider switch
        {
            EPaymentProvider.MercadoPago => new MercadoPagoPixProvider(config, _pixService),
            EPaymentProvider.Gerencianet => new GerencianetPixProvider(config, _pixService),
            _ => throw new NotSupportedException($"Provedor PIX {config.Provider} não suportado.")
        };
    }
}
