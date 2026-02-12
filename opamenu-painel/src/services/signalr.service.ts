import { HubConnection, HubConnectionBuilder, LogLevel, HubConnectionState } from "@microsoft/signalr";

type OrderNotification = {
  orderId: string;
  status: number;
  [key: string]: any;
};

type NewOrderNotification = {
  orderId: string;
  customerName: string;
  totalAmount: number;
  [key: string]: any;
};

class SignalRService {
  private connection: HubConnection | null = null;
  private callbacks: { [key: string]: ((data: any) => void)[] } = {};
  private static instance: SignalRService;
  private connectPromise: Promise<void> | null = null;
  private token: string | null = null;

  private constructor() { }

  public static getInstance(): SignalRService {
    if (!SignalRService.instance) {
      SignalRService.instance = new SignalRService();
    }
    return SignalRService.instance;
  }

  public async connect(token?: string): Promise<void> {
    // Update token if provided
    if (token) this.token = token;

    // If already connected, return immediately
    if (this.connection?.state === HubConnectionState.Connected) {
      console.log("SignalR já conectado.");
      return;
    }

    // If connection is in progress, return the existing promise
    if (this.connectPromise) {
      console.log("SignalR: Aguardando conexão em andamento...");
      return this.connectPromise;
    }

    // Create a new connection promise
    this.connectPromise = this.performConnect();

    try {
      await this.connectPromise;
    } finally {
      this.connectPromise = null;
    }
  }

  private async performConnect(): Promise<void> {
    try {
      // Build connection if it doesn't exist
      if (!this.connection) {
        const apiUrl = (import.meta.env.VITE_API_URL || "https://opamenu-production.up.railway.app/api").replace(/\/$/, "");
        const baseUrl = apiUrl.endsWith("/api") ? apiUrl.slice(0, -4) : apiUrl;
        const hubUrl = `${baseUrl}/hubs/notifications`;

        console.log(`Iniciando conexão SignalR em: ${hubUrl}`);

        this.connection = new HubConnectionBuilder()
          .withUrl(hubUrl, {
            accessTokenFactory: () => this.token || "",
            withCredentials: true,
          })
          .withAutomaticReconnect({
            nextRetryDelayInMilliseconds: retryContext => {
              if (retryContext.previousRetryCount === 0) return 0;
              if (retryContext.previousRetryCount < 3) return 2000;
              if (retryContext.previousRetryCount < 5) return 10000;
              return 30000;
            }
          })
          .configureLogging(LogLevel.Information)
          .build();

        this.connection.onclose((error) => {
          console.error("SignalR: Conexão fechada!", error);
        });

        this.connection.onreconnecting((error) => {
          console.warn("SignalR: Reconectando...", error);
        });

        this.connection.onreconnected((connectionId) => {
          console.log(`SignalR: Reconectado! ID: ${connectionId}`);
          this.joinAdminGroup();
        });

        this.registerHandlers();
      }

      // Start the connection if it's disconnected
      if (this.connection.state === HubConnectionState.Disconnected) {
        await this.connection.start();
        await this.joinAdminGroup();
      }

    } catch (err) {
      // Clean up on error
      this.connection = null;

      if (err instanceof Error && (err.name === "AbortError" || err.message.includes("AbortError"))) {
        console.log("SignalR: Conexão cancelada (AbortError).");
        return;
      }
      console.error("❌ Erro ao conectar no SignalR: ", err);
      throw err;
    }
  }

  private registerHandlers() {
    if (!this.connection) return;

    this.connection.on("NewOrderReceived", (data) => {
      console.log("🔔 SignalR Evento Recebido: NewOrderReceived", data);
      this.emit("NewOrderReceived", data);
    });

    this.connection.on("OrderStatusChanged", (data) => {
      console.log("🔔 SignalR Evento Recebido: OrderStatusChanged", data);
      this.emit("OrderStatusChanged", data);
    });
  }

  public async disconnect(): Promise<void> {
    // Wait for any ongoing connection attempt to complete
    if (this.connectPromise) {
      try {
        await this.connectPromise;
      } catch {
        // Ignore errors from connection attempt
      }
    }

    if (this.connection) {
      try {
        await this.connection.stop();
        console.log("SignalR desconectado.");
      } catch (err) {
        console.error("Erro ao desconectar SignalR:", err);
      } finally {
        this.connection = null;
        this.connectPromise = null;
      }
    }
  }

  public async joinAdminGroup(): Promise<void> {
    if (this.connection?.state === HubConnectionState.Connected) {
      try {
        await this.connection.invoke("JoinAdminGroup");
        console.log("Joined Admin Group");
      } catch (err) {
        console.error("Error joining admin group", err);
      }
    }
  }

  public on(event: string, callback: (data: any) => void): void {
    if (!this.callbacks[event]) {
      this.callbacks[event] = [];
    }
    this.callbacks[event].push(callback);
  }

  public off(event: string, callback: (data: any) => void): void {
    if (this.callbacks[event]) {
      this.callbacks[event] = this.callbacks[event].filter(cb => cb !== callback);
    }
  }

  private emit(event: string, data: any): void {
    if (this.callbacks[event]) {
      this.callbacks[event].forEach(callback => callback(data));
    }
  }

  public getConnectionState(): HubConnectionState {
    return this.connection?.state || HubConnectionState.Disconnected;
  }
}

export const signalRService = SignalRService.getInstance();
