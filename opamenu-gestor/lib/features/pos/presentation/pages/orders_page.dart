import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import '../controllers/orders_controller.dart';
import '../widgets/order_details.dart';
import '../widgets/order_list_item.dart';

class OrdersPage extends ConsumerWidget {
  const OrdersPage({super.key});

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final ordersAsync = ref.watch(ordersControllerProvider);
    final selectedOrderId = ref.watch(selectedOrderIdProvider);
    final totalOrders = ref.watch(totalOrdersCountProvider);
    final currentPage = ref.watch(ordersPaginationProvider);
    final totalPages = (totalOrders / 10).ceil(); // PageSize = 10

    final selectedOrder = ordersAsync.value?.data?.where((o) => o.id == selectedOrderId).firstOrNull;

    return Row(
      crossAxisAlignment: CrossAxisAlignment.stretch,
      children: [
        // Lista de Pedidos (Lado Esquerdo)
        Expanded(
          flex: 4,
          child: Container(
            color: Colors.white,
            child: Column(
              children: [
                // Header
                Padding(
                  padding: const EdgeInsets.all(24.0),
                  child: Row(
                    children: [
                      const Text(
                        'All Orders',
                        style: TextStyle(
                          fontSize: 20,
                          fontWeight: FontWeight.bold,
                        ),
                      ),
                      const Spacer(),
                      IconButton(
                        onPressed: () => ref.refresh(ordersControllerProvider),
                        icon: const Icon(Icons.refresh),
                      ),
                    ],
                  ),
                ),
                const Divider(height: 1),
                
                // Lista
                Expanded(
                  child: ordersAsync.when(
                    data: (pagedResponse) {
                      final orders = pagedResponse.data ?? [];
                      if (orders.isEmpty) {
                        return const Center(child: Text('Nenhum pedido encontrado'));
                      }
                      return ListView.builder(
                        itemCount: orders.length,
                        itemBuilder: (context, index) {
                          final order = orders[index];
                          return OrderListItem(
                            order: order,
                            isSelected: order.id == selectedOrderId,
                            onTap: () {
                              ref
                                  .read(selectedOrderIdProvider.notifier)
                                  .select(order.id);
                            },
                          );
                        },
                      );
                    },
                    loading: () => const Center(child: CircularProgressIndicator()),
                    error: (error, stack) => Center(
                      child: Column(
                        mainAxisAlignment: MainAxisAlignment.center,
                        children: [
                          const Icon(Icons.error_outline, color: Colors.red, size: 48),
                          const SizedBox(height: 16),
                          Text(
                            'Erro ao carregar pedidos',
                            style: TextStyle(color: Colors.grey[800], fontWeight: FontWeight.bold),
                          ),
                          const SizedBox(height: 8),
                          Text(
                            error.toString(),
                            style: const TextStyle(color: Colors.grey),
                            textAlign: TextAlign.center,
                          ),
                          const SizedBox(height: 16),
                          ElevatedButton(
                            onPressed: () => ref.refresh(ordersControllerProvider),
                            child: const Text('Tentar novamente'),
                          ),
                        ],
                      ),
                    ),
                  ),
                ),
                
                // Paginação
                Container(
                  padding: const EdgeInsets.all(16),
                  decoration: BoxDecoration(
                    border: Border(top: BorderSide(color: Colors.grey[200]!)),
                  ),
                  child: Row(
                    mainAxisAlignment: MainAxisAlignment.spaceBetween,
                    children: [
                      Text('Page $currentPage of ${totalPages == 0 ? 1 : totalPages}'),
                      Row(
                        children: [
                          IconButton(
                            onPressed: currentPage > 1
                                ? () => ref
                                    .read(ordersPaginationProvider.notifier)
                                    .previousPage()
                                : null,
                            icon: const Icon(Icons.chevron_left),
                          ),
                          IconButton(
                            onPressed: currentPage < totalPages
                                ? () => ref
                                    .read(ordersPaginationProvider.notifier)
                                    .nextPage()
                                : null,
                            icon: const Icon(Icons.chevron_right),
                          ),
                        ],
                      ),
                    ],
                  ),
                ),
              ],
            ),
          ),
        ),
        
        // Detalhes (Lado Direito)
        Expanded(
          flex: 6,
          child: Container(
            color: const Color(0xFFF9FAFB),
            child: selectedOrder == null
                ? const Center(
                    child: Column(
                      mainAxisAlignment: MainAxisAlignment.center,
                      children: [
                        Icon(Icons.receipt_long, size: 64, color: Colors.grey),
                        SizedBox(height: 16),
                        Text(
                          'Selecione um pedido para ver os detalhes',
                          style: TextStyle(
                            fontSize: 18,
                            color: Colors.grey,
                            fontWeight: FontWeight.w500,
                          ),
                        ),
                      ],
                    ),
                  )
                : OrderDetails(order: selectedOrder),
          ),
        ),
      ],
    );
  }
}
