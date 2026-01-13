import 'package:riverpod_annotation/riverpod_annotation.dart';
import '../../data/models/paged_response_model.dart';
import '../../data/repositories/orders_repository.dart';
import '../../domain/models/order_response_dto.dart';
import '../../domain/models/create_order_request_dto.dart';

part 'orders_controller.g.dart';

@riverpod
class OrdersController extends _$OrdersController {
  @override
  Future<PagedResponseModel<List<OrderResponseDto>>> build() async {
    final page = ref.watch(ordersPaginationProvider);
    return _fetchOrders(page);
  }

  Future<PagedResponseModel<List<OrderResponseDto>>> _fetchOrders(int page) async {
    final repository = ref.read(ordersRepositoryProvider);
    return repository.getOrders(page: page);
  }
  
  Future<void> refresh() async {
    ref.invalidateSelf();
    await future;
  }

  Future<OrderResponseDto> createOrder(CreateOrderRequestDto dto) async {
    final repository = ref.read(ordersRepositoryProvider);
    final order = await repository.createOrder(dto);
    try {
      ref.invalidateSelf();
    } catch (_) {}
    return order;
  }

  Future<OrderResponseDto> addItemsToOrder(int orderId, List<CreateOrderItemRequestDto> items) async {
    final repository = ref.read(ordersRepositoryProvider);
    final order = await repository.addItemsToOrder(orderId, items);
    try {
      ref.invalidateSelf();
    } catch (_) {}
    return order;
  }
}

@riverpod
class OrdersPagination extends _$OrdersPagination {
  @override
  int build() => 1;
  
  void setPage(int page) => state = page;
  
  void nextPage() => state++;
  
  void previousPage() {
    if (state > 1) state--;
  }
}

@riverpod
int totalOrdersCount(Ref ref) {
  final ordersAsync = ref.watch(ordersControllerProvider);
  return ordersAsync.maybeWhen(
    data: (pagedResponse) => pagedResponse.totalItems,
    orElse: () => 0,
  );
}

@riverpod
class SelectedOrderId extends _$SelectedOrderId {
  @override
  int? build() => null;
  
  void select(int? id) => state = id;
}
