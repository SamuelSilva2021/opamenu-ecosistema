
# Fluxo de Pagamento PIX — OpaMenu (SaaS Multitenant)

## Visão Geral

Este documento descreve o fluxo completo de pagamento via **PIX** no OpaMenu, um sistema de cardápio digital SaaS **multitenant**.

O objetivo é garantir:
- Isolamento por tenant
- Suporte a múltiplos provedores de pagamento
- Fluxo assíncrono e resiliente
- Aderência a Clean Architecture e SOLID

---

## Conceitos-Chave

### Tenant
Empresa/loja cliente do SaaS. Cada tenant:
- Possui sua própria configuração de pagamento
- Escolhe seu provedor PIX
- Recebe os valores diretamente no seu PSP

### Payment
Representa uma tentativa de pagamento vinculada a um pedido.

### Payment Provider
Abstração de um PSP (Mercado Pago, Gerencianet, Stripe, etc).

---

## Entidades Principais

### PaymentEntity

- Id
- TenantId
- OrderId
- Amount
- Method (PIX, CASH, etc)
- Status (Pending, Paid, Expired…)
- Provider
- GatewayTransactionId
- CreatedAt
- ProcessedAt

### TenantPaymentConfig

- TenantId
- Provider
- ApiKey / ClientId / ClientSecret
- IsActive

### PaymentRefundEntity

- PaymentId
- Amount
- Reason
- GatewayRefundId
- RefundedAt

---

## Enums

### EPaymentMethod

- Pix
- Cash
- CreditCard (futuro)

### EPaymentStatus

- Pending
- Authorized
- Paid
- Expired
- Failed
- Refunded
- Cancelled

### EPixProvider

- MercadoPago
- Gerencianet
- Stripe

---

## Fluxo Completo do PIX

### 1. Cliente acessa o cardápio

- URL pública com `tenantSlug`
- Backend resolve o Tenant

### 2. Cliente cria o pedido

- Pedido salvo com status `PendingPayment`

### 3. Cliente escolhe PIX

- Frontend chama:
  POST `/payments/pix`

### 4. PaymentService

- Valida pedido
- Cria `PaymentEntity` com status `Pending`
- Resolve provedor do tenant
- Delegada para o Provider

### 5. Provedor PIX

- Gera cobrança
- Retorna:
  - QR Code
  - Copia e cola
  - Expiração
  - TransactionId

### 6. Backend retorna ao frontend

- Exibe QR Code
- Inicia polling ou aguarda webhook

### 7. Webhook do provedor

- Provedor chama endpoint público
- Sistema valida assinatura
- Atualiza pagamento para `Paid`
- Atualiza pedido para `Paid`

---

## Factory — PaymentEntity

```csharp
public static PaymentEntity CreatePix(PixRequestDto request, Guid tenantId)
{
    return new PaymentEntity
    {
        Id = Guid.NewGuid(),
        TenantId = tenantId,
        OrderId = request.OrderId,
        Amount = request.Amount,
        Method = EPaymentMethod.Pix,
        Status = EPaymentStatus.Pending,
        CreatedAt = DateTime.UtcNow
    };
}
```

---

## Resolução do Provedor

```csharp
public async Task<IPixProvider> ResolvePixProviderAsync(Guid tenantId)
{
    var config = await _tenantPaymentConfigRepository.GetActivePixConfigAsync(tenantId);

    return config.Provider switch
    {
        EPixProvider.MercadoPago => new MercadoPagoPixProvider(config),
        EPixProvider.Gerencianet => new GerencianetPixProvider(config),
        _ => throw new NotSupportedException("Provedor PIX não suportado")
    };
}
```

---

## Resultado do Provedor

```csharp
public class PixProviderResult
{
    public string TransactionId { get; init; } = default!;
    public string QrCode { get; init; } = default!;
    public string QrCodeBase64 { get; init; } = default!;
    public DateTime ExpiresAt { get; init; }
    public decimal Amount { get; init; }
}
```

---

## Webhook

- Endpoint público
- Validação de assinatura
- Idempotência por TransactionId

Estados possíveis:
- Paid
- Failed
- Expired

---

## Boas Práticas

- Nunca confiar no frontend
- Sempre validar tenant
- Webhook é a fonte da verdade
- Pagamento ≠ Pedido
- Logs e observabilidade obrigatórios

---

## Evoluções Futuras

- Split de pagamento
- Cartão de crédito
- Assinaturas
- Retry automático de webhook

---

## Conclusão

Este fluxo garante escalabilidade, isolamento multitenant e flexibilidade para múltiplos provedores de pagamento, mantendo o core do sistema desacoplado de integrações externas.
