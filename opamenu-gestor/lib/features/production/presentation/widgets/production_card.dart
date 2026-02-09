import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:intl/intl.dart';
import '../../../pos/domain/models/order_response_dto.dart';
import '../../../pos/domain/enums/order_status.dart';
import '../../../../core/theme/app_colors.dart';
import '../../../../core/services/printer_service.dart';
import '../../../../core/utils/receipt_generator.dart';
import 'package:esc_pos_utils_plus/esc_pos_utils.dart' as esc;
import '../controllers/production_controller.dart';
import '../../../../features/settings/presentation/providers/settings_notifier.dart';

class ProductionCard extends ConsumerWidget {
  final OrderResponseDto order;
  final VoidCallback onNextStatus;

  const ProductionCard({
    super.key,
    required this.order,
    required this.onNextStatus,
  });

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final theme = Theme.of(context);

    return Card(
      elevation: 4,
      shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(12)),
      child: Container(
        width: 300,
        padding: const EdgeInsets.all(16),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Row(
              mainAxisAlignment: MainAxisAlignment.spaceBetween,
              children: [
                Row(
                  children: [
                    Text(
                      '#${order.queuePosition}',
                      style: theme.textTheme.headlineSmall?.copyWith(fontWeight: FontWeight.bold),
                    ),
                    const SizedBox(width: 8),
                    IconButton(
                      icon: const Icon(Icons.print, size: 20, color: Colors.grey),
                      padding: EdgeInsets.zero,
                      constraints: const BoxConstraints(),
                      tooltip: 'Imprimir Pedido',
                      onPressed: () async {
                        try {
                          // Get Printer Settings
                          final settings = await ref.read(settingsProvider.future);
                          final printerIp = settings['kitchen'] ?? '192.168.1.100';

                          final profile = await esc.CapabilityProfile.load();
                          final bytes = await ReceiptGenerator.generateOrderReceipt(
                            order: order,
                            paperSize: PaperSize.mm80,
                            profile: profile,
                            restaurantName: settings['name'],
                            restaurantAddress: settings['address'],
                            restaurantPhone: settings['phone'],
                          );

                          final printerService = ref.read(printerServiceProvider.notifier);
                          final device = PrinterDeviceInfo(
                            name: 'Impressora Cozinha',
                            address: printerIp,
                            type: PrinterConnectionType.network,
                          );

                          final success = await printerService.printReceipt(device, bytes);
                          if (context.mounted) {
                            ScaffoldMessenger.of(context).showSnackBar(
                              SnackBar(
                                content: Text(success ? 'Cupom impresso!' : 'Erro na impressora'),
                                backgroundColor: success ? Colors.green : Colors.red,
                                duration: const Duration(seconds: 1),
                              ),
                            );
                          }
                        } catch (e) {
                          if (context.mounted) {
                            ScaffoldMessenger.of(context).showSnackBar(
                              SnackBar(content: Text('Erro: $e'), backgroundColor: Colors.red),
                            );
                          }
                        }
                      },
                    ),
                  ],
                ),
                _OrderTimer(createdAt: order.createdAt),
              ],
            ),
            const SizedBox(height: 8),
            Text(
              order.customerName,
              style: theme.textTheme.titleMedium?.copyWith(fontWeight: FontWeight.bold),
              maxLines: 1,
              overflow: TextOverflow.ellipsis,
            ),
            const Divider(),
            Expanded(
              child: ListView.builder(
                itemCount: order.items.length,
                itemBuilder: (context, index) {
                  final item = order.items[index];
                  final isItemReady = item.status == OrderStatus.ready.index;

                  return Padding(
                    padding: const EdgeInsets.symmetric(vertical: 4),
                    child: InkWell(
                      onTap: () {
                        final nextStatus = isItemReady ? OrderStatus.preparing : OrderStatus.ready;
                        ref.read(productionOrdersProvider.notifier).updateItemStatus(item.id, nextStatus);
                      },
                      child: Row(
                        crossAxisAlignment: CrossAxisAlignment.start,
                        children: [
                          Icon(
                            isItemReady ? Icons.check_circle : Icons.radio_button_unchecked,
                            color: isItemReady ? Colors.green : Colors.grey,
                            size: 20,
                          ),
                          const SizedBox(width: 8),
                          Text('${item.quantity}x ', style: const TextStyle(fontWeight: FontWeight.bold)),
                          Expanded(
                            child: Column(
                              crossAxisAlignment: CrossAxisAlignment.start,
                              children: [
                                Text(
                                  item.productName,
                                  style: TextStyle(
                                    decoration: isItemReady ? TextDecoration.lineThrough : null,
                                    color: isItemReady ? Colors.grey : null,
                                  ),
                                ),
                                if (item.notes != null && item.notes!.isNotEmpty)
                                  Text(
                                    item.notes!,
                                    style: const TextStyle(fontSize: 12, color: Colors.red, fontStyle: FontStyle.italic),
                                  ),
                              ],
                            ),
                          ),
                        ],
                      ),
                    ),
                  );
                },
              ),
            ),
            const SizedBox(height: 16),
            SizedBox(
              width: double.infinity,
              child: ElevatedButton(
                onPressed: onNextStatus,
                style: ElevatedButton.styleFrom(
                  backgroundColor: _getButtonColor(order.status),
                  foregroundColor: Colors.white,
                  padding: const EdgeInsets.symmetric(vertical: 16),
                  shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(8)),
                ),
                child: Text(
                  _getButtonText(order.status, order.isDelivery),
                  style: const TextStyle(fontWeight: FontWeight.bold),
                ),
              ),
            ),
          ],
        ),
      ),
    );
  }

  Color _getButtonColor(OrderStatus status) {
    switch (status) {
      case OrderStatus.pending:
        return Colors.orange;
      case OrderStatus.preparing:
        return Colors.blue;
      case OrderStatus.ready:
        return Colors.green;
      case OrderStatus.outForDelivery:
        return Colors.purple;
      default:
        return AppColors.primary;
    }
  }

  String _getButtonText(OrderStatus status, bool isDelivery) {
    if (status == OrderStatus.pending) return 'INICIAR PREPARO';
    if (status == OrderStatus.preparing) return 'PRONTO';
    if (status == OrderStatus.ready) {
      return isDelivery ? 'SAIU P/ ENTREGA' : 'ENTREGUE';
    }
    if (status == OrderStatus.outForDelivery) return 'ENTREGUE';
    return 'PRÓXIMO';
  }
}

