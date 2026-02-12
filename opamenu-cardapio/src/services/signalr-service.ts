import * as signalR from '@microsoft/signalr';
import { API_BASE_URL } from '@/config/api';

class SignalRService {
  private connection: signalR.HubConnection | null = null;
  private connectionPromise: Promise<void> | null = null;
  private statusUpdateCallbacks: Set<Function> = new Set();
  private otherEventsCallbacks: Map<string, Set<Function>> = new Map();

  private getHubUrl(): string {
    let baseUrl = API_BASE_URL;
    
    if (baseUrl.startsWith('/')) {
      return `${window.location.origin}/hubs/notifications`;
    }
    if (baseUrl.endsWith('/api')) {
      baseUrl = baseUrl.slice(0, -4);
    }
    
    return `${baseUrl}/hubs/notifications`;
  }

  public async startConnection(): Promise<void> {
    if (this.connection?.state === signalR.HubConnectionState.Connected) {
      return;
    }

    if (this.connectionPromise) {
      return this.connectionPromise;
    }

    const hubUrl = this.getHubUrl();
    console.log('Iniciando conexão SignalR em:', hubUrl);

    this.connection = new signalR.HubConnectionBuilder()
      .withUrl(hubUrl, {
        skipNegotiation: true,
        transport: signalR.HttpTransportType.WebSockets
      })
      .withAutomaticReconnect()
      .configureLogging(signalR.LogLevel.Information)
      .build();

    this.connection.onclose((error) => {
      console.log('Conexão SignalR fechada', error);
      this.connectionPromise = null;
    });

    this.connectionPromise = this.connection.start()
      .then(() => {
        console.log('SignalR Conectado!');
        this.setupListeners();
      })
      .catch((err) => {
        console.error('Erro ao conectar SignalR:', err);
        this.connectionPromise = null;
        throw err;
      });

    return this.connectionPromise;
  }

  private setupListeners() {
    if (!this.connection) return;

    this.connection.on('EOrderStatusUpdated', (data: any) => {
      console.log('Evento EOrderStatusUpdated recebido:', data);
      const orderId = data?.orderId || data?.OrderId;
      const newStatus = data?.newStatus || data?.NewStatus || data?.status || data?.Status;
      if (orderId && (newStatus !== undefined)) {
        this.statusUpdateCallbacks.forEach(callback => callback(orderId, newStatus));
      }
    });

    const otherEvents = ['OrderAccepted', 'OrderReady', 'OrderCompleted'];
    otherEvents.forEach(event => {
      this.connection?.on(event, (data: any) => {
        console.log(`Evento ${event} recebido:`, data);
        const id = data?.orderId || data?.OrderId;
        if (id) {
          this.otherEventsCallbacks.get(event)?.forEach(callback => callback(id, 'UPDATED'));
        }
      });
    });
  }

  public async joinOrderGroup(orderId: string | number): Promise<void> {
    await this.startConnection();

    if (!this.connection) return;

    // Garantir que o ID seja uma string para compatibilidade com GUIDs
    const idString = String(orderId);

    try {
      await this.connection.invoke('JoinOrderGroup', idString);
      console.log(`Entrou no grupo do pedido ${idString}`);
    } catch (err) {
      console.error(`Erro ao entrar no grupo do pedido ${idString}:`, err);
    }
  }

  public async leaveOrderGroup(orderId: string | number): Promise<void> {
    if (!this.connection || this.connection.state !== signalR.HubConnectionState.Connected) return;

    const idString = String(orderId);
    const groupName = `Order_${idString}`;

    try {
      await this.connection.invoke('LeaveGroup', groupName);
    } catch (err) {
      console.error(`Erro ao sair do grupo ${groupName}:`, err);
    }
  }

  public onOrderStatusUpdated(callback: (orderId: string | number, status: string | number) => void): () => void {
    this.statusUpdateCallbacks.add(callback);
    return () => {
      this.statusUpdateCallbacks.delete(callback);
    };
  }

  public onOtherEvent(event: string, callback: (orderId: string | number, status: string | number) => void): () => void {
    if (!this.otherEventsCallbacks.has(event)) {
      this.otherEventsCallbacks.set(event, new Set());
    }
    this.otherEventsCallbacks.get(event)?.add(callback);
    return () => {
      this.otherEventsCallbacks.get(event)?.delete(callback);
    };
  }

  public stopConnection() {
    if (this.connection) {
      this.connection.stop();
      this.connection = null;
      this.connectionPromise = null;
    }
  }
}

export const signalRService = new SignalRService();
