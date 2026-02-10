# Arquitetura de Notificações em Tempo Real (SignalR)

Este documento descreve a modelagem e integração do sistema de notificações em tempo real entre o backend `opamenu-api` e o frontend `opamenu-gestor` utilizando SignalR.

## 1. Visão Geral

O sistema utiliza WebSockets (com fallback para Long Polling) para enviar atualizações instantâneas do servidor para os clientes conectados.

*   **Protocolo**: SignalR (.NET Core).
*   **Hub Endpoint**: `/hubs/notifications`.
*   **Transporte**: WebSockets (preferencial).

---

## 2. Estrutura do Backend (Existente)

O backend (`opamenu-api`) já possui o `OrderNotificationHub` implementado com a seguinte estrutura:

### 2.1 Canais (Groups)

| Grupo | Descrição | Público Alvo |
| :--- | :--- | :--- |
| `Administrators` | Recebe todos os novos pedidos e mudanças de status globais. | Painel do Gestor / Cozinha (KDS) |
| `Order_{id}` | Recebe atualizações de um pedido específico. | Cliente final (App de Pedidos) |
| `MenuUpdates` | Recebe avisos de pausa/ativação de produtos. | Cardápio Digital / Totem |

### 2.2 Métodos do Cliente (Upstream)

Métodos que o frontend pode invocar no Hub:

*   `JoinAdminGroup()`: Registra a conexão para receber eventos administrativos.
*   `JoinOrderGroup(int orderId)`: Registra para acompanhar um pedido.
*   `JoinMenuGroup()`: Registra para atualizações de cardápio.
*   `LeaveGroup(string groupName)`: Sai de um canal.
*   `Ping()`: Mantém a conexão ativa (Health Check).

### 2.3 Eventos do Servidor (Downstream)

Eventos disparados pelo servidor para os clientes:

| Evento | Payload (Exemplo) | Destino | Descrição |
| :--- | :--- | :--- | :--- |
| `NewOrderReceived` | `{ OrderId: 123, TotalAmount: 50.00, Items: [...] }` | `Administrators` | Novo pedido criado. Deve tocar som e atualizar Kanban. |
| `OrderStatusChanged` | `{ OrderId: 123, NewStatus: "Preparing", OldStatus: "Pending" }` | `Administrators`, `Order_{id}` | Pedido avançou de etapa. |
| `OrderReady` | `{ OrderId: 123, Message: "Seu pedido está pronto!" }` | `Order_{id}` | Notificação para o cliente retirar. |

---

## 3. Modelagem no Frontend (Opamenu Gestor)

A implementação no Flutter seguirá o padrão Service/Repository com Riverpod.

### 3.1 Dependências
*   `signalr_netcore`: Cliente SignalR compatível com .NET Core.

### 3.2 Camada de Serviço (`RealtimeService`)

Classe singleton responsável por manter a conexão persistente.

```dart
abstract class RealtimeService {
  Future<void> connect();
  Future<void> disconnect();
  Future<void> joinAdminChannel();
  
  // Streams para reagir na UI
  Stream<OrderDto> get onNewOrder;
  Stream<OrderStatusUpdateDto> get onOrderStatusChanged;
}
```

### 3.3 Integração com Estado (Riverpod)

1.  **`realtimeProvider`**: Provê a instância do serviço.
2.  **`ConnectionStatusProvider`**: Monitora se está Conectado/Desconectado/Reconectando.
3.  **Integração KDS (`ProductionController`)**:
    *   Ao iniciar, o controller se inscreve no stream `onNewOrder`.
    *   Quando receber evento: Adiciona o pedido à lista local sem recarregar toda a API.
    *   **Benefício**: Redução drástica de chamadas HTTP (polling) e latência zero.

### 3.4 Fluxo de Conexão

1.  App inicia -> Verifica Login.
2.  Se Logado -> `RealtimeService.connect()`.
3.  Após conexão ("Connected") -> Chama `JoinAdminGroup()`.
4.  App fica em "Listening Mode".
5.  Logout -> `RealtimeService.disconnect()`.

---

## 4. Segurança

*   **Atual**: O Hub é público (sem `[Authorize]`).
*   **Futuro (Recomendado)**: Adicionar autenticação JWT na conexão SignalR.
    *   Backend: Adicionar `[Authorize]` no Hub.
    *   Frontend: Enviar `access_token` via Query Parameter ou Header na negociação do WebSocket.

## 5. Plano de Implementação

1.  [ ] Adicionar pacote `signalr_netcore` ao `pubspec.yaml`.
2.  [ ] Criar `RealtimeService` em `core/infrastructure/services`.
3.  [ ] Configurar injeção de dependência (Riverpod).
4.  [ ] Integrar com `Dashboard` (Contador de pedidos) e `ProductionPage` (KDS).