class _OrderTimer extends StatefulWidget {
  final DateTime createdAt;

  const _OrderTimer({required this.createdAt});

  @override
  State<_OrderTimer> createState() => _OrderTimerState();
}

class _OrderTimerState extends State<_OrderTimer> {
  late Duration _waitingTime;
  late bool _isLongWaiting;
  
  @override
  void initState() {
    super.initState();
    _updateTime();
    // Update every minute
    Future.doWhile(() async {
      if (!mounted) return false;
      await Future.delayed(const Duration(seconds: 60));
      if (mounted) {
        setState(() => _updateTime());
      }
      return mounted;
    });
  }

  void _updateTime() {
    _waitingTime = DateTime.now().difference(widget.createdAt);
    _isLongWaiting = _waitingTime.inMinutes > 15;
  }

  @override
  Widget build(BuildContext context) {
    return Container(
      padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 4),
      decoration: BoxDecoration(
        color: _isLongWaiting ? Colors.red.withOpacity(0.1) : Colors.green.withOpacity(0.1),
        borderRadius: BorderRadius.circular(8),
      ),
      child: Text(
        '${_waitingTime.inMinutes} min',
        style: TextStyle(
          color: _isLongWaiting ? Colors.red : Colors.green,
          fontWeight: FontWeight.bold,
        ),
      ),
    );
  }
}
