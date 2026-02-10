import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:signalr_netcore/signalr_client.dart';
import 'package:opamenu_gestor/core/infrastructure/services/signalr_service.dart';

// Provider global do serviço (Singleton)
final signalRServiceProvider = Provider<SignalRService>((ref) {
  final service = SignalRService();
  ref.onDispose(() => service.dispose());
  return service;
});

// Stream do status da conexão
final signalRConnectionStatusProvider = StreamProvider<HubConnectionState>((ref) {
  final service = ref.watch(signalRServiceProvider);
  return service.connectionStatus;
});

// Stream de novos pedidos
final newOrderStreamProvider = StreamProvider<Map<String, dynamic>>((ref) {
  final service = ref.watch(signalRServiceProvider);
  return service.onNewOrder;
});

// Stream de mudança de status
final orderStatusStreamProvider = StreamProvider<Map<String, dynamic>>((ref) {
  final service = ref.watch(signalRServiceProvider);
  return service.onOrderStatusChanged;
});
