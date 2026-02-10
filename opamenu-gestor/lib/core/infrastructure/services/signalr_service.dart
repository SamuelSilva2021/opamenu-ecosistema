import 'dart:async';
import 'package:logging/logging.dart';
import 'package:signalr_netcore/signalr_client.dart';
import 'package:opamenu_gestor/core/config/env_config.dart';

class SignalRService {
  HubConnection? _hubConnection;
  final Logger _logger = Logger('SignalRService');
  
  // Streams controllers
  final _connectionStatusController = StreamController<HubConnectionState>.broadcast();
  final _newOrderController = StreamController<Map<String, dynamic>>.broadcast();
  final _orderStatusController = StreamController<Map<String, dynamic>>.broadcast();
  
  Stream<HubConnectionState> get connectionStatus => _connectionStatusController.stream;
  Stream<Map<String, dynamic>> get onNewOrder => _newOrderController.stream;
  Stream<Map<String, dynamic>> get onOrderStatusChanged => _orderStatusController.stream;
  
  bool get isConnected => _hubConnection?.state == HubConnectionState.Connected;

  Future<void> init() async {
    // Usando apiBaseUrl para construir a URL do Hub
    // Remove trailing slash se existir e adiciona o path do hub
    final baseUrl = EnvConfig.apiBaseUrl.endsWith('/') 
        ? EnvConfig.apiBaseUrl.substring(0, EnvConfig.apiBaseUrl.length - 1) 
        : EnvConfig.apiBaseUrl;
        
    final hubUrl = '$baseUrl/hubs/notifications';
    
    _logger.info('Inicializando SignalR em: $hubUrl');
    
    _hubConnection = HubConnectionBuilder()
        .withUrl(hubUrl)
        .withAutomaticReconnect()
        .build();
        
    _hubConnection?.onclose(({error}) {
      _logger.warning('Conexão SignalR fechada', error);
      _connectionStatusController.add(HubConnectionState.Disconnected);
    });
    
    _hubConnection?.onreconnecting(({error}) {
      _logger.info('Reconectando SignalR...', error);
      _connectionStatusController.add(HubConnectionState.Reconnecting);
    });
    
    _hubConnection?.onreconnected(({connectionId}) {
      _logger.info('SignalR Reconectado: $connectionId');
      _connectionStatusController.add(HubConnectionState.Connected);
      // Re-entrar nos grupos se necessário
      joinAdminGroup();
    });

    // Registrar listeners de eventos do servidor
    _registerEventHandlers();
  }
  
  void _registerEventHandlers() {
    _hubConnection?.on('NewOrderReceived', (arguments) {
      if (arguments != null && arguments.isNotEmpty) {
        _logger.info('Novo pedido recebido via SignalR: ${arguments[0]}');
        try {
          final data = arguments[0] as Map<String, dynamic>;
          _newOrderController.add(data);
        } catch (e) {
          _logger.severe('Erro ao processar payload de NewOrderReceived', e);
        }
      }
    });
    
    _hubConnection?.on('OrderStatusChanged', (arguments) {
      if (arguments != null && arguments.isNotEmpty) {
        _logger.info('Status de pedido alterado via SignalR: ${arguments[0]}');
        try {
          final data = arguments[0] as Map<String, dynamic>;
          _orderStatusController.add(data);
        } catch (e) {
          _logger.severe('Erro ao processar payload de OrderStatusChanged', e);
        }
      }
    });
  }

  Future<void> connect() async {
    if (_hubConnection == null) {
      await init();
    }
    
    if (_hubConnection?.state == HubConnectionState.Disconnected) {
      try {
        _logger.info('Tentando conectar ao SignalR...');
        await _hubConnection?.start();
        _logger.info('Conectado ao SignalR com sucesso');
        _connectionStatusController.add(HubConnectionState.Connected);
        
        // Entrar no grupo de admin automaticamente ao conectar
        await joinAdminGroup();
      } catch (e) {
        _logger.severe('Erro ao conectar ao SignalR', e);
        // Não relançamos o erro para não quebrar a UI, mas logamos
      }
    }
  }
  
  Future<void> disconnect() async {
    if (_hubConnection?.state == HubConnectionState.Connected) {
      _logger.info('Desconectando do SignalR...');
      await _hubConnection?.stop();
      _connectionStatusController.add(HubConnectionState.Disconnected);
    }
  }
  
  Future<void> joinAdminGroup() async {
    if (isConnected) {
      try {
        _logger.info('Entrando no grupo Administrators...');
        await _hubConnection?.invoke('JoinAdminGroup');
        _logger.info('Entrou no grupo Administrators');
      } catch (e) {
        _logger.severe('Erro ao entrar no grupo Admin', e);
      }
    } else {
      _logger.warning('Tentou entrar no grupo Admin sem estar conectado');
    }
  }
  
  void dispose() {
    _connectionStatusController.close();
    _newOrderController.close();
    _orderStatusController.close();
  }
}
