import 'package:riverpod_annotation/riverpod_annotation.dart';
import '../../domain/models/create_order_request_dto.dart';
import '../../domain/models/order_response_dto.dart';
import '../../domain/models/update_order_status_request_dto.dart';
import '../datasources/orders_remote_datasource.dart';
import '../models/paged_response_model.dart';

part 'orders_repository.g.dart';

@riverpod
OrdersRepository ordersRepository(Ref ref) {
  return OrdersRepository(ref.watch(ordersRemoteDataSourceProvider));
}

class OrdersRepository {
  final OrdersRemoteDataSource _datasource;

  OrdersRepository(this._datasource);

  Future<PagedResponseModel<List<OrderResponseDto>>> getOrders({int page = 1, int pageSize = 10}) async {
    return await _datasource.getOrders(page: page, pageSize: pageSize);
  }

  Future<OrderResponseDto> createOrder(CreateOrderRequestDto order) async {
    return await _datasource.createOrder(order);
  }

  Future<OrderResponseDto> addItemsToOrder(String orderId, List<CreateOrderItemRequestDto> items) async {
    return await _datasource.addItemsToOrder(orderId, items);
  }

  Future<List<OrderResponseDto>> getOrdersByStatus(int status) async {
    return await _datasource.getOrdersByStatus(status);
  }

  Future<List<OrderResponseDto>> getOrdersByCustomer(String phone) async {
    return await _datasource.getOrdersByCustomer(phone);
  }

  Future<OrderResponseDto> updateOrderStatus(String orderId, int status, {String? notes, String? driverId}) async {
    final dto = UpdateOrderStatusRequestDto(
      status: status, 
      notes: notes,
      driverId: driverId
    );
    return await _datasource.updateOrderStatus(orderId, dto);
  }

  Future<OrderResponseDto> updateOrderItemStatus(String orderItemId, int status) async {
    final dto = UpdateOrderStatusRequestDto(status: status);
    return await _datasource.updateOrderItemStatus(orderItemId, dto);
  }
}
