using OpaMenu.Application.Services.Interfaces.Opamenu;
using OpaMenu.Domain.DTOs.Opamenu.Providers;
using OpaMenu.Infrastructure.Shared.Entities.Opamenu;
using OpaMenu.Infrastructure.Shared.Enums.Opamenu;
using System;
using System.Threading.Tasks;
using OpaMenu.Infrastructure.Services;
using MercadoPago.Client.Common;
using MercadoPago.Client.Payment;
using MercadoPago.Config;
using MercadoPago.Resource.Payment;
using System.Collections.Generic;
using MercadoPago.Client;

namespace OpaMenu.Infrastructure.Services.PaymentProviders;

public class MercadoPagoPixProvider(TenantPaymentConfigEntity config, IPixService pixService) : IPixPaymentProvider
{
    private readonly TenantPaymentConfigEntity _config = config;
    private readonly IPixService _pixService = pixService;

    public EPaymentProvider ProviderType => EPaymentProvider.MercadoPago;

    public async Task<PixProviderResultDto> CreatePixAsync(PaymentEntity paymentEntity)
    {
        try
        {
            var accessToken = !string.IsNullOrEmpty(_config.AccessToken) ? _config.AccessToken : _config.ClientSecret;

            if (string.IsNullOrEmpty(accessToken))
                throw new Exception("Access Token não configurado para Mercado Pago.");

            MercadoPagoConfig.AccessToken = accessToken;

            var requestOptions = new RequestOptions();
            requestOptions.CustomHeaders.Add("x-idempotency-key", $"pgto_{paymentEntity.Id}");

            var payerEmail = !string.IsNullOrEmpty(paymentEntity.Order?.CustomerEmail) ? paymentEntity.Order.CustomerEmail : "cliente@opamenu.com.br";
            var payerFirstName = !string.IsNullOrEmpty(paymentEntity.Order?.CustomerName) ? paymentEntity.Order.CustomerName.Split(' ')[0] : "Cliente";
            var payerLastName = !string.IsNullOrEmpty(paymentEntity.Order?.CustomerName) && paymentEntity.Order.CustomerName.Contains(" ")
                ? paymentEntity.Order.CustomerName.Substring(paymentEntity.Order.CustomerName.IndexOf(' ') + 1)
                : "OpaMenu";


            var item = new PaymentItemRequest
            {
                Id = paymentEntity!.Order!.Id.ToString(),
            };

            var payerInfo = new PaymentAdditionalInfoPayerRequest
            {
                FirstName = payerFirstName,
                LastName = payerLastName,
                Phone = new PhoneRequest
                {
                    AreaCode = "11",
                    Number = "999966550"
                },
                Address = new AddressRequest
                {
                    StreetNumber = 0
                }
            };
            var shipmentsInfo = new PaymentShipmentsRequest
            {

                ReceiverAddress = new PaymentReceiverAddressRequest
                {
                    ZipCode = "12312-123",
                    StateName = "Rio de Janeiro",
                    CityName = "Buzios",
                    StreetName = "Av das Nacoes Unidas",
                    StreetNumber = 3003
                }

            };

            var additionalInfo = new PaymentAdditionalInfoRequest
            {
                Items = new List<PaymentItemRequest> { item },
                Payer = payerInfo,
                Shipments = shipmentsInfo
            };

            var paymentPayerRequest = new PaymentPayerRequest
            {
                Email = payerEmail,
                FirstName = payerFirstName,
                LastName = payerLastName,
                Identification = new IdentificationRequest
                {
                    Type = "CPF",
                    Number = "01234567890",
                },
            };

            var request = new PaymentCreateRequest
            {
                ApplicationFee = null,
                BinaryMode = false,
                CampaignId = null,
                Capture = true, // deixar true para pix pois Pix é liquidação imediata
                CouponAmount = null,
                Description = $"Pedido #{paymentEntity!.OrderId}",
                DifferentialPricingId = null,
                ExternalReference = $"{paymentEntity.Id}",
                Installments = 1, // Pix é sempre à vista (1 parcela)
                Metadata = null,
                NotificationUrl = null,
                Payer = paymentPayerRequest,
                PaymentMethodId = "pix",
                StatementDescriptor = null,
                TransactionAmount = paymentEntity!.Amount,
                //Token = "ff8080814c11e237014c1ff593b57b4d", somente para cartão para mascarar
                AdditionalInfo = additionalInfo,
                DateOfExpiration = DateTime.UtcNow.AddMinutes(15)
            };

            var client = new PaymentClient();
            Payment payment = await client.CreateAsync(request, requestOptions);

            var pointOfInteraction = payment.PointOfInteraction;
            var transactionData = pointOfInteraction?.TransactionData;

            return new PixProviderResultDto
            {
                Provider = EPaymentProvider.MercadoPago.ToString(),
                ProviderPaymentId = payment.Id.ToString(),
                QrCode = transactionData?.QrCode ?? "",
                QrCodeBase64 = transactionData?.QrCodeBase64 ?? "",
                ExpiresAt = payment.DateOfExpiration?.ToUniversalTime() ?? DateTime.UtcNow.AddMinutes(30),
                Amount = payment.TransactionAmount ?? paymentEntity.Amount,
                Currency = payment.CurrencyId ?? "BRL"
            };

            
        }
        catch (Exception ex)
        {
            // Fallback to static PIX if API fails (optional, but requested to use API)
            // Or just rethrow
            Console.WriteLine($"Erro MercadoPago: {ex.Message}");
            throw;
        }
    }

    public Task<WebhookPaymentResultDto> ProcessWebhookAsync(string payload, string signature)
    {
        // TODO: Validar assinatura usando _config.ClientSecret (HMAC SHA256 geralmente)
        
        // Simulação de parsing do payload do Mercado Pago
        // O payload real teria { "data": { "id": "123" }, "action": "payment.created" }
        
        return Task.FromResult(new WebhookPaymentResultDto
        {
            ProviderPaymentId = "simulated-mp-id", // Deveria extrair do payload
            NewStatus = EPaymentStatus.Paid,
            PaidAmount = 0, // Extrair se disponível, ou 0 para pegar do pedido
            PaidAt = DateTime.UtcNow,
            RawResponse = payload
        });
    }
}
