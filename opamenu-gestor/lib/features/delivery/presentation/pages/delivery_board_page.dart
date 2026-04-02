import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import '../notifiers/delivery_notifier.dart';
import '../../../pos/domain/models/order_response_dto.dart';
import 'package:intl/intl.dart';

class DeliveryBoardPage extends ConsumerWidget {
  const DeliveryBoardPage({Key? key}) : super(key: key);

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final stateAsync = ref.watch(deliveryProvider);

    return Scaffold(
      backgroundColor: const Color(0xFF0F172A), // Premium Dark
      appBar: AppBar(
        title: const Text('Gestão de Delivery', style: TextStyle(fontWeight: FontWeight.w600)),
        backgroundColor: const Color(0xFF1E293B),
        actions: [
          IconButton(
            icon: const Icon(Icons.refresh),
            tooltip: 'Atualizar Kanban',
            onPressed: () => ref.read(deliveryProvider.notifier).refresh(),
          )
        ],
      ),
      body: stateAsync.when(
        loading: () => const Center(child: CircularProgressIndicator(color: Colors.white)),
        error: (e, _) => Center(child: Text('Erro: $e', style: const TextStyle(color: Colors.red))),
        data: (state) {
          return Padding(
            padding: const EdgeInsets.all(16.0),
            child: Row(
              children: [
                _buildColumn('Novos', state.newOrders, context, ref, const Color(0xFF3B82F6)),
                const SizedBox(width: 16),
                _buildColumn('Em Preparo', state.preparingOrders, context, ref, const Color(0xFFF59E0B)),
                const SizedBox(width: 16),
                _buildColumn('Pronto', state.readyOrders, context, ref, const Color(0xFF10B981)),
                const SizedBox(width: 16),
                _buildColumn('Em Rota', state.dispatchOrders, context, ref, const Color(0xFF8B5CF6)),
              ],
            ),
          );
        },
      ),
    );
  }

  Widget _buildColumn(String title, List<OrderResponseDto> orders, BuildContext context, WidgetRef ref, Color headerColor) {
    return Expanded(
      child: Container(
        decoration: BoxDecoration(
          color: const Color(0xFF1E293B),
          borderRadius: BorderRadius.circular(12),
          border: Border.all(color: Colors.white.withOpacity(0.05)),
        ),
        child: Column(
          children: [
            Container(
              padding: const EdgeInsets.all(12),
              decoration: BoxDecoration(
                color: headerColor.withOpacity(0.1),
                borderRadius: const BorderRadius.vertical(top: Radius.circular(12)),
              ),
              child: Row(
                mainAxisAlignment: MainAxisAlignment.spaceBetween,
                children: [
                  Text(
                    title,
                    style: TextStyle(color: headerColor, fontWeight: FontWeight.bold, fontSize: 16),
                  ),
                  Container(
                    padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 2),
                    decoration: BoxDecoration(color: headerColor, borderRadius: BorderRadius.circular(12)),
                    child: Text('${orders.length}', style: const TextStyle(color: Colors.white, fontWeight: FontWeight.bold)),
                  )
                ],
              ),
            ),
            Expanded(
              child: ListView.separated(
                padding: const EdgeInsets.all(12),
                itemCount: orders.length,
                separatorBuilder: (c, i) => const SizedBox(height: 12),
                itemBuilder: (c, i) => _DeliveryOrderCard(order: orders[i]),
              ),
            )
          ],
        ),
      ),
    );
  }
}

class _DeliveryOrderCard extends StatelessWidget {
  final OrderResponseDto order; 
  
  const _DeliveryOrderCard({Key? key, required this.order}) : super(key: key);

  String _timeAgo(DateTime date) {
    final diff = DateTime.now().difference(date);
    if (diff.inMinutes < 1) return 'Agora';
    if (diff.inHours < 1) return '${diff.inMinutes}m atrás';
    if (diff.inDays < 1) return '${diff.inHours}h atrás';
    return DateFormat('dd/MM HH:mm').format(date);
  }

  @override
  Widget build(BuildContext context) {
    final shortId = order.orderNumber?.toString() ?? order.id.substring(0, 4);

    return Container(
      padding: const EdgeInsets.all(12),
      decoration: BoxDecoration(
        color: const Color(0xFF334155),
        borderRadius: BorderRadius.circular(8),
        boxShadow: [BoxShadow(color: Colors.black.withOpacity(0.1), blurRadius: 4, offset: const Offset(0, 2))],
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Row(
            mainAxisAlignment: MainAxisAlignment.spaceBetween,
            children: [
              Text('#$shortId', style: const TextStyle(color: Colors.white, fontWeight: FontWeight.bold)),
              Text(_timeAgo(order.createdAt), style: const TextStyle(color: Colors.white54, fontSize: 12)),
            ],
          ),
          const SizedBox(height: 8),
          Text(order.customerName, style: const TextStyle(color: Colors.white, fontSize: 14)),
          const SizedBox(height: 4),
          Row(
            children: [
              const Icon(Icons.delivery_dining, size: 14, color: Colors.amber),
              const SizedBox(width: 4),
              Text(order.driverName ?? 'Sem entregador', style: TextStyle(color: order.driverName != null ? Colors.white : Colors.white54, fontSize: 12)),
            ],
          )
        ],
      ),
    );
  }
}
