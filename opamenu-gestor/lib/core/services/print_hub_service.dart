import 'package:riverpod_annotation/riverpod_annotation.dart';
import 'package:signalr_netcore/signalr_client.dart';
import '../config/env_config.dart';
import '../models/print_job_dto.dart';
import 'printer_service.dart';

part 'print_hub_service.g.dart';

@riverpod
class PrintHubService extends _$PrintHubService {
  HubConnection? _connection;

  @override
  void build() {
    _connect();
    ref.onDispose(dispose);
  }

  Future<void> _connect() async {
    _connection = HubConnectionBuilder()
        .withUrl('${EnvConfig.apiBaseUrl}/hubs/print')
        .withAutomaticReconnect()
        .build();

    _connection!.on('OnNewOrder', _handleIncomingOrder);
    await _connection!.start();
  }

  void _handleIncomingOrder(List<Object?>? args) async {
    if (args == null || args.isEmpty) return;
    
    try {
      final job = PrintJobDto.fromJson(args[0] as Map<String, dynamic>);
      
      // TODO: Get mapped printer for job.destination
      // For now, generating bytes as proof of concept
      final bytes = await ref.read(printerServiceProvider.notifier).generateOrderTicket(job, PaperSize.mm80);
      
      // ref.read(printerServiceProvider.notifier).printReceipt(mappedPrinter, bytes);
    } catch (e) {
      print('SignalR PrintHub error: $e');
    }
  }

  Future<void> dispose() => _connection?.stop() ?? Future.value();
}
