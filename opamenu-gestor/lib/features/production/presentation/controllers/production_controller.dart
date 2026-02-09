import 'package:opamenu_gestor/features/pos/domain/models/order_item_response_dto.dart';
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
      // state = newData; // Don't replace state with error if we have data
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

    // Optimistic Update
    final previousState = state;
    if (state.hasValue) {
      final currentList = state.value!;
      final updatedList = currentList.map((o) {
        if (o.id == order.id) {
          return o.copyWith(status: nextStatus);
        }
        return o;
      }).toList();
      
      // Re-sort based on new status/time if needed or just update
      // For Kanban columns, changing status automatically moves the card
      state = AsyncValue.data(updatedList);
    }

    try {
      await repository.updateOrderStatus(order.id, nextStatus.index);
      // Silent refresh to ensure data consistency
      await refresh();
    } catch (e) {
      // Revert on error
      state = previousState;
    }
  }

  Future<void> updateItemStatus(String orderItemId, OrderStatus status) async {
    final repository = ref.read(ordersRepositoryProvider);
    
    // Optimistic Update
    final previousState = state;
    if (state.hasValue) {
      final currentList = state.value!;
      final updatedList = currentList.map((order) {
        // Find order containing the item
        final itemIndex = order.items.indexWhere((i) => i.id == orderItemId);
        if (itemIndex != -1) {
          final updatedItems = List<OrderItemResponseDto>.from(order.items);
          updatedItems[itemIndex] = updatedItems[itemIndex].copyWith(status: status.index);
          return order.copyWith(items: updatedItems);
        }
        return order;
      }).toList();
      
      state = AsyncValue.data(updatedList);
    }

    try {
      await repository.updateOrderItemStatus(orderItemId, status.index);
      // Silent refresh
      await refresh();
    } catch (e) {
      // Revert on error
      state = previousState;
    }
  }
}
