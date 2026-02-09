import 'package:flutter/material.dart';
import 'package:intl/intl.dart';
import '../../../pos/domain/models/order_response_dto.dart';
import '../../../pos/domain/enums/order_status.dart';
import '../../../../core/theme/app_colors.dart';

class ProductionCard extends StatelessWidget {
  final OrderResponseDto order;
  final VoidCallback onNextStatus;

  const ProductionCard({
    super.key,
    required this.order,
    required this.onNextStatus,
  });

  @override
  Widget build(BuildContext context) {
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
                Text(
                  '#${order.queuePosition}',
                  style: theme.textTheme.headlineSmall?.copyWith(fontWeight: FontWeight.bold),
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
                  return Padding(
                    padding: const EdgeInsets.symmetric(vertical: 4),
                    child: Row(
                      crossAxisAlignment: CrossAxisAlignment.start,
                      children: [
                        Text('${item.quantity}x ', style: const TextStyle(fontWeight: FontWeight.bold)),
                        Expanded(
                          child: Column(
                            crossAxisAlignment: CrossAxisAlignment.start,
                            children: [
                              Text(item.productName),
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
                  backgroundColor: order.status == OrderStatus.confirmed ? AppColors.primary : Colors.green,
                  foregroundColor: Colors.white,
                  padding: const EdgeInsets.symmetric(vertical: 16),
                  shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(8)),
                ),
                child: Text(
                  order.status == OrderStatus.confirmed ? 'COMEÇAR' : 'PRONTO',
                  style: const TextStyle(fontWeight: FontWeight.bold),
                ),
              ),
            ),
          ],
        ),
      ),
    );
  }
}
