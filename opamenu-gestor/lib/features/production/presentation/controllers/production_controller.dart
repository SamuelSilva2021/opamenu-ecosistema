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
    
    // Fetch both confirmed and preparing orders
    final confirmed = await repository.getOrdersByStatus(OrderStatus.confirmed.index);
    final preparing = await repository.getOrdersByStatus(OrderStatus.preparing.index);
    
    final all = [...confirmed, ...preparing];
    all.sort((a, b) => a.createdAt.compareTo(b.createdAt));
    return all;
  }

  Future<void> refresh() async {
    state = const AsyncValue.loading();
    state = await AsyncValue.guard(() => _fetchProductionOrders());
  }

  Future<void> moveNextStatus(OrderResponseDto order) async {
    final repository = ref.read(ordersRepositoryProvider);
    OrderStatus nextStatus;

    if (order.status == OrderStatus.confirmed) {
      nextStatus = OrderStatus.preparing;
    } else if (order.status == OrderStatus.preparing) {
      nextStatus = OrderStatus.ready;
    } else {
      return;
    }

    await repository.updateOrderStatus(order.id, nextStatus.index);
    await refresh();
  }
}
