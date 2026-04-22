# Correções para o lançamento

Este documento é o **checklist crítico** para colocar o OpaMenu em produção com segurança e previsibilidade operacional.

## Meta (Definition of Done)
- Nenhum endpoint público permite alteração de pedido sem validação de vínculo (tenant/cliente).
- Tempo real (SignalR) funciona com autenticação e contrato consistente entre backend e apps.
- Enum/status de pedido estão alinhados (backend ↔ painel ↔ gestor).
- PIX está pronto para produção (webhook confiável + pedido reflete pagamento).
- `docker-compose.yml` e variáveis de ambiente batem com o que o backend lê.

## P0 — Bloqueadores de lançamento

### 1) Falhas graves em endpoints públicos (manipulação de pedidos por ID)
**Problema**
- Rotas públicas permitem atualizar dados de um pedido apenas com `orderId`, sem validação forte de vínculo com `slug/tenant` e/ou “dono” do pedido.

**Onde aparece**
- Controller público: [PublicMenuController.cs](file:///d:/dev/opamenu-ecosistema/opamenu-api/OpaMenu.Web/UserEntry/Public/PublicMenuController.cs)
- Atualização de pagamento: [UpdateOrderPaymentMethodAsync](file:///d:/dev/opamenu-ecosistema/opamenu-api/OpaMenu.Application/Services/Opamenu/OrderService.cs#L1391-L1437)
- Atualização de entrega: [UpdateOrderDeliveryTypeAsync](file:///d:/dev/opamenu-ecosistema/opamenu-api/OpaMenu.Application/Services/Opamenu/OrderService.cs#L1438-L1496)
- Há TODO explícito indicando falta de validação: [PublicMenuController.cs](file:///d:/dev/opamenu-ecosistema/opamenu-api/OpaMenu.Web/UserEntry/Public/PublicMenuController.cs#L140-L151)

**Risco/impacto**
- Fraude e caos operacional: qualquer pessoa que obtenha um `orderId` consegue alterar forma de pagamento/entrega.

**Correção mínima (MVP seguro)**
- Toda operação pública de update/cancel deve validar:
  - `orderId` pertence ao `tenantSlug` da rota
  - e/ou o request possui um “token de acesso do cliente” (ex.: `public_order_token`) associado ao pedido
  - e deve aplicar rate limit/anti-bruteforce.

**Critério de aceite**
- Alterações públicas retornam `404`/`403` quando `orderId` não pertence ao `slug`.
- Não é possível atualizar/cancelar pedido com `orderId` de outro tenant.
- Logs e auditoria registram tentativas inválidas.

**Testes**
- Teste automatizado cobrindo: tenant correto vs tenant incorreto.
- Smoke manual: criar pedido no slug A e tentar alterar via slug B.

---

### 2) SignalR sem proteção adequada + grupo “Administrators” acessível
**Problema**
- Hub permite entrada em grupo admin sem validação robusta.

**Onde aparece**
- Hub: [OrderNotificationHub.cs](file:///d:/dev/opamenu-ecosistema/opamenu-api/OpaMenu.Web/Hubs/OrderNotificationHub.cs#L60-L76)
- Pipeline: [Program.cs](file:///d:/dev/opamenu-ecosistema/opamenu-api/OpaMenu.Web/Program.cs#L189-L205)

**Risco/impacto**
- Vazamento de eventos (novos pedidos/status) para usuários não autorizados.
- Superfície de ataque para ruído/DoS via conexões e joins.

**Correção mínima (MVP seguro)**
- Exigir autenticação JWT no Hub.
- Remover/fechar `JoinAdminGroup` e criar grupos por tenant + role (ex.: `tenant:{id}:admins`).
- Validar permissão/módulo antes de inscrever o usuário no grupo.

**Critério de aceite**
- Conexão sem token falha.
- Usuário sem permissão não recebe eventos.
- Eventos chegam apenas para o tenant correto.

---

### 3) Tempo real quebrado por contrato inconsistente (nome de evento)
**Problema**
- Backend emite evento com nome diferente do que os apps escutam.

**Onde aparece**
- Backend emite: `EOrderStatusChanged`: [SignalRNotificationServiceWrapper.cs](file:///d:/dev/opamenu-ecosistema/opamenu-api/OpaMenu.Web/Services/SignalRNotificationServiceWrapper.cs#L62-L85)
- Painel escuta: `OrderStatusChanged`: [signalr.service.ts](file:///d:/dev/opamenu-ecosistema/opamenu-painel/src/services/signalr.service.ts#L119-L131)
- Gestor escuta: `OrderStatusChanged`: [signalr_service.dart](file:///d:/dev/opamenu-ecosistema/opamenu-gestor/lib/core/infrastructure/services/signalr_service.dart#L58-L82)

**Risco/impacto**
- Operação “não anda”: board não atualiza, cozinha perde visibilidade.

**Correção mínima (MVP seguro)**
- Padronizar e versionar contrato de eventos:
  - escolher 1 nome único (`OrderStatusChanged`) e aplicar no backend.
  - criar documento de contrato (evento, payload, grupos).

**Critério de aceite**
- Ao mudar status do pedido via API, painel/gestor atualizam em tempo real.

---

### 4) Enum/status divergentes entre backend e painel
**Problema**
- Backend e painel possuem listas diferentes/ordem diferente de status.

**Onde aparece**
- Backend: [EOrderStatus.cs](file:///d:/dev/opamenu-ecosistema/opamenu-commons/OpaMenu.Infrastructure.Shared/Enums/Opamenu/EOrderStatus.cs#L6-L16)
- Painel: [types.ts](file:///d:/dev/opamenu-ecosistema/opamenu-painel/src/features/orders/types.ts#L1-L10)

**Risco/impacto**
- Kanban incorreto, transições inválidas, filtros errados e bugs silenciosos.

**Correção mínima (MVP seguro)**
- Remover status “fantasma” (`Confirmed`) do painel ou mapear de forma explícita (sem deslocar valores).
- Preferir contrato por **string** (nome do enum) na API para evitar quebra por “valor numérico” no futuro.

**Critério de aceite**
- Um pedido com status `Preparing` no backend aparece como `Preparing` no painel.
- As transições aceitas no backend batem com os botões/ações do painel.

---

### 5) Pagamento PIX incompleto para produção (webhook não fecha ciclo do pedido)
**Problema**
- Provedor Mercado Pago está com `NotificationUrl` nulo e dados mock.
- Webhook atualiza `PaymentEntity`, mas não garante atualização do `OrderEntity`.

**Onde aparece**
- Provider: [MercadoPagoPixProvider.cs](file:///d:/dev/opamenu-ecosistema/opamenu-api/OpaMenu.Infrastructure/Services/PaymentProviders/MercadoPagoPixProvider.cs#L53-L121)
- Webhook: [PaymentsController.cs](file:///d:/dev/opamenu-ecosistema/opamenu-api/OpaMenu.Web/UserEntry/PaymentsController.cs#L79-L112)
- Processamento: [PaymentService.cs](file:///d:/dev/opamenu-ecosistema/opamenu-api/OpaMenu.Application/Services/Opamenu/PaymentService.cs#L126-L160)

**Risco/impacto**
- PIX pago e pedido não entra em produção, ou produção sem pagamento confirmado.

**Correção mínima (MVP seguro)**
- Configurar `NotificationUrl` (por tenant, ambiente e provider).
- Garantir idempotência de webhook (mesmo evento N vezes não corrompe estado).
- Ao receber pagamento confirmado, atualizar o `OrderEntity` de forma determinística (ex.: marcar `IsPaid`, ou transicionar status conforme regra definida).

**Critério de aceite**
- Pagamento confirmado no provider reflete no pedido em até X segundos.
- Reenvio de webhook não duplica eventos nem altera indevidamente o pedido.

---

### 6) Deploy/config do `docker-compose.yml` desalinhado com o que o backend lê
**Problema**
- Compose usa chaves diferentes das que o backend exige.

**Onde aparece**
- Backend exige `ConnectionStrings:OpamenuDatabase`, `AccessControlDatabase`, `MultiTenantDatabase`:
  - [ServiceCollectionExtensions.cs](file:///d:/dev/opamenu-ecosistema/opamenu-api/OpaMenu.Web/Extensions/ServiceCollectionExtensions.cs#L39-L86)
- Compose usa `ConnectionStrings__DefaultConnection` e `Jwt__Key`:
  - [docker-compose.yml](file:///d:/dev/opamenu-ecosistema/opamenu-api/docker-compose.yml#L24-L38)

**Risco/impacto**
- Ambiente não sobe, time perde tempo e bugs de runtime ficam “escondidos”.

**Correção mínima (MVP seguro)**
- Atualizar `docker-compose.yml` para setar exatamente as chaves usadas no código.
- Documentar `.env` mínimo e garantir que o compose usa o mesmo.

**Critério de aceite**
- Subir com `docker compose up` inicializa a API sem erro de configuração.

---

## Observações (crítico para o modelo de negócio)
- Se o MVP vai vender **assinatura por módulo**, o backend deve impor isso.
- Hoje existe base de dados para `TenantModule`/`PlanModule` e UI de gestão no `access-control-web`, mas o enforcement ainda precisa ser consistente dentro do `opamenu-api`.

