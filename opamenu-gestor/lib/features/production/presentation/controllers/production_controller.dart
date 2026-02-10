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
    // Silent refresh to keep UI stable
    final newData = await AsyncValue.guard(() => _fetchProductionOrders());
    if (newData.hasValue) {
      state = newData;
    } else if (newData.hasError) {
      // Keep showing old data but maybe log error
    }
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

    try {
      // 1. Show loading by temporarily invalidating state or setting a loading flag if needed.
      // However, to keep it smooth, we just wait for the API call.
      
      // 2. Call API
      await repository.updateOrderStatus(order.id, nextStatus.index);
      
      // 3. Force UI refresh with new data from server
      state = const AsyncValue.loading(); // Show loading indicator
      state = await AsyncValue.guard(() => _fetchProductionOrders());
    } catch (e) {
      // Restore old state if error
      // Log error or show snackbar
      state = await AsyncValue.guard(() => _fetchProductionOrders()); // Try to restore
    }
  }

  Future<void> updateItemStatus(String orderItemId, OrderStatus status) async {
    final repository = ref.read(ordersRepositoryProvider);
    
    try {
      await repository.updateOrderItemStatus(orderItemId, status.index);
      // Force refresh with loading indicator
      state = const AsyncValue.loading();
      state = await AsyncValue.guard(() => _fetchProductionOrders());
    } catch (e) {
      // Log error
      state = await AsyncValue.guard(() => _fetchProductionOrders());
    }
  }
}
