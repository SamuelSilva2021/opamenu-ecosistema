import 'package:riverpod_annotation/riverpod_annotation.dart';
import '../../../pos/domain/models/order_response_dto.dart';
import '../../../pos/domain/enums/order_status.dart';
import '../../../pos/data/repositories/orders_repository.dart';

part 'production_controller.g.dart';

@riverpod
class ProductionOrders extends _$ProductionOrders {
  @override
  Future<List<OrderResponseDto>> build() async {
    return _fetchProductionOrders();
  }

  Future<List<OrderResponseDto>> _fetchProductionOrders() async {
    final repository = ref.read(ordersRepositoryProvider);
    
    // Fetch active orders (Pending, Preparing, Ready)
    final pending = await repository.getOrdersByStatus(OrderStatus.pending.index);
    final preparing = await repository.getOrdersByStatus(OrderStatus.preparing.index);
    final ready = await repository.getOrdersByStatus(OrderStatus.ready.index);
    
    final allOrders = [...pending, ...preparing, ...ready];
    allOrders.sort((a, b) => a.createdAt.compareTo(b.createdAt));
    
    return allOrders;
  }

  Future<void> refresh() async {
    state = const AsyncValue.loading();
    state = await AsyncValue.guard(() => _fetchProductionOrders());
  }

  Future<void> moveNextStatus(OrderResponseDto order) async {
    final repository = ref.read(ordersRepositoryProvider);
    OrderStatus nextStatus;

    if (order.status == OrderStatus.pending) {
      nextStatus = OrderStatus.preparing;
    } else if (order.status == OrderStatus.preparing) {
      nextStatus = OrderStatus.ready;
    } else if (order.status == OrderStatus.ready && order.isDelivery) {
      nextStatus = OrderStatus.outForDelivery;
    } else if (order.status == OrderStatus.outForDelivery || (order.status == OrderStatus.ready && !order.isDelivery)) {
      nextStatus = OrderStatus.delivered;
    } else {
      return;
    }

    await repository.updateOrderStatus(order.id, nextStatus.index);
    await refresh();
  }

  Future<void> updateItemStatus(String orderItemId, OrderStatus status) async {
    final repository = ref.read(ordersRepositoryProvider);
    await repository.updateOrderItemStatus(orderItemId, status.index);
    await refresh();
  }
}
