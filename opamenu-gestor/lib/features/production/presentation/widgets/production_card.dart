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
    final waitingTime = DateTime.now().difference(order.createdAt);
    final isLongWaiting = waitingTime.inMinutes > 15;

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
                          final profile = await esc.CapabilityProfile.load();
                          final bytes = await ReceiptGenerator.generateOrderReceipt(
                            order: order,
                            paperSize: PaperSize.mm80,
                            profile: profile,
                          );

                          final printerService = ref.read(printerServiceProvider.notifier);
                          final device = PrinterDeviceInfo(
                            name: 'Impressora Cozinha',
                            address: '192.168.1.100',
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
                Container(
                  padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 4),
                  decoration: BoxDecoration(
                    color: isLongWaiting ? Colors.red.withOpacity(0.1) : Colors.green.withOpacity(0.1),
                    borderRadius: BorderRadius.circular(8),
                  ),
                  child: Text(
                    '${waitingTime.inMinutes} min',
                    style: TextStyle(
                      color: isLongWaiting ? Colors.red : Colors.green,
                      fontWeight: FontWeight.bold,
                    ),
                  ),
                ),
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
      case OrderStatus.preparing:
        return Colors.green;
      case OrderStatus.ready:
        return AppColors.primary;
      case OrderStatus.outForDelivery:
        return Colors.orange;
      default:
        return AppColors.primary;
    }
  }

  String _getButtonText(OrderStatus status, bool isDelivery) {
    if (status == OrderStatus.preparing) return 'PRONTO';
    if (status == OrderStatus.ready) {
      return isDelivery ? 'SAIU P/ ENTREGA' : 'ENTREGUE';
    }
    if (status == OrderStatus.outForDelivery) return 'ENTREGUE';
    return 'PRÓXIMO';
  }
}
