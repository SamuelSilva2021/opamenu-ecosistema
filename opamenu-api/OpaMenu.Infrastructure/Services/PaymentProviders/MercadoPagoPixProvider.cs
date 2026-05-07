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

    public async Task<PixProviderResultDto> CreatePixAsync(PaymentEntity paymentEntity, string? notificationUrl = null)
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
                    AreaCode = "00",
                    Number = paymentEntity.Order?.CustomerPhone ?? "000000000"
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
                    ZipCode = "00000-000",
                    StreetName = paymentEntity.Order?.DeliveryAddress ?? "N/A",
                    StreetNumber = 0
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
                LastName = payerLastName
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
                NotificationUrl = notificationUrl,
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

    public async Task<WebhookPaymentResultDto> ProcessWebhookAsync(string payload, string signature)
    {
        try
        {
            // Mercado Pago envia um JSON que pode conter o ID do pagamento em 'data.id' ou ser apenas o ID
            var json = System.Text.Json.JsonDocument.Parse(payload);
            string? mpPaymentId = null;

            if (json.RootElement.TryGetProperty("data", out var data) && data.TryGetProperty("id", out var idProp))
            {
                mpPaymentId = idProp.GetString() ?? idProp.GetRawText();
            }
            else if (json.RootElement.TryGetProperty("id", out var idDirect))
            {
                mpPaymentId = idDirect.GetString() ?? idDirect.GetRawText();
            }

            if (string.IsNullOrEmpty(mpPaymentId))
                return null!;

            // Para produção, deveríamos consultar o status atual na API do Mercado Pago usando o mpPaymentId
            // Por enquanto, assumimos que se o webhook chegou e a action é 'payment.updated' ou similar,
            // vamos considerar como processável.
            
            var action = json.RootElement.TryGetProperty("action", out var actionProp) ? actionProp.GetString() : "";
            
            // Simplificação para o MVP: se recebemos algo do MP sobre esse ID, consultamos o status (opcional aqui, mas recomendado)
            // Para este fluxo, vamos retornar o ID para que o PaymentService localize o registro.
            
            return new WebhookPaymentResultDto
            {
                ProviderPaymentId = mpPaymentId,
                NewStatus = EPaymentStatus.Paid, // O PaymentService vai validar se realmente mudou
                PaidAmount = 0,
                PaidAt = DateTime.UtcNow,
                RawResponse = payload
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao processar webhook MP: {ex.Message}");
            return null!;
        }
    }
}
